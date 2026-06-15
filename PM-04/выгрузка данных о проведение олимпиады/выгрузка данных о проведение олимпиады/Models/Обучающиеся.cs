using System;
using System.Collections.Generic;

namespace выгрузка_данных_о_проведение_олимпиады.Models;

public partial class Обучающиеся
{
    public int ID { get; set; }

    public string Фамилия { get; set; } = null!;

    public string Имя { get; set; } = null!;

    public string? Отчеcтво { get; set; }

    public int Образовательное_учереждение_ID { get; set; }

    public int Класс_ID { get; set; }

    public virtual Класс Класс { get; set; } = null!;

    public virtual Образовательное_учереждение Образовательное_учереждение { get; set; } = null!;

    public virtual ICollection<Участие_в_олимпиаде> Участие_в_олимпиадеs { get; set; } = new List<Участие_в_олимпиаде>();
}
