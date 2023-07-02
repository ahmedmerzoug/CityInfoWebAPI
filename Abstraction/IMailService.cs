namespace CityInfoAPI.Abstraction
{
    public interface IMailService
    {
        void Send(string subject, string message);
    }
}
