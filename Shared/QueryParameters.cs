namespace ClinicAPI.Shared
{
    public class QueryParameters
    {
        /// <summary>
        ///  Set the default Page size 10
        /// </summary>
        private int _pageSize = 10;

        /// <summary>
        /// Get or Set Page Number
        /// </summary>
        public int Page { get; set; } = 0;


        /// <summary>
        /// Set The  Maximum Page Size
        /// </summary>
        private const int MAX_PAGE_SIZE = 100;


        /// <summary>
        ///  Set or get the PageSize
        /// </summary>
        public int Limit
        {
            get => _pageSize;
            set => _pageSize = (value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : value;
        }

        public int Skip { get => (Page) * Limit; }

        /// <summary>
        ///  Set or get the OrderName
        /// </summary>
        public string? OrderName { get; set; }

        /// <summary>
        ///  Set or get the OrderDirection
        /// </summary>
        public string? OrderDirection { get; set; }
    }
}
