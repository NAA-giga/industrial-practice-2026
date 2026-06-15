using System;
using System.Collections.Generic;

namespace выгрузка_данных_о_проведение_олимпиады.Models;

public partial class Сведенье_о_обучающиеся
{
    public string ФИО { get; set; } = null!;

    public string Класс { get; set; } = null!;

    public string Учитель { get; set; } = null!;

    public int? Количество_участий_в_олимпиаде { get; set; }
}
