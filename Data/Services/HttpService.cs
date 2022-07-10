using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BlazorBlog.Data.Model;

namespace BlazorBlog.Data.Services
{
    public interface IHttpService
    {
        Task<T> Get<T>(string uri);
        Task<T> Post<T>(string uri, object value);
        Task<T> Delete<T>(string uri);
        Task<T> Put<T>(string uri, object value);
    }

    public class HttpService : IHttpService
    {
        private HttpClient _httpClient;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;
        private IConfiguration _configuration;

        public HttpService(
            HttpClient httpClient,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            IConfiguration configuration
        ) {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
            _configuration = configuration;
        }

        public async Task<T> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await sendRequest<T>(request);
        }

        public async Task<T> Post<T>(string uri, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            JsonSerializerOptions options = new JsonSerializerOptions() {
                 PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            request.Content = new StringContent(JsonSerializer.Serialize(value, options), Encoding.UTF8, "application/json");
            return await sendRequest<T>(request);
        }
        public async Task<T> Delete<T> (string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            return await sendRequest<T>(request);
        }
        // helper methods

        private async Task<T> sendRequest<T>(HttpRequestMessage request)
        {
            // add jwt auth header if user is logged in and request is to the api url
            var credential = await _localStorageService.GetItem<BlogCredential>("credential");
            var isApiUrl = request.RequestUri.IsAbsoluteUri;
            if (credential != null && isApiUrl)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", credential.tokenWrapper.token);

            using var response = await _httpClient.SendAsync(request);

            // auto logout on 401 response
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    _navigationManager.NavigateTo("logout");
                    return default;
                
                case HttpStatusCode.BadRequest:
                    _navigationManager.NavigateTo("/addpost");
                    return default;                
            }

            // throw exception on error response
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                throw new Exception(error["message"]);
            }
            JsonSerializerOptions options = new JsonSerializerOptions() {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        };
            return await response.Content.ReadFromJsonAsync<T>(options);
        }

        public async Task<T> Put<T>(string uri, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri);
            JsonSerializerOptions options = new JsonSerializerOptions() {
                 PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            request.Content = new StringContent(JsonSerializer.Serialize(value, options), Encoding.UTF8, "application/json");
            return await sendRequest<T>(request);
        }
    }
}