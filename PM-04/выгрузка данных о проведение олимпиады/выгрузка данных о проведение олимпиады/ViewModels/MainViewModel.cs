using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using выгрузка_данных_о_проведение_олимпиады.Models;
using выгрузка_данных_о_проведение_олимпиады.Services;

namespace выгрузка_данных_о_проведение_олимпиады.ViewModels;

/// <summary>
/// Главная модель представления – управление данными об участии в олимпиаде.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IOlympiadDataService _dataService;
    private readonly IDialogService _dialogService;
    private readonly IExcelService _excelService;

    // Фиксированный список предметов
    [ObservableProperty]
    private ObservableCollection<string> _subjects = new()
    {
        "Математика", "Физика", "Химия", "Биология", "Информатика",
        "Русский язык", "Литература", "История", "Обществознание", "География",
        "Английский язык", "Немецкий язык", "Французский язык", "ОБЖ", "Физкультура"
    };

    // Классы 1–11 для олимпиады
    [ObservableProperty]
    private ObservableCollection<int> _grades = new();

    // Коллекция строк таблицы (ввод данных)
    [ObservableProperty]
    private ObservableCollection<ParticipationRow> _rows = new();

    // Выбранные значения
    [ObservableProperty]
    private string? _selectedSubject;

    [ObservableProperty]
    private int? _selectedGrade;

    [ObservableProperty]
    private ParticipationRow? _selectedRow;

    // Состояние
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _institutionName = string.Empty;

    [ObservableProperty]
    private string _institutionAddress = string.Empty;

    // Поиск по строкам таблицы
    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ParticipationRow> _filteredRows = new();

    [ObservableProperty]
    private ParticipationRow? _selectedSearchRow;

    //управление видимостью выпадающего списка для поиска
    [ObservableProperty]
    private bool _isDropDownOpen;

    // Команды
    public IAsyncRelayCommand LoadDataCommand { get; }
    public IRelayCommand AddRowCommand { get; }
    public IRelayCommand DeleteRowCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand ImportExcelCommand { get; }
    public IAsyncRelayCommand ExportTemplateCommand { get; }

    public MainViewModel(IOlympiadDataService dataService, IDialogService dialogService, IExcelService excelService)
    {
        _dataService = dataService;
        _dialogService = dialogService;
        _excelService = excelService;

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddRowCommand = new RelayCommand(AddRow);
        DeleteRowCommand = new RelayCommand(DeleteRow);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        ImportExcelCommand = new AsyncRelayCommand(ImportExcelAsync);
        ExportTemplateCommand = new AsyncRelayCommand(ExportTemplateAsync);

        for (int i = 1; i <= 11; i++)
            Grades.Add(i);

        LoadDataCommand.Execute(null);
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsBusy = true;
            // Здесь можно добавить загрузку справочников, если потребуется, 
            // но для текущей задачи – оставляем пустым.
            StatusMessage = string.Empty;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки: {ex.Message}";
            _dialogService.ShowMessage("Ошибка", ex.Message, true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void AddRow()
    {
        Rows.Add(new ParticipationRow());
    }

    private void DeleteRow()
    {
        if (SelectedRow != null)
            Rows.Remove(SelectedRow);
        else
            _dialogService.ShowMessage("Удаление", "Не выбрана строка для удаления", false);
    }

    private async Task SaveAsync()
    {
        // 1. Проверка выбора предмета и класса олимпиады
        if (string.IsNullOrWhiteSpace(SelectedSubject))
        {
            _dialogService.ShowMessage("Внимание", "Выберите предмет олимпиады", false);
            return;
        }
        if (SelectedGrade == null || SelectedGrade < 1 || SelectedGrade > 11)
        {
            _dialogService.ShowMessage("Внимание", "Выберите класс олимпиады", false);
            return;
        }

        // 2. Проверка названия учреждения
        if (string.IsNullOrWhiteSpace(InstitutionName))
        {
            _dialogService.ShowMessage("Внимание", "Введите название образовательного учреждения", false);
            return;
        }

        // 3. Сохраняем учреждение
        try
        {
            await _dataService.GetOrCreateInstitutionAsync(InstitutionName, InstitutionAddress);
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage("Ошибка", $"Не удалось сохранить учреждение: {ex.Message}", true);
            return;
        }

        // 4. Сохраняем олимпиаду (находим или создаём)
        int olympiadId;
        try
        {
            olympiadId = await _dataService.GetOrCreateOlympiadAsync(SelectedSubject, SelectedGrade.Value);
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage("Ошибка", $"Не удалось сохранить олимпиаду: {ex.Message}", true);
            return;
        }

        // 5. Проверяем, есть ли строки для сохранения
        if (Rows.Count == 0)
        {
            _dialogService.ShowMessage("Внимание", "Нет данных для сохранения", false);
            return;
        }

        // 6. Валидация строк и формирование списка DTO
        var invalidRows = new List<string>();
        var entries = new List<ParticipationEntry>();

        for (int i = 0; i < Rows.Count; i++)
        {
            var row = Rows[i];
            if (string.IsNullOrWhiteSpace(row.StudentLastName))
                invalidRows.Add($"Строка {i + 1}: не указана фамилия ученика");
            else if (string.IsNullOrWhiteSpace(row.StudentFirstName))
                invalidRows.Add($"Строка {i + 1}: не указано имя ученика");
            else if (row.StudentGrade < 1 || row.StudentGrade > 11)
                invalidRows.Add($"Строка {i + 1}: класс ученика должен быть от 1 до 11 (введено {row.StudentGrade})");
            else if (string.IsNullOrWhiteSpace(row.Points))
                invalidRows.Add($"Строка {i + 1}: не указаны баллы");
            else
            {
                if (!int.TryParse(row.Points, out int pointsValue))
                    invalidRows.Add($"Строка {i + 1}: баллы должны быть целым числом");
                else if (pointsValue < 0 || pointsValue > 100)
                    invalidRows.Add($"Строка {i + 1}: баллы должны быть от 0 до 100 (введено {pointsValue})");
                else
                {
                    entries.Add(new ParticipationEntry
                    {
                        StudentLastName = row.StudentLastName.Trim(),
                        StudentFirstName = row.StudentFirstName.Trim(),
                        StudentPatronymic = string.IsNullOrWhiteSpace(row.StudentPatronymic) ? null : row.StudentPatronymic.Trim(),
                        StudentGrade = row.StudentGrade,
                        TeacherLastName = string.IsNullOrWhiteSpace(row.TeacherLastName) ? null : row.TeacherLastName.Trim(),
                        TeacherFirstName = string.IsNullOrWhiteSpace(row.TeacherFirstName) ? null : row.TeacherFirstName.Trim(),
                        TeacherPatronymic = string.IsNullOrWhiteSpace(row.TeacherPatronymic) ? null : row.TeacherPatronymic.Trim(),
                        Points = pointsValue
                    });
                }
            }
        }

        if (invalidRows.Any())
        {
            _dialogService.ShowMessage("Ошибки в данных", string.Join("\n", invalidRows), true);
            return;
        }

        // 7. Сохраняем участия через сервис
        try
        {
            IsBusy = true;
            await _dataService.SaveParticipationsWithNamesAsync(olympiadId, entries);
            _dialogService.ShowMessage("Успех", $"Сохранено {entries.Count} участий", false);
            Rows.Clear();
            StatusMessage = string.Empty;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка сохранения: {ex.Message}";
            _dialogService.ShowMessage("Ошибка", ex.Message, true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ImportExcelAsync()
    {
        var filePath = _dialogService.ShowOpenFileDialog("Excel files (*.xlsx)|*.xlsx", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        if (string.IsNullOrEmpty(filePath))
            return;

        try
        {
            IsBusy = true;
            using var stream = File.OpenRead(filePath);
            var importedRows = await _excelService.ReadRowsFromExcelAsync(stream);
            if (importedRows != null && importedRows.Any())
            {
                foreach (var row in importedRows)
                    Rows.Add(row);
                _dialogService.ShowMessage("Импорт", $"Импортировано {importedRows.Count} строк", false);
            }
            else
            {
                _dialogService.ShowMessage("Импорт", "Файл не содержит данных или формат неверен", false);
            }
        }
        catch (IOException)
        {
            _dialogService.ShowMessage("Ошибка", "Не удалось открыть файл. Скорее всего, он уже открыт в другой программе (например, в Microsoft Excel).\n\nЗакройте файл и повторите попытку.", true);
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage("Ошибка импорта", $"Не удалось прочитать файл: {ex.Message}", true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ExportTemplateAsync()
    {
        var filePath = _dialogService.ShowSaveFileDialog("Excel files (*.xlsx)|*.xlsx", "xlsx", "OlympiadTemplate");
        if (string.IsNullOrEmpty(filePath))
            return;

        try
        {
            var templateBytes = _excelService.GenerateTemplate();
            await File.WriteAllBytesAsync(filePath, templateBytes);
            _dialogService.ShowMessage("Шаблон", $"Шаблон сохранён: {filePath}", false);
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage("Ошибка экспорта", ex.Message, true);
        }
    }

    // Поиск по строкам таблицы
    partial void OnSearchTextChanged(string value)
    {
        SelectedSearchRow = null; // сбрасываем выбранный элемент при изменении текста пользователем
        if (string.IsNullOrWhiteSpace(value))
        {
            FilteredRows.Clear();
            IsDropDownOpen = false;
            return;
        }
        var filtered = Rows.Where(row =>
            (row.StudentLastName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true) ||
            (row.StudentFirstName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true) ||
            (row.StudentPatronymic?.Contains(value, StringComparison.OrdinalIgnoreCase) == true) ||
            (row.StudentLastName + " " + row.StudentFirstName + " " + row.StudentPatronymic).Contains(value, StringComparison.OrdinalIgnoreCase)
        ).Take(20).ToList();

        FilteredRows.Clear();
        foreach (var row in filtered)
            FilteredRows.Add(row);

        IsDropDownOpen = FilteredRows.Any();
    }

    // При выборе строки в выпадающем списке – выделяем её в таблице
    partial void OnSelectedSearchRowChanged(ParticipationRow? value)
    {
        if (value != null)
        {
            SelectedRow = value;
            SearchText = value.StudentLastName;   // устанавливаем фамилию в поле поиска
            FilteredRows.Clear();
            IsDropDownOpen = false;
            // Через небольшую задержку сбрасываем SelectedSearchRow, чтобы можно было снова выбрать
            Task.Run(async () =>
            {
                await Task.Delay(100);
                Application.Current.Dispatcher.Invoke(() => SelectedSearchRow = null);
            });
        }
    }
}