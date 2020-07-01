
using Common.Commands.Events;
using Common.Commands.Events.PoC;
using Microsoft.Extensions.DependencyInjection;
using PoCEventHandler.Services;
using Serilog;

namespace PoCEventHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = Startup.ConfigureServices(new ServiceCollection(), args);
            var eventHandler = serviceProvider.GetService<EventHandler>();
            var watermarkService = serviceProvider.GetService<WatermarkService>();
            long currentPosition = watermarkService.GetCurrentWatermark();

            while (true)
            {
                var events = eventHandler.GetEvents(1+currentPosition);
                if (events.Count > 0)
                {
                    foreach (var eventStoreEvent in events)
                    {
                        Log.Logger.Debug($"Got Event {eventStoreEvent.Sequence} - {eventStoreEvent.Data}");
                        
                        switch (eventStoreEvent.Data.GetType().ToString())
                        {
                            case "Common.Commands.Events.Heartbeat":
                                var obj = eventStoreEvent.Data as Heartbeat;
                                Log.Logger.Debug($"{eventStoreEvent.Sequence} - Heartbeat - {obj?.Source} from {obj?.CreatedBy}");
                                break;
                            case "Common.Commands.Events.PoC.PocCharEvent":
                                var charEvent = eventStoreEvent.Data as PocCharEvent;
                                Log.Logger.Information($"{eventStoreEvent.Sequence} - Message - {charEvent?.Character}");
                                break;
                        }
                        currentPosition = eventStoreEvent.Sequence;
                        watermarkService.UpdateWatermark(currentPosition);
                    }
                }
            }
        }
    }
}