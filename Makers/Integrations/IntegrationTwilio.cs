using Newtonsoft.Json;
using System.Net;
using Makers.Utilities;

namespace Makers.Integrations;

public  static class IntegrationTwilio
{

    public static async Task<bool> AddPhoneNumber(string phoneNumber,
        string deviceModel,
        string deviceOs,
        string country,
        ILogger logger)
                                                        
    {
        logger.LogMsg($"Will send try to add new phoe number {phoneNumber}");



        var userInfo = new
        {
            app_id = Constants.appId,
            identifier = phoneNumber,
            device_type = 14,
            language = "en",
            timezone = "-28800",
            game_version = "1.1.1",
            device_model = deviceModel.ToString(),
            device_os = deviceOs.ToString(),
            session_count = 600,
            /// tags = new { first_name = " ", last_name = " ", level = "99", amount_spent = "6000", account_type = "VIP", key_to_delete = "" },
            // amount_spent = "100.99",
            playtime = 600,
            notification_types = 1,
            lat = 37.563,
            //long= 122.3255,
            country = country,
            timezone_id = "Europe/Warsaw"
        };


        string body = JsonConvert.SerializeObject(userInfo);



        var httpRequest1 = (HttpWebRequest)WebRequest.Create("https://onesignal.com/api/v1/players");
        httpRequest1.Method = "POST";
        httpRequest1.ContentType = "application/json";
        httpRequest1.Headers["Authorization"] = "Basic ZDQ0NjE2N2YtMmM3MC00MDRlLTk3MTctYTgyMDE4ZDhhZTI5";



        using (var streamWriter = new StreamWriter(httpRequest1.GetRequestStream()))
        {

            streamWriter.Write(body);
        }

        var httpResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
        using (var streamReader = new StreamReader(httpResponse1.GetResponseStream()))
        {
            var result = streamReader.ReadToEnd();
        }

        Console.WriteLine(httpResponse1.StatusCode);

        return true ;
    }

}