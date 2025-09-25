using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
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
    /// AddTodoWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AddTodoWindow : Window
    {
        public bool isInitializing = true;
        public AddTodoWindow()
        {
            InitializeComponent();
            isInitializing = false;
        }

        //ORマッパー使わない場合
        //private void Register_Click(object sender, RoutedEventArgs e)
        //{
        //    using var connection = new SqliteConnection($"Data Source={TodoWindowViewModel.DbPath}");
        //    connection.Open();

        //    var command = connection.CreateCommand();
        //    command.CommandText = "INSERT INTO Todo (Task, DueDate, Status, Description) VALUES (@task, @duedate, @status, @desc)";
        //    command.Parameters.AddWithValue("@task", TaskNameTextBox.Text);
        //    command.Parameters.AddWithValue("@duedate", DueDatePicker.SelectedDate?.ToString("yyyy-MM-dd"));
        //    command.Parameters.AddWithValue("@status", "未完了");
        //    command.Parameters.AddWithValue("@desc", DetailTextBox.Text);
        //    command.ExecuteNonQuery();

        //    this.DialogResult = true;
        //    this.Close();
        //}

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            DateTime jst = DueDatePicker.SelectedDate.Value;
            var utcDueDate = TimeZoneInfo.ConvertTimeToUtc(jst, TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));

            var newItem = new TodoItem
            {
                Task = TaskNameTextBox.Text,
                DueDate = utcDueDate.ToString("yyyy-MM-ddTHH:mm:ssZ"), // 例: "2025-09-25T03:00:00Z",
                Status = "未完了",
                Description = DetailTextBox.Text
            };

            using (var db = new TodoContext())
            {
                db.Todo.Add(newItem); // 追加
                db.SaveChanges();   // DB反映
            }
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

        private void DueDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitializing)
            {
                DateTime selectedDate = DueDatePicker.SelectedDate.Value;
                if (selectedDate < DateTime.Now.Date)
                {
                    MessageBox.Show("過去日を選択しています", "警告", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
