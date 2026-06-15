using выгрузка_данных_о_проведение_олимпиады.Models;

namespace выгрузка_данных_о_проведение_олимпиады.Services;

/// <summary>
/// Сервис для работы с данными об олимпиадах, учениках, учителях и участиях.
/// </summary>
public interface IOlympiadDataService
{
    Task<Образовательное_учереждение?> GetInstitutionAsync();
    Task<Образовательное_учереждение> GetOrCreateInstitutionAsync(string name, string address);
    Task<int> GetOrCreateOlympiadAsync(string subject, int grade);
    Task<List<Обучающиеся>> GetStudentsAsync();
    Task SaveParticipationsWithNamesAsync(int olympiadId, List<ParticipationEntry> entries);
}