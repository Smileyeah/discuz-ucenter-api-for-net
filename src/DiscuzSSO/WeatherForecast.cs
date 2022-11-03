namespace DiscuzSSO;

public class WeatherForecast
{
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}

public class DiscuzRegister
{
    public string password { get; set; }
    
    public string questionid { get; set; }
    
    public string answer { get; set; }
    
    public string email { get; set; }
    
    public string username { get; set; }

}

public class DiscuzLogin
{
    public string password { get; set; }
    
    public string questionid { get; set; }
    
    public string answer { get; set; }
    
    public string isuid { get; set; }
    
    public string checkques { get; set; }
    
    public string username { get; set; }

}

public class DiscuzSyncLogin
{
    public string uid { get; set; }
}

public class DiscuzUCenter
{
    public string UC_IP { get; set; }
    public string UC_API { get; set; }
    public string UC_ROUTER { get; set; }
    public string UC_KEY { get; set; }
    public int UC_APPID { get; set; }
}