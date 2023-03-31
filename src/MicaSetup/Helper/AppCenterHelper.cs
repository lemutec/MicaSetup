using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace MicaSetup.Helper;

public static class AppCenterHelper
{
    [SuppressMessage("CodeQuality", "IDE0052:")]
    private static readonly Lazy<HttpClient> client = new();

    public static async Task SendPageViewAsync(string page, string title = null!)
    {
        try
        {
            await Task.CompletedTask;
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}
