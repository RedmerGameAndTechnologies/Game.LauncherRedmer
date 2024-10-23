using System.Net.Http;
using System.Threading.Tasks;

namespace LauncherLes1.View.Resources.Script
{
    class HttpsClientClass
    {
        public static async Task<string> HttpResponse(string line)
        {
            using (var net = new HttpClient())
            {
                var response = await net.GetAsync(line);
                return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
            }
        }
    }
}
