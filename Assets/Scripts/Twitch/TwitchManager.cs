#nullable enable
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using UnityEngine;

namespace AvatarViewer.Twitch
{

    public class TwitchManager
    {
        public string ClientId { get; } = "dbssbmbve5yi1qg9tthugzoldqi6yh";
        private string RedirectUrl { get; } = "http://localhost:24000/redirect/";

        public async Task GenerateClientSecret()
        {
            var server = new WebServer(RedirectUrl);

            var url = "https://id.twitch.tv/oauth2/authorize" +
                "?response_type=token" +
                $"&client_id={ClientId}" +
                $"&redirect_uri={HttpUtility.UrlEncode(RedirectUrl)}" +
                $"&scope={HttpUtility.UrlEncode("channel:read:redemptions")}";

            server.Start();

            Application.OpenURL(url);

            var accessToken = await server.Listen();

            PlayerPrefs.SetString("TwitchAccessToken", accessToken);
        }

        public async Task<bool> ValidateToken()
        {
            using var client = new HttpClient();

            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://id.twitch.tv/oauth2/validate");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("OAuth", PlayerPrefs.GetString("TwitchAccessToken"));

            using var res = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

            if (res.StatusCode == HttpStatusCode.Unauthorized)
            {
                return false;
            }
            else
            {
                var response = JsonConvert.DeserializeObject<TwitchValidateResponse>(await res.Content.ReadAsStringAsync());
                PlayerPrefs.SetString("UserId", response!.user_id);
                return true;
            }
        }

    }

    internal class TwitchValidateResponse
    {
        public string client_id { get; set; } = null!;
        public string login { get; set; } = null!;
        public string[] scopes { get; set; } = null!;
        public string user_id { get; set; } = null!;
        public int expires_in { get; set; }
    }
}