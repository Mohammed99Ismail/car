namespace ClinicAPI.Shared
{
    public class PagedListModel<T> where T : class
    {
        /// <summary>
        /// Get or set data
        /// </summary>
        public IList<T>? Data { get; set; }

        /// <summary>
        /// Get or Set Total Count
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Get or Set Current Page Index
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Get or Set Page size
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Get or Set Page size
        /// </summary>


        public int Pages
        {
           
            get
            {
                if (Limit == 0)
                    return 1;

                int totalPages = Math.DivRem(Length, Limit, out int remainder);

                if (remainder > 0)
                    totalPages++;

                return totalPages;
            }
           
        }
    }
}
