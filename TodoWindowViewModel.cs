using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Runtime.CompilerServices;

namespace DoListApp
{
    internal class TodoWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TodoItem> AllItems { get; set; } = new();
        public ObservableCollection<TodoItem> PageItems { get; set; } = new();
        public const string DbPath = "todo.db";
        public bool isDueDateAscending = true;

        public event PropertyChangedEventHandler? PropertyChanged;

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged();
                    UpdatePagedItems();
                    UpdateButtonStates();
                    OnPropertyChanged(nameof(PageInfo));
                }
            }
        }

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (_pageSize != value)
                {
                    _pageSize = value;
                    OnPropertyChanged();
                    UpdatePageInfo();
                    UpdatePagedItems();
                    OnPropertyChanged(nameof(PageInfo));
                }
            }
        }

        private int _totalPages = 1;
        public int TotalPages
        {
            get => _totalPages;
            set
            {
                if (_totalPages != value)
                {
                    _totalPages = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PageInfo));
                }
            }
        }

        public string PageInfo => $"{CurrentPage} / {TotalPages}";

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TodoWindowViewModel()
        {
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
            AllItems.Clear();
            var items = GetAllTodoItemsFromDatabase();
            foreach (var item in items)
            {
                changeUTCtoJST(item);
                AllItems.Add(item);
            }
            CurrentPage = 1;
            UpdatePageInfo();
            UpdatePagedItems();
            UpdateButtonStates();
        }

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
            var tempList = GetAllTodoItemsFromDatabase();

            IEnumerable<TodoItem> filtered = tempList;
            switch (filter)
            {
                case "すべて":
                    filtered = tempList;
                    break;
                case "未完了":
                    filtered = tempList.Where(x => x.Status == "未完了");
                    break;
                case "完了":
                    filtered = tempList.Where(x => x.Status == "完了");
                    break;
                default:
                    MessageBox.Show("不明なフィルターです", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }

            AllItems.Clear();
            foreach (var item in filtered)
            {
                changeUTCtoJST(item);
                AllItems.Add(item);
            }
            CurrentPage = 1;
            UpdatePageInfo();
            UpdatePagedItems();
            UpdateButtonStates();
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
            LoadFromDatabase();
        }

        public TodoItem changeUTCtoJST(TodoItem item)
        {
            DateTime utc = DateTime.Parse(item.DueDate, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
            TimeZoneInfo jstZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            DateTime jst = TimeZoneInfo.ConvertTimeFromUtc(utc, jstZone);
            item.DueDate = jst.ToString("yyyy-MM-dd");
            return item;
        }

        public void SortDueDate()
        {
            var sorted = isDueDateAscending
                ? AllItems.OrderBy(x => x.DueDate).ToList()
                : AllItems.OrderByDescending(x => x.DueDate).ToList();

            AllItems.Clear();
            foreach (var item in sorted)
            {
                AllItems.Add(item);
            }
            isDueDateAscending = !isDueDateAscending;
            UpdatePagedItems();
        }

        private void UpdatePagedItems()
        {
            PageItems.Clear();
            var paged = AllItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
            foreach (var item in paged)
            {
                PageItems.Add(item);
            }
            OnPropertyChanged(nameof(PageItems));
            OnPropertyChanged(nameof(PageInfo));
        }

        public void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
            }
        }

        public void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
            }
        }

        private void UpdatePageInfo()
        {
            TotalPages = (int)Math.Ceiling((double)AllItems.Count / PageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;
            if (CurrentPage < 1) CurrentPage = 1;
            OnPropertyChanged(nameof(PageInfo));
        }

        public bool CanGoPrevious => CurrentPage > 1;
        public bool CanGoNext => CurrentPage < TotalPages;

        private void UpdateButtonStates()
        {
            OnPropertyChanged(nameof(CanGoPrevious));
            OnPropertyChanged(nameof(CanGoNext));
        }
    }
}
