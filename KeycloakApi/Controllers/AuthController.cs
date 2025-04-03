using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeycloakApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var redirectUri = "http://localhost:5267/api/auth/callback";
            var authUrl = $"http://localhost:8080/realms/master/protocol/openid-connect/auth?client_id=TestClient&response_type=code&scope=openid%20profile%20email&redirect_uri={Uri.EscapeDataString(redirectUri)}";
            return Redirect(authUrl);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            var client = _httpClientFactory.CreateClient();
            var tokenEndpoint = "http://localhost:8080/realms/master/protocol/openid-connect/token";
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", "http://localhost:5267/api/auth/callback" },
                { "client_id", "TestClient" },
                { "client_secret", "cptGPe6umHay3zUEJXk9qKXBFaQjCe6u" } // Replace with actual secret
            });

            var response = await client.PostAsync(tokenEndpoint, content);
            if (response.IsSuccessStatusCode)
            {
                var tokens = await response.Content.ReadAsStringAsync();
                return Redirect($"http://localhost:4200/callback?tokens={Uri.EscapeDataString(tokens)}");
            }
            return BadRequest(await response.Content.ReadAsStringAsync());
        }

        [HttpGet("userinfo")]
        public async Task<IActionResult> GetUserInfo(string accessToken)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var userInfoEndpoint = "http://localhost:8080/realms/master/protocol/openid-connect/userinfo";
            var response = await client.GetAsync(userInfoEndpoint);
            if (response.IsSuccessStatusCode)
            {
                var userInfo = await response.Content.ReadAsStringAsync();
                return Ok(userInfo);
            }
            return Unauthorized();
        }
    }
}