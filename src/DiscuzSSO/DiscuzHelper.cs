using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DiscuzSSO;

public static class DiscuzHelper
{
    public static string TransferToQuery(Dictionary<string, string> collection)
    {
        return string.Join("&", collection.Select(i => i.Key + "=" + i.Value));
    }

    private static string MD5Encode(string toEncode)
    {
        using var md5 = MD5.Create();
        
        var hashDatas= md5.ComputeHash(Encoding.UTF8.GetBytes(toEncode));
        var builder = new StringBuilder();
        // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串 
        foreach (var data in hashDatas)
        {
            builder.Append(data.ToString("x2"));
        }
        
        return builder.ToString();
    }

    private static string Base64Encode(string toEncode)
    {
        var plainTextBytes = Encoding.Latin1.GetBytes(toEncode);
        return Convert.ToBase64String(plainTextBytes);
    }
    
    private static string? UrlEncode(string? toEncode)
    {
        return WebUtility.UrlEncode(toEncode);
    }

    public static string ToQueryString(object queryModel)
    {
        var properties = queryModel.GetType().GetProperties();

        var queryString = string.Join("&", properties.Select(d => $"{d.Name}={UrlEncode(d.GetValue(queryModel)?.ToString())}"));

        return queryString;
    }

    private static string AuthEncode(string toAuth, string key)
    {
        var ckeyLen = 4;    //note 随机密钥长度 取值 0-32;
        //note 加入随机密钥，可以令密文无任何规律，即便是原文和密钥完全相同，加密结果也会每次不同，增大破解难度。
        //note 取值越大，密文变动规律越大，密文变化 = 16 的 ckey_length 次方
        //note 当此值为 0 时，则不产生随机密钥

        key = MD5Encode(key);
        var keyA = MD5Encode(key.Substring(0, 16));
        var keyB = MD5Encode(key.Substring(16, 16));
        var keyC = MD5Encode("1667440721000"/*DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()*/)[^ckeyLen..];
        
        var cryptKey = keyA + MD5Encode(keyA + keyC);
        var keyLen = cryptKey.Length;

        toAuth = "0000000000" + MD5Encode(toAuth + keyB).Substring(0, 16) + toAuth;
        var strLen = toAuth.Length;

        var builder = new StringBuilder();

        var box = new int[256];
        for (var i = 0; i < 256; i++) {
            box[i] = i;
        }

        var rndKey = new int[256];
        for (var i = 0; i <= 255; i++) {
            rndKey[i] = cryptKey.ElementAt(i % keyLen);
        }

        int j = 0;
        for (int i = 0; i < 256; i++) {
            j = (j + box[i] + rndKey[i]) % 256;
            (box[i], box[j]) = (box[j], box[i]);
        }

        j = 0;
        var a = 0;
        for (var i = 0; i < strLen; i++) {
            a = (a + 1) % 256;
            j = (j + box[a]) % 256;
            (box[a], box[j]) = (box[j], box[a]);

            builder.Append((char) ((int) toAuth.ElementAt(i) ^ box[(box[a] + box[j]) % 256]));

        }

        var result = Base64Encode(builder.ToString());
        
        return keyC + result.Replace("=", "");
    }

    public static string? InputEncode(string toAuth, string key)
    {
        return UrlEncode(AuthEncode(toAuth + "&agent=" + MD5Encode("") + "&time=" + "1667440721"/*DateTimeOffset.UtcNow.ToUnixTimeSeconds()*/, key));
    }


    public static async Task<string> SendBySocketAsync(string url, Dictionary<string, string> nvPair)
    {
        var matches = new Uri(url);
        var host = matches.Host;
        var path = matches.PathAndQuery;
        var port = matches.Port;

        var buffer = new StringBuilder();
        var postData = string.Join("&", nvPair.Select(kv => $"{kv.Key}={kv.Value}"));
        if (postData is { Length: > 0 })
        {
            buffer.Append("POST ").Append(path).Append(" HTTP/1.0\r\n");
            buffer.Append("Accept: */*\r\n");
            buffer.Append("Accept-Language: zh-cn\r\n");
            buffer.Append("Content-Type: application/x-www-form-urlencoded\r\n");
            buffer.Append("User-Agent: \r\n");
            buffer.Append("Host: ").Append(host).Append("\r\n");
            buffer.Append("Content-Length: ").Append(postData.Length).Append("\r\n");
            buffer.Append("Connection: Close\r\n");
            buffer.Append("Cache-Control: no-cache\r\n");
            buffer.Append("Cookie: \r\n\r\n");
            buffer.Append(postData);
        }

        var returnString = new StringBuilder();
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        await socket.ConnectAsync(host, port);

        var sendBuffer = Encoding.UTF8.GetBytes(buffer.ToString());
        await socket.SendAsync(sendBuffer, SocketFlags.None);

        while (true)
        {
            var rcvBuffer = new byte[1024 * 1024 * 3];
            var rcvLen = await socket.ReceiveAsync(rcvBuffer, SocketFlags.None);
            if (rcvLen == 0)
            {
                break;
            }
            
            returnString.Append(Encoding.UTF8.GetString(rcvBuffer, 0, rcvLen));
        }

        socket.Close();

        return returnString.ToString();
    }
}