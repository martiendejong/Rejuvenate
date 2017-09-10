using Microsoft.AspNet.SignalR.Hubs;
using Rejuvenate.v2.SignalRChangePublishing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.v2.Internal
{
    public class SignalRHubListeners
    {
        public Dictionary<Type, object> Listeners = new Dictionary<Type, object>();

        public SignalRHubListener<HubType> GetListener<HubType>() where HubType : IHub
        {
            var type = typeof(HubType);
            if (!Listeners.ContainsKey(type))
            {
                Listeners.Add(type, new SignalRHubListener<HubType>());
            }
            var publisher = (SignalRHubListener<HubType>)Listeners[type];
            return publisher;
        }
    }
}
