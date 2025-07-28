using System;

namespace WebApplication1.Models
{

    public class CustomerLogDto
    {
        public string UserName { get; set; }
        public string Company { get; set; }
        public string IpAddress { get; set; }
        public string PageVisited { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
