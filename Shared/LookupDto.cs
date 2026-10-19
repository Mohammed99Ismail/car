namespace ClinicAPI.Shared
{
    public class LookupDto
    {
        public int? Id { get; set; }
        public required string NameAr { get; set; }
        public string? NameEn { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
