// регистрация, проверка уникальности и сохранение в БД
using System.Windows;
using Npgsql;

namespace ProjectManagementApp
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Имя пользователя и пароль не могут быть пустыми.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var conn = new NpgsqlConnection(App.ConnectionString))
                {
                    conn.Open();

                    // Проверка существования пользователя
                    string checkUserQuery = "SELECT COUNT(*) FROM \"Users\" WHERE username = @username";
                    using (var cmd = new NpgsqlCommand(checkUserQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("username", username);
                        int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                        if (userCount > 0)
                        {
                            MessageBox.Show("Пользователь с таким именем уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }

                    // Регистрация нового пользователя
                    string insertUserQuery = "INSERT INTO \"Users\" (username, password) VALUES (@username, @password) RETURNING user_id";
                    using (var cmd = new NpgsqlCommand(insertUserQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("username", username);
                        cmd.Parameters.AddWithValue("password", password);
                        
                        // Получаем ID нового пользователя
                        int newUserId = Convert.ToInt32(cmd.ExecuteScalar());
                        
                        // Устанавливаем текущего пользователя
                        AppContext.CurrentUserId = newUserId;
                        AppContext.CurrentUserName = username;
                    }

                    MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Открываем окно записей
                    var notesWindow = new NotesWindow();
                    notesWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}