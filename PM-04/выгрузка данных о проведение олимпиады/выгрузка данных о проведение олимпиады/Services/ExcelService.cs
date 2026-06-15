using ClosedXML.Excel;
using выгрузка_данных_о_проведение_олимпиады.ViewModels;
using System.IO;
using System.Linq;

namespace выгрузка_данных_о_проведение_олимпиады.Services;

public class ExcelService : IExcelService
{
    public byte[] GenerateTemplate()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Шаблон");

        string[] headers = {
        "Фамилия ученика", "Имя ученика", "Отчество ученика",
        "Цифра класса", "Фамилия учителя", "Имя учителя", "Отчество учителя",
        "Баллы"
    };
        for (int i = 0; i < headers.Length; i++)
            worksheet.Cell(1, i + 1).Value = headers[i];

        // ✅ Можно добавить пустую строку для удобства (необязательно)
        // worksheet.Row(2).IsEmpty = true; // не нужно, по умолчанию пусто

        worksheet.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public Task<List<ParticipationRow>> ReadRowsFromExcelAsync(Stream fileStream)
    {
        var rows = new List<ParticipationRow>();
        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheet(1); // берём первый лист
        var firstRowUsed = worksheet.FirstRowUsed();
        if (firstRowUsed == null)
            return Task.FromResult(rows);

        // Определяем последнюю строку с данными
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
        for (int i = firstRowUsed.RowNumber() + 1; i <= lastRow; i++)
        {
            var row = worksheet.Row(i);
            if (row.IsEmpty()) continue;

            var participationRow = new ParticipationRow
            {
                StudentLastName = row.Cell(1).GetString()?.Trim() ?? "",
                StudentFirstName = row.Cell(2).GetString()?.Trim() ?? "",
                StudentPatronymic = row.Cell(3).GetString()?.Trim() ?? "",
                StudentGrade = row.Cell(4).TryGetValue<int>(out int grade) ? grade : 0,
                TeacherLastName = row.Cell(5).GetString()?.Trim() ?? "",
                TeacherFirstName = row.Cell(6).GetString()?.Trim() ?? "",
                TeacherPatronymic = row.Cell(7).GetString()?.Trim() ?? "",
                Points = row.Cell(8).GetString()?.Trim() ?? ""   // баллы как строка
            };
            rows.Add(participationRow);
        }
        return Task.FromResult(rows);
    }
}