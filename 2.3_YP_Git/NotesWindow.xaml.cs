// главное окно, отображение записей и пользователя, манипуляция записями 
using System.Windows;
using Npgsql;
using System.Collections.Generic;
using System;

namespace ProjectManagementApp
{
    public partial class NotesWindow : Window
    {
        public NotesWindow()
        {
            InitializeComponent();
            LoadNotes();
            
            // Устанавливаем заголовок окна с именем пользователя
            this.Title = $"Записная книжка - {AppContext.CurrentUserName}";
        }
        
        private void LoadNotes()
        {
            try
            {
                string query = @"
                    SELECT note_id, title, content, created_at, is_blue, is_yellow, is_red
                    FROM ""Notes""
                    WHERE user_id = @user_id
                    ORDER BY created_at DESC";
                
                using (var conn = new NpgsqlConnection(App.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("user_id", AppContext.CurrentUserId);
                        
                        var reader = cmd.ExecuteReader();
                        var notes = new List<Note>();
                        
                        while (reader.Read())
                        {
                            notes.Add(new Note
                            {
                                NoteId = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Content = reader.GetString(2),
                                CreatedAt = reader.GetDateTime(3),
                                IsBlue = reader.GetBoolean(4),
                                IsYellow = reader.GetBoolean(5),
                                IsRed = reader.GetBoolean(6)
                            });
                        }
                        
                        NotesListView.ItemsSource = notes;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке записей: {ex.Message}");
            }
        }
        
        private void CreateNote_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditNoteWindow();
            if (editWindow.ShowDialog() == true)
            {
                LoadNotes();
            }
        }
        
        private void EditNote_Click(object sender, RoutedEventArgs e)
        {
            if (NotesListView.SelectedItem is Note selectedNote)
            {
                var editWindow = new EditNoteWindow(selectedNote);
                if (editWindow.ShowDialog() == true)
                {
                    LoadNotes();
                }
            }
        }
        
        private void DeleteNote_Click(object sender, RoutedEventArgs e)
        {
            if (NotesListView.SelectedItem is Note selectedNote)
            {
                try
                {
                    string query = "DELETE FROM \"Notes\" WHERE note_id = @note_id";
                    
                    using (var conn = new NpgsqlConnection(App.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("note_id", selectedNote.NoteId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    
                    LoadNotes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении записи: {ex.Message}");
                }
            }
        }
        
        private void BlueFlag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.CheckBox checkBox && checkBox.DataContext is Note note)
            {
                UpdateFlag("is_blue", note);
            }
        }

        private void YellowFlag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.CheckBox checkBox && checkBox.DataContext is Note note)
            {
                UpdateFlag("is_yellow", note);
            }
        }

        private void RedFlag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.CheckBox checkBox && checkBox.DataContext is Note note)
            {
                UpdateFlag("is_red", note);
            }
        }
        
        private void UpdateFlag(string flagName, Note note)
        {
            try
            {
                string query = $@"
                    UPDATE ""Notes""
                    SET {flagName} = @flag_value
                    WHERE note_id = @note_id";
                
                using (var conn = new NpgsqlConnection(App.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        bool flagValue = false;
                        if (flagName == "is_blue") flagValue = note.IsBlue;
                        else if (flagName == "is_yellow") flagValue = note.IsYellow;
                        else if (flagName == "is_red") flagValue = note.IsRed;
                        
                        cmd.Parameters.AddWithValue("flag_value", flagValue);
                        cmd.Parameters.AddWithValue("note_id", note.NoteId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении флага: {ex.Message}");
                LoadNotes();
            }
        }
        
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            AppContext.CurrentUserId = 0;
            var loginWindow = new ProjectManagementApp.LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
    
    public class Note
    {
        public int NoteId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsBlue { get; set; }
        public bool IsYellow { get; set; }
        public bool IsRed { get; set; }
    }
}