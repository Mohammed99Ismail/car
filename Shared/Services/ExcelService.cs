using System.Data;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ClinicAPI.Shared;
using ClinicAPI.Shared.Enums;
using ClinicAPI.Features.Reports.DTO;

namespace ClinicAPI.Shared.Services
{
    public class ExcelService : IExcelService
    {
        public ExcelService()
        {
            // Set EPPlus license for non-commercial use (EPPlus 8+ approach)
            ExcelPackage.License.SetNonCommercialPersonal("ClinicAPI");
        }

        public async Task<string> ExportAsync<TData>(IEnumerable<TData> data, Dictionary<string, Func<TData, object>> mappers, string sheetName = "Sheet1", bool isArabic = false)
        {
            using var p = new ExcelPackage();
            p.Workbook.Properties.Author = "ClinicAPI";
            p.Workbook.Worksheets.Add("Data");
            var ws = p.Workbook.Worksheets[0];
            ws.Name = sheetName;
            ws.Cells.Style.Font.Size = 11;
            ws.Cells.Style.Font.Name = isArabic ? "Arial" : "Calibri";

            // Set worksheet-level RTL direction for Arabic
            if (isArabic)
            {
                ws.View.RightToLeft = true;
            }

            var colIndex = 1;
            var rowIndex = 1;

            var headers = mappers.Keys.Select(x => x).ToList();

            // For Arabic, reverse the column order to start from right
            if (isArabic)
            {
                headers.Reverse();
            }

            foreach (var header in headers)
            {
                var cell = ws.Cells[rowIndex, colIndex];

                var fill = cell.Style.Fill;
                fill.PatternType = ExcelFillStyle.Solid;
                fill.BackgroundColor.SetColor(Color.LightBlue);

                var border = cell.Style.Border;
                border.Bottom.Style =
                    border.Top.Style =
                        border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                // Apply RTL formatting for Arabic
                if (isArabic)
                {
                    cell.Style.ReadingOrder = ExcelReadingOrder.RightToLeft;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }

                cell.Value = header;

                colIndex++;
            }

            var dataList = data.ToList();
            foreach (var item in dataList)
            {
                colIndex = 1;
                rowIndex++;

                // For Arabic, we need to process the data in the same order as headers (reversed)
                var result = headers.Select(header => mappers[header](item));

                foreach (var value in result)
                {
                    var dataCell = ws.Cells[rowIndex, colIndex];
                    dataCell.Value = value;
                    
                    // Apply RTL formatting for Arabic data cells
                    if (isArabic)
                    {
                        dataCell.Style.ReadingOrder = ExcelReadingOrder.RightToLeft;
                        dataCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    
                    colIndex++;
                }
            }

            using (ExcelRange autoFilterCells = ws.Cells[1, 1, dataList.Count + 1, headers.Count])
            {
                autoFilterCells.AutoFilter = true;
                autoFilterCells.AutoFitColumns();
            }

            var byteArray = await p.GetAsByteArrayAsync();
            return Convert.ToBase64String(byteArray);
        }

        public async Task<CustomApiResponse<IEnumerable<TEntity>>> ImportAsync<TEntity>(Stream stream, Dictionary<string, Func<DataRow, TEntity, object>> mappers, string sheetName = "Sheet1")
        {
            var result = new List<TEntity>();
            var response = new CustomApiResponse<IEnumerable<TEntity>> { ResultStatus = ResultStatusEnum.Ok };
            
            try
            {
                using var p = new ExcelPackage();
                stream.Position = 0;
                await p.LoadAsync(stream);

                var ws = p.Workbook.Worksheets[sheetName];
                if (ws == null)
                {
                    response.ResultStatus = ResultStatusEnum.BadRequest;
                    response.Message = string.Format("Sheet with name {0} does not exist!", sheetName);
                    return response;
                }

                var dt = new DataTable();
                var titlesInFirstRow = true;
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    dt.Columns.Add(titlesInFirstRow ? firstRowCell.Text : $"Column {firstRowCell.Start.Column}");
                }

                var startRow = titlesInFirstRow ? 2 : 1;
                var headers = mappers.Keys.ToList();
                var errors = new List<string>();

                // Validate headers
                foreach (var header in headers)
                {
                    if (!dt.Columns.Contains(header))
                    {
                        errors.Add(string.Format("Header '{0}' does not exist in table!", header));
                    }
                }

                if (errors.Any())
                {
                    response.ResultStatus = ResultStatusEnum.BadRequest;
                    response.Message = string.Join(", ", errors);
                    return response;
                }

                // Process rows
                for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    try
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        DataRow row = dt.Rows.Add();
                        var item = (TEntity)Activator.CreateInstance(typeof(TEntity));

                        // Add cell values to DataRow
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Text;
                        }

                        // Apply mappers to map the DataRow to TEntity
                        headers.ForEach(x => mappers[x](row, item));

                        result.Add(item);
                    }
                    catch (Exception e)
                    {
                        response.ResultStatus = ResultStatusEnum.Error;
                        response.Message = e.Message;
                        return response;
                    }
                }

