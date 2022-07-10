    using System.ComponentModel.DataAnnotations;
    namespace BlazorBlog.Data.Model {
    
        public class Post
        {
            public int Id { get; set; }
            [Required]
            public string? Title { get; set; }
            [Required]
            public string? Contents { get; set; }
            public DateTime Timestamp {get; set;}
            [Required]
            [Range(1,3, ErrorMessage = "Please choose a Category")]
            public int CategoryId {get; set;}
            public string? CategoryName {get; set;}
        }
        public class Category {
            public int CategoryId {get; set;}

            public string? CategoryName {get;set;}
        }
    }