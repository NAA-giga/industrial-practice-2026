namespace выгрузка_данных_о_проведение_олимпиады.Models;

public class ParticipationEntry
{
    // Ученик
    public string StudentLastName { get; set; } = string.Empty;
    public string StudentFirstName { get; set; } = string.Empty;
    public string? StudentPatronymic { get; set; }
    public int StudentGrade { get; set; }      // цифра класса (1–11)

    // Учитель (опционально)
    public string? TeacherLastName { get; set; }
    public string? TeacherFirstName { get; set; }
    public string? TeacherPatronymic { get; set; }

    public int Points { get; set; }
}