namespace ProjectMVC.Models
{
	public class Pager
	{
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }

        public Pager()
        {
            
        }

        public Pager(int totalItems, int page, int pageSize = 9)
        {
            int totalPages = (int)Math.Ceiling(totalItems / (decimal)pageSize);
            int cureentPage = page;

            int startPage = cureentPage - 5;
            int endPage = cureentPage + 4;

            if (startPage <= 0) 
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages) 
            { 
                endPage = totalPages;
                if (endPage > 10) 
                {
                    startPage = endPage - 9;
                }

            }

            TotalItems = totalItems;
            CurrentPage = cureentPage;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
            PageSize = pageSize;


        } 
    }
}
