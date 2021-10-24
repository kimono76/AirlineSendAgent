using System.Runtime.Serialization.Json;
using System.Net.Http;
using System.Threading.Tasks;
using AirlineSendAgent.Dtos;
using System.Text.Json;
using System.Net.Http.Headers;
using System;

namespace AirlineSendAgent.Client
{
    public class WebHookClient : IntWebhookClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public WebHookClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        } 
        public async Task SendWebhookNotification(FlightDetailChangePayloadDto flightDetailChangePayloadDto)
        {
            var serializedPayload = JsonSerializer.Serialize(flightDetailChangePayloadDto);
            var httpClient = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post,flightDetailChangePayloadDto.WebhookURI);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(serializedPayload);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            try
            {
                using (var response = await httpClient.SendAsync(request)){
                    Console.WriteLine("Success");
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"unsucceful request: {ex.Message}");
            }
        }
    }
}