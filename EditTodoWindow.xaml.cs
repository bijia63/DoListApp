using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DoListApp
{
    /// <summary>
    /// EditTodoWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditTodoWindow : Window
    {
        public int Id { get; set; }
        public EditTodoWindow()
        {
            InitializeComponent();
        }

        public EditTodoWindow(int id, string Task, string DueDate, string Description, string Status)
        {
            InitializeComponent();
            Id = id;
            TaskNameTextBox.Text = Task;
            DueDatePicker.SelectedDate = DateTime.TryParse(DueDate, out var d) ? d : null;
            DetailTextBox.Text = Description;
            StatusComboBox.SelectedIndex = Status == "完了" ? 1 : 0;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new TodoContext())
            {
                var updateTodo = db.Todo.Find(Id);
                if (updateTodo != null)
                {
                    updateTodo.Task = TaskNameTextBox.Text;
                    updateTodo.DueDate = DueDatePicker.SelectedDate?.ToString("yyyy-MM-dd");
                    updateTodo.Status = StatusComboBox.Text;
                    updateTodo.Description = DetailTextBox.Text;
                    db.SaveChanges();
                }
            };

            this.DialogResult = true;
            this.Close();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            TaskNameTextBox.Text = "";
            DueDatePicker.SelectedDate = null;
            DetailTextBox.Text = "";
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
