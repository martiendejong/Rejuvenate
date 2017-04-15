using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Rejuvenate.Db.SignalR
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

    public interface ISignalRSubscriber : ISignalRClient
    {
        List<int> PublisherIds { get; set; }
    }

    public class SignalRSubscriber : SignalRClient, ISignalRSubscriber
    {
        public List<int> PublisherIds { get; set; }

        public SignalRSubscriber(string connectionId) : base(connectionId)
        {
            PublisherIds = new List<int>();
        }
    }
}