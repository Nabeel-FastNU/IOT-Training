using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace SignalRChart.Models
{
    [HubName("chartHub")]
    public class ChartHub : Hub
    {
        private readonly ChartData _pointer;

        public ChartHub() : this(ChartData.Instance) { }

        public ChartHub(ChartData pointer)
        {
            _pointer = pointer;
        }

        public void initData()
        {
            _pointer.initData();
        }
    }
}