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


        public static async Task<PagedList<T>> CreatePagedListAsync(IQueryable<T> source, int currentPage, int itemsPerPage)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();
            return new PagedList<T>(items, currentPage, count, itemsPerPage);
        }
    }
}