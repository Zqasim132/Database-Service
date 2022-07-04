using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseService
{
    public class EventCounterListener : EventListener
    {
        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            // Only enable events from SqlClientEventSource.
            if (eventSource.Name.Equals("Microsoft.Data.SqlClient.EventSource"))
            {
                var options = new Dictionary<string, string>();
                // define time interval 1 second
                // without defining this parameter event counters will not enabled
                options.Add("EventCounterIntervalSec", "1");
                // enable for the None keyword
                EnableEvents(eventSource, EventLevel.Informational, EventKeywords.None, options);
            }
        }

        // This callback runs whenever an event is written by SqlClientEventSource.
        // Event data is accessed through the EventWrittenEventArgs parameter.
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData.Payload.FirstOrDefault(p => p is IDictionary<string, object> x && x.ContainsKey("Name")) is IDictionary<string, object> counters)
            {
                if (counters.TryGetValue("DisplayName", out object name) && name is string cntName
                    && counters.TryGetValue("Mean", out object value) && value is double cntValue)
                {
                    // print event counter's name and mean value
                    if (cntName == "Actual active connections currently made to servers" 
                        || cntName == "Active connections retrieved from the connection pool" 
                        || cntName == "Number of connections managed by the connection pool"
                        ||cntName == "Number of active connections")
                        //cntValue ++;
                    Console.WriteLine($"{cntName}\t\t{cntValue -1}");
                }
            }
        }
    }
}
