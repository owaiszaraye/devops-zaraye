using System;
using System.Net;

namespace Zaraye.Core.Domain.Messages
{
    public partial class PushNotificationresponse
    {
        public bool IsSuccessStatusCode { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ResponseContent { get; set; }
    }
}
