using System.Collections.Specialized;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DiscuzSSO.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DiscuzUCenter _config;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,
        IHttpClientFactory clientFactory,
        IOptions<DiscuzUCenter> options)
    {
        _logger = logger;
        _config = options.Value;
        _httpClientFactory = clientFactory;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
    
    [HttpPost]
    public async Task<string> Register(DiscuzRegister model)
    {
        var queryStr = DiscuzHelper.ToQueryString(model);

        var inputParam = DiscuzHelper.InputEncode(queryStr, _config.UC_KEY);
        
        var nameValue = new Dictionary<string, string>
        {
            { "m", "user" },
            { "a", "register" },
            { "inajax", "2" },
            { "release", "20090212" },
            { "input", inputParam},
            {"appid", _config.UC_APPID.ToString()}
        };

        // using var content = new FormUrlEncodedContent(nameValue);
        //
        // var client = _httpClientFactory.CreateClient("Discuz");
        // var response = await client.PostAsync(_config.UC_ROUTER, content);

        // return await response.Content.ReadAsStringAsync();
        
        var socketRsp = await DiscuzHelper.SendBySocketAsync(_config.UC_API, nameValue);
        return socketRsp;
    }
    
    [HttpPost]
    public async Task<string> Login(DiscuzLogin model)
    {
        var queryStr = DiscuzHelper.ToQueryString(model);

        var inputParam = DiscuzHelper.InputEncode(queryStr, _config.UC_KEY);
        
        var nameValue = new Dictionary<string, string>
        {
            { "m", "user" },
            { "a", "login" },
            { "inajax", "2" },
            { "release", "20090212" },
            { "input", inputParam},
            {"appid", _config.UC_APPID.ToString()}
        };
        
        var socketRsp = await DiscuzHelper.SendBySocketAsync(_config.UC_API, nameValue);
        return socketRsp;
    }
    
    [HttpPost]
    public async Task<string> SyncLogin(DiscuzSyncLogin model)
    {
        var queryStr = DiscuzHelper.ToQueryString(model);

        var inputParam = DiscuzHelper.InputEncode(queryStr, _config.UC_KEY);
        
        var nameValue = new Dictionary<string, string>
        {
            { "m", "user" },
            { "a", "synlogin" },
            { "inajax", "2" },
            { "release", "20090212" },
            { "input", inputParam},
            {"appid", _config.UC_APPID.ToString()}
        };
        
        var socketRsp = await DiscuzHelper.SendBySocketAsync(_config.UC_API, nameValue);
        return socketRsp;
    }
}