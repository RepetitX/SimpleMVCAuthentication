namespace SimpleMVCAuthentication.Settings
{
    public class AuthenticationSettings
    {
        public string CookieName { get; set; }
        public string SessionCookieName { get; set; }
        public int SessionTimeOut { get; set; }
        public int DaysToExpiration { get; set; }
        public string EncryptionKey { get; set; }
    }
}