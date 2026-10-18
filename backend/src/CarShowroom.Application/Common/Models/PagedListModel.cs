namespace CarShowroom.Application.Common.Models;

/// <summary>
/// Paged response model (adapted from Shared/PagedListModel).
/// </summary>
public class PagedListModel<T>
{
    /// <summary>Get or set data</summary>
    public IReadOnlyList<T> Data { get; set; } = [];

    /// <summary>Get or Set Total Count</summary>
    public int Length { get; set; }

    /// <summary>Get or Set Current Page Index</summary>
    public int Page { get; set; }

    /// <summary>Get or Set Page size</summary>
    public int Limit { get; set; }

    /// <summary>Total pages</summary>
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

/// <summary>Bilingual lookup item (adapted from Shared/LookupDetailsDto).</summary>
public record LookupDto(int Id, string NameAr, string NameEn);

/// <summary>Single-name lookup item (car makes, models, branches...).</summary>
public record LookupItemDto(int Id, string Name);
