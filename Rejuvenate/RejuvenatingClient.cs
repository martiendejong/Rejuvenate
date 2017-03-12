using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Rejuvenate
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

    public interface IRejuvenatingClient : ISignalRClient
    {
        List<int> RejuvenatorIds { get; set; }
    }

    public class RejuvenatingClient : SignalRClient, IRejuvenatingClient
    {
        public List<int> RejuvenatorIds { get; set; }

        public RejuvenatingClient(string connectionId) : base(connectionId)
        {
            RejuvenatorIds = new List<int>();
        }
    }
}