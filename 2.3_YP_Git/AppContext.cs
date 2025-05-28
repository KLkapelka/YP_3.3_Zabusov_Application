// хранения ID и имени текущего пользователя
namespace ProjectManagementApp
{
    public static class AppContext
    {
        public static int CurrentUserId { get; set; }
        public static string CurrentUserName { get; set; }
    }
}