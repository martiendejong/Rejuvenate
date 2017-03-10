using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace poller
{
    public interface ISignalRClient
    {
        string ConnectionId { get; }
    }

    public class SignalRClient : ISignalRClient
    {
        public string ConnectionId { get; }

        public SignalRClient(string connectionId)
        {
            ConnectionId = connectionId;
        }
    }

    public class AwarenessClient : SignalRClient
    {
        public AwarenessClient(string connectionId) : base(connectionId)
        {
        }
    }

    public interface ISignalRClients
    {
        List<AwarenessClient> AwarenessClients { get; }
    }

    public class SignalRClients : ISignalRClients
    {
        public List<AwarenessClient> AwarenessClients { get; }

        public SignalRClients()
        {
            AwarenessClients = new List<AwarenessClient>();
        }
    }
}