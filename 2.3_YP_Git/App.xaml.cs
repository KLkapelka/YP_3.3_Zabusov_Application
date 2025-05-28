// конфигурация приложения, инициализация БД
using System.Windows;
using Npgsql;

namespace ProjectManagementApp
{
    public partial class App : Application
    {
        public static string ConnectionString { get; } = 
            "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=QWEasd123!@#123;Include Error Detail=true";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Open();
            
                    // таблица Users
                    string createUsersTableQuery = @"
                CREATE TABLE IF NOT EXISTS ""Users"" (
                    user_id SERIAL PRIMARY KEY,
                    username VARCHAR(255) NOT NULL UNIQUE,
                    password VARCHAR(255) NOT NULL
                )";
            
                    using (var cmd = new NpgsqlCommand(createUsersTableQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // таблица Notes
                    string createNotesTableQuery = @"
                CREATE TABLE IF NOT EXISTS ""Notes"" (
                    note_id SERIAL PRIMARY KEY,
                    user_id INT NOT NULL,
                    title VARCHAR(255) NOT NULL,
                    content TEXT NOT NULL,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    is_blue BOOLEAN DEFAULT false,
                    is_yellow BOOLEAN DEFAULT false,
                    is_red BOOLEAN DEFAULT false,
                    FOREIGN KEY (user_id) REFERENCES ""Users"" (user_id)
                )";
            
                    using (var cmd = new NpgsqlCommand(createNotesTableQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации базы данных: {ex.Message}");
            }
        }
    }
}