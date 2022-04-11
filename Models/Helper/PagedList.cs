using Microsoft.EntityFrameworkCore;

namespace Models.Helper
{
    public class PagedList<T>
    {
        private PagedList(List<T> items, int currentPage, int totalItems, int itemsPerPage)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
            TotalPages = (int)Math.Ceiling(totalItems / (itemsPerPage * 1.0));
            this.Items.AddRange(items);
        }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public List<T> Items { get; set; } = new List<T>();

        /*
            total 50
            show 10
            skip 40
            update 55
            diff =5
            skip (10)+ diff
            15
        */
        public static async Task<PagedList<T>> CreatePagedListAsync(IQueryable<T> source, int oldTotal = 0, int currentPage = 1, int itemsPerPage = 1)
        {
            var count = await source.CountAsync();
            int diff = count - oldTotal;
            if (oldTotal <= 0 || currentPage == 1) diff = 0;
            var items = await source.Skip((currentPage - 1) * itemsPerPage + diff).Take(itemsPerPage).ToListAsync();
            return new PagedList<T>(items, currentPage+Convert.ToInt32(diff/itemsPerPage), count, itemsPerPage);
        }
        public static async Task<PagedList<T>> CreatePagedListAsync(IQueryable<T> source, Pagination pagination)
        {
            return await CreatePagedListAsync(source, currentPage: pagination.PageNumber,itemsPerPage: pagination.PageSize);
        }
        public static async Task<PagedList<T>> CreatePagedListAsync(IQueryable<T> source, DynamicPagination pagination)
        {
            return await CreatePagedListAsync(source, pagination.OldTotal, pagination.PageNumber, pagination.PageSize);
        }
    }
}
