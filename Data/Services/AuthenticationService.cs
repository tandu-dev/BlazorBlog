using BlazorBlog.Data.Model;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazorBlog.Data.Services
{
    public interface IAuthenticationService
    {
        BlogCredential blogCredential { get; }
        Task Initialize();
        Task Login(BlogCredential blogCredential);
        Task Logout();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private IHttpService _httpService;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;
        private IConfiguration _config;

        public BlogCredential blogCredential { get; private set; }

        public AuthenticationService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            IConfiguration config
        ) {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
            _config = config;
        }

        public async Task Initialize()
        {
            blogCredential = await _localStorageService.GetItem<BlogCredential>("credential");
        }

        public async Task Login(BlogCredential paramBlogCredential)
        {
            var url = $"{_config["GetLoginAddress"]}";
            TokenWrapper token = await _httpService.Post<TokenWrapper>(url, paramBlogCredential);
            paramBlogCredential.tokenWrapper = token;
            blogCredential = paramBlogCredential;
            await _localStorageService.SetItem("credential", blogCredential); 
        }

        public async Task Logout()
        {
            blogCredential = null;
            await _localStorageService.RemoveItem("credential");
            _navigationManager.NavigateTo("login");
        }
    }
}