namespace BlazorBlog.Data.Model {

public class PageParameters
    {
        const int maxPageSize = 10;
        public int PageNumber = 1;
        private int _pagesize = 2;

        public int PageSize
        {
            get { return _pagesize; }
            set { _pagesize = (value > maxPageSize) ? maxPageSize : value; }
        }
    }
}