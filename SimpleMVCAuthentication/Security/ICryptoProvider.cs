namespace SimpleMVCAuthentication.Security
{
    public interface ICryptoProvider
    {
        string Encrypt<T>(T Object);
        T Decrypt<T>(string Data);
    }
}
