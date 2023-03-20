using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PasteboardProject.Services;

public class IpInformationService
{
    static string _apiUrl = $"http://ip-api.com/json/";
    public static async Task<string> GetCityFromIp(string ip)
    {
        if (ip == "::1")
        {
            return "LocalMachine";
        }
        var json = "";
        var city = "";
        using (var client = new HttpClient())
        {
            
            var response = await client.GetAsync(_apiUrl + $"{ip}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var jsonData = (JObject)JsonConvert.DeserializeObject(result);
            city = jsonData["city"].Value<string>();
        }
        return city;
    }
}