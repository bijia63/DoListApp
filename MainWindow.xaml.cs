using MahApps.Metro.Controls;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DoListApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private TodoWindowViewModel TodoVM;
        public MainWindow()
        {
            InitializeComponent();
            TodoVM = new TodoWindowViewModel();
            this.DataContext = TodoVM;

        }

        private void AddTodo_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddTodoWindow();
            if (addWindow.ShowDialog() == true)
            {
                TodoVM.LoadFromDatabase();
                MessageBox.Show("登録しました", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditTodo_Click(object sender, RoutedEventArgs e)
        {
            if (TodoDataGrid.SelectedItem is TodoItem item)
            {
                var editWindow = new EditTodoWindow(item.Id);

                if (editWindow.ShowDialog() == true)
                {
                    TodoVM.LoadFromDatabase();
                    MessageBox.Show("更新しました", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

        }
        private void DeleteTodo_Click(object sender, RoutedEventArgs e)
        {
            if (TodoDataGrid.SelectedItem is TodoItem item)
            {
                TodoVM.Delete_Click(item.Id);

                TodoVM.LoadFromDatabase();
                MessageBox.Show("削除しました", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void SortDueDate_Click(object sender, RoutedEventArgs e)
        {
            if (TodoVM == null) return;
            TodoVM.SortDueDate();
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            TodoVM.PreviousPage();
        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            TodoVM.NextPage();
        }
        private void FilterRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (TodoVM == null) return;
            var radio = (RadioButton)sender;
            String selectedRadio = radio.Content.ToString();
            TodoVM.FilterRadioButtonSort(selectedRadio);
        }
    }
}