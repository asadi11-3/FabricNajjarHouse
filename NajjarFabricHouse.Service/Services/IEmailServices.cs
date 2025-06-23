using NajjarFabricHouse.Service.Models;


namespace NajjarFabricHouse.Service.Services
{
    public interface IEmailServices
    {
        void SendEmail(Message message);
    }
}