                response.Data = result;
                response.Message = "Import Success";
                return response;
            }
            catch (Exception ex)
            {
                response.ResultStatus = ResultStatusEnum.Error;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<string> ExportServicesReportAsync(ServicesReportResponseDto reportData, string sheetName = "Services Report")
        {
            using var p = new ExcelPackage();
            var ws = p.Workbook.Worksheets.Add(sheetName);
            ws.View.RightToLeft = true; // Arabic report
            ws.Cells.Style.Font.Name = "Arial";
            ws.Cells.Style.Font.Size = 11;

            int col = 1;
            int headerRow1 = 1;
            int headerRow2 = 2;

            // 1. Fixed Columns at the beginning (Date, Day, Customer Count, Cases count)
            // Note: In the image, these are at the RIGHT side for Arabic. 
            // In EPPlus RTL, Col 1 is the right-most.

            // Static Columns
            ws.Cells[headerRow1, col, headerRow1, col + 3].Merge = true;
            ws.Cells[headerRow1, col].Value = "بيانات اليوم";
            ws.Cells[headerRow1, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[headerRow1, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[headerRow1, col].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
            ws.Cells[headerRow1, col].Style.Font.Color.SetColor(Color.White);

            ws.Cells[headerRow2, col++].Value = "التاريخ";
            ws.Cells[headerRow2, col++].Value = "اليوم";
            ws.Cells[headerRow2, col++].Value = "عدد الجلسات";
            ws.Cells[headerRow2, col++].Value = "عدد العملاء";

            // Cases Info (New/Old Patient)
            ws.Cells[headerRow1, col, headerRow1, col + 1].Merge = true;
            ws.Cells[headerRow1, col].Value = "عدد الحالات";
            ws.Cells[headerRow1, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[headerRow1, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[headerRow1, col].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
            ws.Cells[headerRow1, col].Style.Font.Color.SetColor(Color.White);

            ws.Cells[headerRow2, col++].Value = "New Patient";
            ws.Cells[headerRow2, col++].Value = "Old Patient";

            // Professional color palette for categories
            var palette = new[] { 
                Color.FromArgb(221, 235, 247), // Light Blue
                Color.FromArgb(226, 239, 218), // Light Green
                Color.FromArgb(255, 242, 204), // Light Yellow
                Color.FromArgb(252, 228, 214), // Light Orange
                Color.FromArgb(235, 231, 245), // Light Purple
                Color.FromArgb(217, 217, 217)  // Light Grey
            };
            int colorIndex = 0;
            int dataStartRow = 3;

            // Dynamic Categories
            foreach (var category in reportData.Categories)
            {
                int serviceCount = category.ServiceNames.Count;
                if (serviceCount == 0) continue;

                var sectionColor = palette[colorIndex % palette.Length];

                // Header Row 1
                var catHeader = ws.Cells[headerRow1, col, headerRow1, col + serviceCount - 1];
                catHeader.Merge = true;
                catHeader.Value = category.CategoryName;
                catHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                catHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                catHeader.Style.Fill.BackgroundColor.SetColor(sectionColor);
                catHeader.Style.Font.Bold = true;
                catHeader.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                foreach (var service in category.ServiceNames)
                {
                    // Header Row 2
                    var servHeader = ws.Cells[headerRow2, col];
                    servHeader.Value = service;
                    servHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    servHeader.Style.Fill.BackgroundColor.SetColor(sectionColor);
                    servHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    servHeader.Style.Font.Size = 10;
                    
                    // Lightly color the entire column for this category
                    var columnRange = ws.Cells[3, col, dataStartRow + reportData.Rows.Count, col];
                    columnRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    columnRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(
                        Math.Min(255, sectionColor.R + 20),
                        Math.Min(255, sectionColor.G + 20),
                        Math.Min(255, sectionColor.B + 20)
                    )); // Even lighter version for data rows

                    col++;
                }
                colorIndex++;
            }

            // Fill Data Rows
            int r = dataStartRow;
            foreach (var reportRow in reportData.Rows)
            {
                int c = 1;
                ws.Cells[r, c++].Value = reportRow.Date;
                ws.Cells[r, c++].Value = reportRow.Day;
                ws.Cells[r, c++].Value = reportRow.SessionCount;
                ws.Cells[r, c++].Value = reportRow.CustomerCount;
                ws.Cells[r, c++].Value = reportRow.NewPatientCount;
                ws.Cells[r, c++].Value = reportRow.OldPatientCount;

                foreach (var category in reportData.Categories)
                {
                    foreach (var service in category.ServiceNames)
                    {
                        var key = $"{category.CategoryName}|{service}";
                        if (reportRow.ServiceCounts.TryGetValue(key, out int count) && count > 0)
                        {
                            ws.Cells[r, c].Value = count;
                            
                            // Highlight cell in Green if value > 0 (Stronger Green than column bg)
                            ws.Cells[r, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[r, c].Style.Fill.BackgroundColor.SetColor(Color.Chartreuse);

                            // Special laser pulse highlight (Pink background)
                            if (service == "النبضات" && count > 5)
                            {
                                ws.Cells[r, c].Style.Fill.BackgroundColor.SetColor(Color.DeepPink);
                            }
                        }
                        c++;
                    }
                }
                r++;
            }

            // Borders for all cells
            using (var range = ws.Cells[1, 1, r - 1, col - 1])
            {
                var border = range.Style.Border;
                border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
                range.AutoFitColumns();
            }

            var byteArray = await p.GetAsByteArrayAsync();
            return Convert.ToBase64String(byteArray);
        }

        public async Task<string> ExportPatientsInquiryReportAsync(PatientsInquiryReportResponseDto reportData, string sheetName = "Patients Analysis")
        {
            using var p = new ExcelPackage();
            var ws = p.Workbook.Worksheets.Add(sheetName);
            ws.View.RightToLeft = true;
            ws.Cells.Style.Font.Name = "Arial";
            ws.Cells.Style.Font.Size = 11;

            // Headers
            string[] headers = { "التاريخ", "الاسم", "الرقم", "الكول سنتر", "التصنيف", "الحجز", "المنطقة", "تاريخ الحجز", "المتابعة" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cells[1, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(41, 82, 163)); // Dark Blue
                cell.Style.Font.Color.SetColor(Color.White);
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Data
            int row = 2;
            foreach (var item in reportData.Rows)
            {
                ws.Cells[row, 1].Value = item.Date;
                ws.Cells[row, 2].Value = item.Name;
                ws.Cells[row, 3].Value = item.Number;
                ws.Cells[row, 4].Value = item.CallCenter;
                ws.Cells[row, 5].Value = item.Classification;
                ws.Cells[row, 6].Value = item.Booking;
                ws.Cells[row, 7].Value = item.Area;
                ws.Cells[row, 8].Value = item.BookingDate;
                ws.Cells[row, 9].Value = item.FollowUp;

                // Color coding for status
                if (item.Classification == "new")
                {
                    ws.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                }
                if (item.Booking == "yes")
                {
                    ws.Cells[row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, 6].Style.Fill.BackgroundColor.SetColor(Color.LightCyan);
                }

                row++;
            }

            // Summary Table
            row += 2;
            int summaryStartCol = 7;
            int summaryEndCol = 9;

            var summaryData = new List<(string Label, int Value)>
            {
                ("اجمالي الاسكرينات", reportData.Summary.TotalEntries),
                ("New", reportData.Summary.NewCount),
                ("Old", reportData.Summary.OldCount),
                ("Yes", reportData.Summary.YesBookingCount),
                ("No", reportData.Summary.NoBookingCount),
                ("جت", reportData.Summary.CameCount),
                ("مجتش", reportData.Summary.DidNotComeCount),
                ("لسه معادها مجاش", reportData.Summary.NotYetCount)
            };

            foreach (var summary in summaryData)
            {
                ws.Cells[row, summaryStartCol].Value = summary.Value;
                ws.Cells[row, summaryStartCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[row, summaryStartCol].Style.Font.Bold = true;
                
                ws.Cells[row, summaryStartCol + 1, row, summaryEndCol].Merge = true;
                ws.Cells[row, summaryStartCol + 1].Value = summary.Label;
                ws.Cells[row, summaryStartCol + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[row, summaryStartCol + 1].Style.Font.Bold = true;
                
                // Styling
                ws.Cells[row, summaryStartCol, row, summaryEndCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[row, summaryStartCol, row, summaryEndCol].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(41, 82, 163));
                ws.Cells[row, summaryStartCol, row, summaryEndCol].Style.Font.Color.SetColor(Color.White);
                ws.Cells[row, summaryStartCol, row, summaryEndCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells[row, summaryStartCol, row, summaryEndCol].Style.Border.Bottom.Color.SetColor(Color.White);

                row++;
            }

            ws.Cells.AutoFitColumns();
            
            var byteArray = await p.GetAsByteArrayAsync();
            return Convert.ToBase64String(byteArray);
        }
    }
}
