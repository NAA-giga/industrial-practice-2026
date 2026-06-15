using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Windows;

namespace выгрузка_данных_о_проведение_олимпиады.Services
{
    public class DialogService : IDialogService
    {
        public void ShowMessage(string title, string message, bool isError = false)
        {
            MessageBox.Show(message, title,
                MessageBoxButton.OK,
                isError ? MessageBoxImage.Error : MessageBoxImage.Information);
        }

        public bool ShowConfirmation(string title, string message)
        {
            var result = MessageBox.Show(message, title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public string? ShowOpenFileDialog(string filter, string initialDir = "")
        {
            var dialog = new OpenFileDialog
            {
                Filter = filter,
                InitialDirectory = string.IsNullOrEmpty(initialDir)
                    ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    : initialDir,
                CheckFileExists = true,
                Multiselect = false
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public string? ShowSaveFileDialog(string filter, string defaultExt, string fileName)
        {
            var dialog = new SaveFileDialog
            {
                Filter = filter,
                DefaultExt = defaultExt,
                FileName = fileName,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }
    }
}
