using System.Collections.Generic;

namespace Notifications.DTO.Configurations
{
    public class AuthResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
    }
    public class RegistrationResponse : AuthResult
    {

    }
}