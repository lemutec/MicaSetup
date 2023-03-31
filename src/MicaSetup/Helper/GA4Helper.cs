using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicaSetup.Helper;

public static class GA4Helper
{
    private static readonly Lazy<HttpClient> client = new();

    public static string ScreenResolution
        => $"{Screen.PrimaryScreen.Bounds.Width}x{Screen.PrimaryScreen.Bounds.Height}";

    public static string UserAgent
        => $"MicaSetup Google Analytics 4 Helper {MicaInfo.Version} ({Environment.OSVersion.VersionString})";

    public static async Task SendPageViewAsync(string page, string title = null!)
    {
        try
        {
            string json = $@"""
            {{
                ""measurement_id"": ""{Option.Current.MeasurementID}"",
                ""client_id"": ""{DeviceHelper.DeviceID}"",
                ""user_agent"": ""{UserAgent}"",
                ""event"": ""page_view"",
                ""screen_resolution"": ""{ScreenResolution}"",
                ""page_location"": ""{$"https://github.com/lemutec/MicaSetup/{page.TrimStart('/')}"}"",
                ""page_title"": ""{title ?? string.Empty}"",
            }}""";
            StringContent content = new(json.Trim('"'), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.Value.PostAsync("https://www.google-analytics.com/mp/collect", content);
            _ = response.StatusCode;
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}
