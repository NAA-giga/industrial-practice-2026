using System;
using System.Collections.Generic;

namespace выгрузка_данных_о_проведение_олимпиады.Models;

public partial class Класс
{
    public int ID { get; set; }

    public int Цифра_класса { get; set; }

    public virtual ICollection<Обучающиеся> Обучающиесяs { get; set; } = new List<Обучающиеся>();
}
