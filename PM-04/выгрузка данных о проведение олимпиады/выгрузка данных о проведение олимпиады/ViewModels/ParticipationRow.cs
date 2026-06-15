using CommunityToolkit.Mvvm.ComponentModel;

namespace выгрузка_данных_о_проведение_олимпиады.ViewModels;

public partial class ParticipationRow : ObservableValidator
{
    [ObservableProperty]
    private string _studentLastName = string.Empty;

    [ObservableProperty]
    private string _studentFirstName = string.Empty;

    [ObservableProperty]
    private string? _studentPatronymic = string.Empty;

    [ObservableProperty]
    private int _studentGrade = 1;  // для класса оставляем int, но можно проверять при сохранении

    [ObservableProperty]
    private string _teacherLastName = string.Empty;

    [ObservableProperty]
    private string _teacherFirstName = string.Empty;

    [ObservableProperty]
    private string? _teacherPatronymic = string.Empty;

    // Баллы – строка, чтобы разрешить ввод любых символов
    [ObservableProperty]
    private string _points = string.Empty;
}