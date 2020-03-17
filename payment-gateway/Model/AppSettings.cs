using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace payment_gateway.Model
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string ConnectionString { get; set; }
        public PaypalSettings PaypalSettings { get; set; }
    }

    public class PaypalSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
