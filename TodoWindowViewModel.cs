using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.Windows;

namespace DoListApp
{
    internal class TodoWindowViewModel
    {
        public ObservableCollection<TodoItem> TodoListView { get; set; }
        public const string DbPath = "todo.db";
        public TodoWindowViewModel()
        {

            TodoListView = new ObservableCollection<TodoItem>();
            InitializeDatabase();
            LoadFromDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection($"Data Source={DbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"CREATE TABLE IF NOT EXISTS Todo(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Task TEXT NOT NULL,
                    DueDate TEXT NOT NULL,
                    Status TEXT,
                    Description TEXT
                );";
            command.ExecuteNonQuery();
        }

        public void LoadFromDatabase()
        {
            TodoListView.Clear();
            var items = GetAllTodoItemsFromDatabase();
            foreach (var item in items)
                TodoListView.Add(item);
        }
        // ORマッパー使わない場合
        //private List<TodoItem> GetAllTodoItemsFromDatabase()
        //{
        //    var items = new List<TodoItem>();
        //    using var connection = new SqliteConnection($"Data Source={DbPath}");
        //    connection.Open();

        //    var command = connection.CreateCommand();
        //    command.CommandText = @"SELECT Id, Task, DueDate, Status, Description FROM Todo;";
        //    using var reader = command.ExecuteReader();
        //    while (reader.Read())
        //    {
        //        items.Add(new TodoItem
        //        {
        //            Id = reader.GetInt32(0),
        //            Task = reader.GetString(1),
        //            DueDate = reader.IsDBNull(2) ? null : reader.GetString(2),
        //            Status = reader.IsDBNull(3) ? "未完了" : reader.GetString(3),
        //            Description = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
        //        });
        //    }
        //    return items;
        //}

        private List<TodoItem> GetAllTodoItemsFromDatabase()
        {
            var itemLists = new List<TodoItem>();
            using (var db = new TodoContext())
            {
                itemLists = db.Todo.ToList();
            }
            return itemLists;
        }

        public void FilterRadioButtonSort(string filter)
        {
            var TempListView = GetAllTodoItemsFromDatabase();

            IEnumerable<TodoItem> filterdItem = TempListView;
            switch (filter)
            {
                case "すべて":
                    filterdItem = TempListView;
                    break;
                case "未完了":
                    filterdItem = TempListView.Where(x => x.Status == "未完了");
                    break;
                case "完了":
                    filterdItem = TempListView.Where(x => x.Status == "完了");
                    break;
                default:
                    MessageBox.Show("不明なフィルターです", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }
            TodoListView.Clear();
            foreach (var item in filterdItem)
                TodoListView.Add(item);
        }

        public void Delete_Click(int id)
        {
            using (var db = new TodoContext())
            {
                var todo = db.Todo.Find(id);
                if (todo != null)
                {
                    db.Todo.Remove(todo);
                    db.SaveChanges();
                }
            }
        }
    }
}
