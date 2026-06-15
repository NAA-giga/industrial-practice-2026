using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace выгрузка_данных_о_проведение_олимпиады.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchComboBox_Loaded(object sender, RoutedEventArgs e)
            => MoveCaretToEnd(sender as ComboBox);

        private void SearchComboBox_GotFocus(object sender, RoutedEventArgs e)
            => MoveCaretToEnd(sender as ComboBox);

        private void SearchComboBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null) return;

            if (!comboBox.IsDropDownOpen)
            {
                comboBox.IsDropDownOpen = true;
                e.Handled = true;
            }
            MoveCaretToEnd(comboBox);
        }

        private void SearchComboBox_DropDownClosed(object sender, EventArgs e)
            => MoveCaretToEnd(sender as ComboBox);

        private void SearchComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                MoveCaretToEnd(sender as ComboBox);
            }), DispatcherPriority.Background);
        }

        private void MoveCaretToEnd(ComboBox comboBox)
        {
            if (comboBox == null) return;
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                if (comboBox.Template?.FindName("PART_EditableTextBox", comboBox) is TextBox textBox)
                {
                    // Курсор в конец текста, выделение отсутствует
                    textBox.Select(textBox.Text.Length, 0);
                }
            }), DispatcherPriority.Background);
        }
    }
}