using System;
using System.Collections.Generic;

namespace выгрузка_данных_о_проведение_олимпиады.Models;

public partial class Участие_в_олимпиаде
{
    public int ID { get; set; }

    public int Количество_баллов { get; set; }

    public string? Результат_участия { get; set; }

    public int Обучающиеся_ID { get; set; }

    public int Олимпиада_ID { get; set; }

    public int? Учитель_ID { get; set; }

    public virtual Обучающиеся Обучающиеся { get; set; } = null!;

    public virtual Олимпиада Олимпиада { get; set; } = null!;

    public virtual Учитель? Учитель { get; set; }
}
