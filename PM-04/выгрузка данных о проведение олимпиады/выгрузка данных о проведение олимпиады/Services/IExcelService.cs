using System.IO;
using выгрузка_данных_о_проведение_олимпиады.ViewModels;

namespace выгрузка_данных_о_проведение_олимпиады.Services;

public interface IExcelService
{
    /// <summary>Генерирует шаблон Excel с заголовками (без данных)</summary>
    byte[] GenerateTemplate();

    /// <summary>Читает данные из Excel-файла и возвращает список строк ParticipationRow</summary>
    Task<List<ParticipationRow>> ReadRowsFromExcelAsync(Stream fileStream);
}