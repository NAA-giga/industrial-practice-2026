using System;
using System.Collections.Generic;

namespace выгрузка_данных_о_проведение_олимпиады.Models;

public partial class Сведенье_о_учителе
{
    public string ФИО_учителя { get; set; } = null!;

    public int? Количество_участий_в_олимпиаде_от_учеников { get; set; }
}
