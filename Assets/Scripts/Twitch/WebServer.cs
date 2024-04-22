#nullable enable

using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace AvatarViewer.Twitch
{
    public class WebServer
    {
        private HttpListener listener;

        public WebServer(string uri)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(uri);
            listener.Prefixes.Add("http://localhost:24000/token/");
        }

        public void Start()
        {
            listener.Start();
        }

        public async Task<string?> Listen()
        {
            try
            {
                while (listener.IsListening)
                {
                    var ctx = await listener.GetContextAsync();
                    var req = ctx.Request;
                    var resp = ctx.Response;

                    await using (var writer = new StreamWriter(resp.OutputStream))
                    {
                        if (req.Url.AbsolutePath == "/token/")
                        {
                            using var reader = new StreamReader(req.InputStream, req.ContentEncoding);
                            var data = await reader.ReadToEndAsync();
                            if (data.Contains("access_token"))
                            {
                                await writer.WriteLineAsync("");
                                await writer.FlushAsync();
                                return data.Split('&')[0].Split('=')[1];
                            }
                            else
                            {
                                await writer.WriteLineAsync("");
                                await writer.FlushAsync();
                                return null;
                            }
                        }
                        else if (req.Url.Query.Contains("error"))
                        {
                            var res = "<html><head><style>body { font-family: -apple-system, BlinkMacSystemFont, \"Segoe UI\", Roboto, Helvetica, Arial, sans-serif, \"Apple Color Emoji\", \"Segoe UI Emoji\", \"Segoe UI Symbol\"; }</style></head><body><b>Access denied. Please try again.</b></body><html>";
                            await writer.WriteLineAsync(res);
                            await writer.FlushAsync();
                            return null;
                        }
                        else
                        {
                            var res = "<html><head><style>body { font-family: -apple-system, BlinkMacSystemFont, \"Segoe UI\", Roboto, Helvetica, Arial, sans-serif, \"Apple Color Emoji\", \"Segoe UI Emoji\", \"Segoe UI Symbol\"; }</style></head><body><b>Authorization completed! You can close this tab</b><script>fetch(\"http://localhost:24000/token/\", {method: \"POST\", body: window.location.hash.substring(1) })</script></body><html>";
                            await writer.WriteLineAsync(res);
                            await writer.FlushAsync();
                        }
                    }
                }
            }
            finally
            {
                listener.Stop();
            }
            return null;
        }
    }
}
