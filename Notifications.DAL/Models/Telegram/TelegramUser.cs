namespace Notifications.DAL.Models.Telegram
{
    public class TelegramUser : BaseEntity
    {
        public long ChatId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
