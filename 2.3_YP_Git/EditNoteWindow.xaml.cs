// создание и редактирвоание записей в БД
using System.Windows;
using Npgsql;

namespace ProjectManagementApp
{
    public partial class EditNoteWindow : Window
    {
        private readonly Note _note;
        
        public EditNoteWindow()
        {
            InitializeComponent();
            Title = "Создание новой записи";
        }
        
        public EditNoteWindow(Note note) : this()
        {
            _note = note;
            TitleTextBox.Text = note.Title;
            ContentTextBox.Text = note.Content;
            Title = "Редактирование записи";
        }
        
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Название не может быть пустым!", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            try
            {
                if (_note == null)
                {
                    // Создание новой записи
                    string query = @"
                        INSERT INTO ""Notes"" (user_id, title, content)
                        VALUES (@user_id, @title, @content)";
                    
                    using (var conn = new NpgsqlConnection(App.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("user_id", AppContext.CurrentUserId);
                            cmd.Parameters.AddWithValue("title", TitleTextBox.Text);
                            cmd.Parameters.AddWithValue("content", ContentTextBox.Text);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    // Обновление существующей записи
                    string query = @"
                        UPDATE ""Notes""
                        SET title = @title, content = @content
                        WHERE note_id = @note_id";
                    
                    using (var conn = new NpgsqlConnection(App.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("title", TitleTextBox.Text);
                            cmd.Parameters.AddWithValue("content", ContentTextBox.Text);
                            cmd.Parameters.AddWithValue("note_id", _note.NoteId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении записи: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}