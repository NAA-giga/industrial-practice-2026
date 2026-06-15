using Microsoft.EntityFrameworkCore;
using выгрузка_данных_о_проведение_олимпиады.Models;

namespace выгрузка_данных_о_проведение_олимпиады.Services;

/// <summary>
/// Реализация сервиса данных с использованием Entity Framework.
/// Все DbSet имеют русские имена, соответствующие контексту School_OlympiadDBContext.
/// </summary>
public class OlympiadDataService : IOlympiadDataService
{
    private readonly School_OlympiadDBContext _context;

    public OlympiadDataService(School_OlympiadDBContext context)
    {
        _context = context;
    }

    // ========== Учреждение ==========

    public async Task<Образовательное_учереждение?> GetInstitutionAsync()
    {
        try
        {
            return await _context.Образовательное_учереждениеs.FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Не удалось загрузить учреждение.", ex);
        }
    }

    public async Task<Образовательное_учереждение> GetOrCreateInstitutionAsync(string name, string address)
    {
        try
        {
            // Ищем учреждение с точным совпадением названия и адреса
            var institution = await _context.Образовательное_учереждениеs
                .FirstOrDefaultAsync(i => i.Наименование == name && i.Адрес == address);

            if (institution != null)
                return institution; // Нашли – используем существующее

            // Не найдено – создаём новое
            institution = new Образовательное_учереждение
            {
                Наименование = name,
                Адрес = address
            };
            _context.Образовательное_учереждениеs.Add(institution);
            await _context.SaveChangesAsync();
            return institution;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Не удалось сохранить учреждение.", ex);
        }
    }

    // ========== Олимпиада ==========

    public async Task<int> GetOrCreateOlympiadAsync(string subject, int grade)
    {
        try
        {
            var existing = await _context.Олимпиадаs
                .FirstOrDefaultAsync(o => o.Предмет == subject && o.Класс_за_который_выполняеться_задание == grade);
            if (existing != null)
                return existing.ID;

            var newOlympiad = new Олимпиада
            {
                Предмет = subject,
                Класс_за_который_выполняеться_задание = grade,
                Дата_проведения = DateOnly.FromDateTime(DateTime.Today)
            };
            _context.Олимпиадаs.Add(newOlympiad);
            await _context.SaveChangesAsync();
            return newOlympiad.ID;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Не удалось найти или создать олимпиаду.", ex);
        }
    }

    // ========== Сохранение участий (с обработкой учеников, учителей, классов) ==========

    public async Task SaveParticipationsWithNamesAsync(int olympiadId, List<ParticipationEntry> entries)
    {
        if (entries == null || entries.Count == 0)
            throw new ArgumentException("Нет данных для сохранения.");

        if (olympiadId <= 0)
            throw new ArgumentException("Не указана олимпиада.");

        // Получаем ID текущего учреждения (или создаём заглушку, если его нет)
        var institution = await GetInstitutionAsync();
        if (institution == null)
            throw new InvalidOperationException("Не найдено образовательное учреждение. Сохраните учреждение перед добавлением участий.");

        int institutionId = institution.ID;

        var participations = new List<Участие_в_олимпиаде>();

        foreach (var entry in entries)
        {
            var student = await FindOrCreateStudentAsync(
                entry.StudentLastName, entry.StudentFirstName, entry.StudentPatronymic,
                entry.StudentGrade, institutionId);

            int? teacherId = null;
            if (!string.IsNullOrWhiteSpace(entry.TeacherLastName) && !string.IsNullOrWhiteSpace(entry.TeacherFirstName))
            {
                var teacher = await FindOrCreateTeacherAsync(
                    entry.TeacherLastName, entry.TeacherFirstName, entry.TeacherPatronymic);
                teacherId = teacher.ID;
            }

            participations.Add(new Участие_в_олимпиаде
            {
                Олимпиада_ID = olympiadId,
                Обучающиеся_ID = student.ID,
                Учитель_ID = teacherId,
                Количество_баллов = entry.Points
            });
        }

        await _context.Участие_в_олимпиадеs.AddRangeAsync(participations);
        await _context.SaveChangesAsync();
    }

    // ========== Вспомогательные методы ==========

    private async Task<Обучающиеся> FindOrCreateStudentAsync(
        string lastName, string firstName, string? patronymic,
        int gradeNumber, int institutionId)
    {
        var existing = await _context.Обучающиесяs
            .Include(s => s.Класс)
            .FirstOrDefaultAsync(s =>
                s.Фамилия == lastName &&
                s.Имя == firstName &&
                (s.Отчеcтво == patronymic || (s.Отчеcтво == null && patronymic == null)));

        var classEntity = await GetOrCreateClassAsync(gradeNumber);

        if (existing != null)
        {
            if (existing.Класс_ID != classEntity.ID)
            {
                existing.Класс_ID = classEntity.ID;
                _context.Обучающиесяs.Update(existing);
                await _context.SaveChangesAsync();
            }
            return existing;
        }

        var newStudent = new Обучающиеся
        {
            Фамилия = lastName,
            Имя = firstName,
            Отчеcтво = patronymic,
            Класс_ID = classEntity.ID,
            Образовательное_учереждение_ID = institutionId
        };
        _context.Обучающиесяs.Add(newStudent);
        await _context.SaveChangesAsync();
        return newStudent;
    }

    private async Task<Учитель> FindOrCreateTeacherAsync(
        string lastName, string firstName, string? patronymic)
    {
        var existing = await _context.Учительs
            .FirstOrDefaultAsync(t =>
                t.Фамилия == lastName &&
                t.Имя == firstName &&
                (t.Отчество == patronymic || (t.Отчество == null && patronymic == null)));
        if (existing != null)
            return existing;

        var newTeacher = new Учитель
        {
            Фамилия = lastName,
            Имя = firstName,
            Отчество = patronymic,
            Телефон = "00000000000",
            Email = $"{lastName.ToLower()}.{firstName.ToLower()}@temp.school"
        };
        _context.Учительs.Add(newTeacher);
        await _context.SaveChangesAsync();
        return newTeacher;
    }

    private async Task<Класс> GetOrCreateClassAsync(int gradeNumber)
    {
        // Поиск по числовому полю Цифра_класса
        var existing = await _context.Классs.FirstOrDefaultAsync(c => c.Цифра_класса == gradeNumber);
        if (existing != null)
            return existing;

        var newClass = new Класс
        {
            Цифра_класса = gradeNumber
        };
        _context.Классs.Add(newClass);
        await _context.SaveChangesAsync();
        return newClass;
    }

    private (string lastName, string firstName, string? patronymic) ParseFullName(string fullName)
    {
        var parts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string lastName = parts.Length > 0 ? parts[0] : "";
        string firstName = parts.Length > 1 ? parts[1] : "";
        string? patronymic = parts.Length > 2 ? parts[2] : null;
        return (lastName, firstName, patronymic);
    }
    public async Task<List<Обучающиеся>> GetStudentsAsync()
    {
        try
        {
            return await _context.Обучающиесяs
                .Include(s => s.Класс)   // загружаем класс, чтобы получить Цифра_класса
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Не удалось загрузить список учеников.", ex);
        }
    }
}