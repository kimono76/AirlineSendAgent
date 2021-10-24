using System.Threading.Tasks;
using AirlineSendAgent.Dtos;

namespace AirlineSendAgent.Client
{
    public interface IntWebhookClient
    {
            Task SendWebhookNotification(FlightDetailChangePayloadDto flightDetailChangePayloadDto);
    }
}