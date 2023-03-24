using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Exception = System.Exception;

namespace PasteboardProject.Services;

public static class IpInformationService
{
    private const string ApiUrl = "http://ip-api.com/json/";
    private const string ApiUrl2 = "https://ipapi.co/";
    public static async Task<string> GetCityFromIp(string ip)
    {
        if (ip == "::1")
        {
            return "LocalMachine";
        }
        var city = "";
        using var client = new HttpClient();
        try
        {
            var response = await client.GetAsync(ApiUrl + $"{ip}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            if (result == null) throw new HttpRequestException();
            var jsonData = (JObject)JsonConvert.DeserializeObject(result)!;
            city = (jsonData["city"] ?? throw new HttpRequestException()).Value<string>();
            return city;
        }
        catch (HttpRequestException)
        {
            var response = await client.GetAsync(ApiUrl2 + $"{ip} + /json/");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            if (result == null) throw new HttpRequestException();
            var jsonData = (JObject)JsonConvert.DeserializeObject(result)!;
            city = (jsonData["city"] ?? throw new Exception()).Value<string>();
            return city;
        }
        catch (Exception)
        {
            return "Error";
        }
    }
}