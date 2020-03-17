using System;
using System.Collections.Generic;
using System.Text;

namespace payment_gateway_core.Model
{
    public class PaymentsFilter
    {
        public string UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string CardNumber { get; set; }

    }
}
