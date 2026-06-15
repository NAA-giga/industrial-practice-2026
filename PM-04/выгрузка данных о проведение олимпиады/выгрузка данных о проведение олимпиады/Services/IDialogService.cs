using System;
using System.Collections.Generic;
using System.Text;

namespace выгрузка_данных_о_проведение_олимпиады.Services
{
    public interface IDialogService
    {
        /// <summary>Показать информационное сообщение или ошибку</summary>
        void ShowMessage(string title, string message, bool isError = false);

        /// <summary>Показать диалог подтверждения (Да/Нет)</summary>
        bool ShowConfirmation(string title, string message);

        /// <summary>Открыть диалог выбора файла для открытия</summary>
        string? ShowOpenFileDialog(string filter, string initialDir = "");

        /// <summary>Открыть диалог сохранения файла</summary>
        string? ShowSaveFileDialog(string filter, string defaultExt, string fileName);
    }
}
