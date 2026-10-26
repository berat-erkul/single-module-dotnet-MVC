using ClosedXML.Excel;
using FirmaPortal.Entities;

namespace FirmaPortal.Business;

public static class ExcelPreviewReader
{
    public static List<List<string>> Read(byte[] content, int maxRows = 20)
    {
        try
        {
            using var stream = new MemoryStream(content);
            using var workbook = new XLWorkbook(stream);
            var range = workbook.Worksheet(1).RangeUsed();
            if (range is null) return new List<List<string>>();

            return range.Rows()
                .Take(maxRows)
                .Select(row => row.Cells().Select(c => c.GetFormattedString()).ToList())
                .ToList();
        }
        catch (Exception ex) when (ex is not BusinessException)
        {
            throw new BusinessException("Dosya okunamadı; geçerli bir .xlsx dosyası yükleyin.");
        }
    }
}
