using BlazorBlog.Data.Model;
using System.Text.Json;

namespace BlazorBlog.Data.Services
{

    public interface IApiService
    {
        Task<IEnumerable<Post>> GetPosts(PageParameters pg);

        Task<Post> GetPostAsync(string Id);
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Post> AddPost(Post model);
        Task DeletePostAsync(string Id);
        Task<Post> UpdatePost(Post model);
    }
    public class ApiService : IApiService
    {
        public ApiService(IHttpService httpService, IConfiguration config)
        {
            HttpService = httpService;
            Config = config;
        }

        public IHttpService HttpService { get; }
        public IConfiguration Config { get; }

        public async Task<Post> GetPostAsync(string Id)
        {
            var url = $"{Config["GetPostAddress"]}/{Id}";
            var response = await HttpService.Get<Post>(url);
            return response;
        }

        public async Task<IEnumerable<Post>> GetPosts(PageParameters pg)
        {
            IEnumerable<Post> posts = Array.Empty<Post>();

            var url = $"{Config["GetPostAddress"]}/{pg.PageNumber}/{pg.PageSize}";
            var response = await HttpService.Get<IEnumerable<Post>>(url);
            return response;
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            IEnumerable<Category> posts = Array.Empty<Category>();
            var url = $"{Config["BaseAddress"]}/categories";
            var response = await HttpService.Get<IEnumerable<Category>>(url);
            return response;
        }
        public async Task<Post> AddPost(Post model)
        {
            var url = $"{Config["GetPostAddress"]}";
            var response = await HttpService.Post<Post>(url, model);
            return model;
        }

        public async Task DeletePostAsync(string Id)
        {
            var url = $"{Config["GetPostAddress"]}/{Id}";
            var response = await HttpService.Delete<Post>(url);
        }

        public async Task<Post> UpdatePost(Post model)
        {
            var url = $"{Config["GetPostAddress"]}/{model.Id}";
            var response = await HttpService.Put<Post>(url, model);
            return response;
        }
    }
}