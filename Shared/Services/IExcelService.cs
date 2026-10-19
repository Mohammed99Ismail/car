using System.Data;
using ClinicAPI.Shared;

namespace ClinicAPI.Shared.Services
{
    public interface IExcelService
    {
        Task<string> ExportAsync<TData>(IEnumerable<TData> data, Dictionary<string, Func<TData, object>> mappers, string sheetName = "Sheet1", bool isArabic = false);
        Task<CustomApiResponse<IEnumerable<TEntity>>> ImportAsync<TEntity>(Stream stream, Dictionary<string, Func<DataRow, TEntity, object>> mappers, string sheetName = "Sheet1");
        Task<string> ExportServicesReportAsync(ClinicAPI.Features.Reports.DTO.ServicesReportResponseDto reportData, string sheetName = "Services Report");
        Task<string> ExportPatientsInquiryReportAsync(ClinicAPI.Features.Reports.DTO.PatientsInquiryReportResponseDto reportData, string sheetName = "Patients Analysis");
    }
}
