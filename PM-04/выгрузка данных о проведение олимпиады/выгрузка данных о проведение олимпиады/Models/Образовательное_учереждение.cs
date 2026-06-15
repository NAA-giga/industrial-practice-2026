using System;
using System.Collections.Generic;

namespace выгрузка_данных_о_проведение_олимпиады.Models;

public partial class Образовательное_учереждение
{
    public int ID { get; set; }

    public string Наименование { get; set; } = null!;

    public string Адрес { get; set; } = null!;

    public virtual ICollection<Обучающиеся> Обучающиесяs { get; set; } = new List<Обучающиеся>();
}
