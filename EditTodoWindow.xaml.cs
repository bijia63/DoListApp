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
        public bool isInitializing = true;

        public EditTodoWindow()
        {
            InitializeComponent();
        }

        public EditTodoWindow(int id)
        {
            InitializeComponent();
            ShowList(id);
            isInitializing = false;
        }

        public void ShowList(int id)
        {

            using (var db = new TodoContext())
            {
                var todo = db.Todo.Find(id);

                Id = id;
                TaskNameTextBox.Text = todo.Task;
                DueDatePicker.SelectedDate = changeUTCtoJST(todo.DueDate);
                DetailTextBox.Text = todo.Description;
                StatusComboBox.SelectedIndex = todo.Status == "完了" ? 1 : 0;
                //CompleteDate.Text = changeUTCtoJST(todo.CompleteDate).ToString("yyyy-MM-ddTHH:mm:ssZ");
                var jstDate = changeUTCtoJST(todo.CompleteDate);
                CompleteDate.Text = jstDate.HasValue ? jstDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {

            using (var db = new TodoContext())
            {
                var updateTodo = db.Todo.Find(Id);
                if (updateTodo != null)
                {
                    updateTodo.Task = TaskNameTextBox.Text;

                    DateTime jstDueDate = DueDatePicker.SelectedDate.Value;
                    var utcDueDate = TimeZoneInfo.ConvertTimeToUtc(jstDueDate, TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));
                    updateTodo.DueDate = utcDueDate.ToString("yyyy-MM-ddTHH:mm:ssZ"); // 例: "2025-09-25T03:00:00Z",;
                    updateTodo.Status = StatusComboBox.Text;
                    updateTodo.Description = DetailTextBox.Text;

                    if (!string.IsNullOrEmpty(CompleteDate.Text))
                    {
                        // JSTとしてパース
                        DateTime jstCompleteDate = DateTime.ParseExact(
                            CompleteDate.Text,
                            "yyyy-MM-dd HH:mm:ss",
                            System.Globalization.CultureInfo.InvariantCulture);

                        // JST→UTC変換
                        var utcCompleteDate = TimeZoneInfo.ConvertTimeToUtc(
                            jstCompleteDate,
                            TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));

                        updateTodo.CompleteDate = utcCompleteDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    }
                    else
                    {
                        updateTodo.CompleteDate = null;
                    }
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

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(StatusComboBox.SelectedItem is ComboBoxItem item && item.Content?.ToString() == "完了" )
            {
                if (string.IsNullOrEmpty(CompleteDate.Text))
                {
                    CompleteDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            else
            {
                CompleteDate.Text = null;
            }
        }

        private DateTime? changeUTCtoJST(string utcString)
        {
            if (string.IsNullOrEmpty(utcString)) return null;
            DateTime utc = DateTime.Parse(utcString, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
            TimeZoneInfo jstZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            DateTime jst = TimeZoneInfo.ConvertTimeFromUtc(utc, jstZone);
            return jst;
        }

        private void DueDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitializing) { 
                DateTime selectedDate = DueDatePicker.SelectedDate.Value;
                if (selectedDate < DateTime.Now.Date){
                    MessageBox.Show("過去日を選択しています","警告",MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }

}
