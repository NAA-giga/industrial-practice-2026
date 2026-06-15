using System;
using System.Collections.Generic;

namespace выгрузка_данных_о_проведение_олимпиады.Models;

public partial class Олимпиада
{
    public int ID { get; set; }

    public string Предмет { get; set; } = null!;

    public int? Класс_за_который_выполняеться_задание { get; set; }

    public DateOnly Дата_проведения { get; set; }

    public virtual ICollection<Участие_в_олимпиаде> Участие_в_олимпиадеs { get; set; } = new List<Участие_в_олимпиаде>();
}
