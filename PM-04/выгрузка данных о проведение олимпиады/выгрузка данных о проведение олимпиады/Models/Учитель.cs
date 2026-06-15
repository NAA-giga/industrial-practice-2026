using System;
using System.Collections.Generic;

namespace выгрузка_данных_о_проведение_олимпиады.Models;

public partial class Учитель
{
    public int ID { get; set; }

    public string Фамилия { get; set; } = null!;

    public string Имя { get; set; } = null!;

    public string? Отчество { get; set; }

    public string Телефон { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Участие_в_олимпиаде> Участие_в_олимпиадеs { get; set; } = new List<Участие_в_олимпиаде>();
}
