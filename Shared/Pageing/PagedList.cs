using Microsoft.EntityFrameworkCore;


namespace ClinicAPI.Shared.Pageing
{
    /// <summary>
    /// Paged list
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    [Serializable]
    public class PagedList<T> : List<T>
    {

        public PagedList(IEnumerable<T> items, int total, int pageIndex, int pageSize, bool getOnlyTotalCount = false)
        {

            if (pageSize <= 0)
                pageSize = total;

            TotalCount = total;
            TotalPages = total / pageSize;

            if (total % pageSize > 0)
                TotalPages++;

            PageSize = pageSize;
            PageIndex = pageIndex;

            if (getOnlyTotalCount)
                return;

            AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();

            source = source.Skip(pageNumber * pageSize);

            if (pageSize > 0)// if 0 is passed, retrieve all records
                source = source.Take(pageSize);

            var items = await source.ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        public static PagedList<T> Create(IList<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();

            var SourceEnumarable = source.Skip(pageNumber * pageSize);

            List<T> items;
            if (pageSize > 0)// if 0 is passed, retrieve all records
            {
                items = SourceEnumarable.Take(pageSize)
                                        .ToList();
            }
            else
                items = SourceEnumarable.ToList();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }



        /// <summary>
        /// Page index
        /// </summary>
        public int PageIndex { get; }

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Total count
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Total pages
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// Has previous page
        /// </summary>
        //public bool HasPreviousPage => PageIndex > 0;

        /// <summary>
        /// Has next page
        /// </summary>
        //public bool HasNextPage => PageIndex + 1 < TotalPages;
    }
}
