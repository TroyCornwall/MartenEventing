using System.Threading.Tasks;
using RestSharp;

namespace PoCAPI.Services
{
    public class EventRaiser
    {

        // private readonly EventRaiserOptions _options;
        //TODO: Use options to set event raiser url
        // public EventRaiser(EventRaiserOptions options)
        // {
        //     
        // }
        
        public async Task<long> AddMessage(string message)
        {
            var client = new RestClient("https://localhost:44366/message");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            
            //TODO: have to wrap the message in quotes as im not passing real objects 
            request.AddParameter("application/json", $"\"{message}\"",  ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteAsync(request);

            var canParse= long.TryParse(response.Content, out var seqId);
            if (canParse)
                return seqId;
            return -1;
        }

        public async Task<long> AddHeartBeat()
        {
            var client = new RestClient("https://localhost:44366/heartbeat");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = await client.ExecuteAsync(request);

            var canParse= long.TryParse(response.Content, out var seqId);
            if (canParse)
                return seqId;
            return -1;
        }
    }
}