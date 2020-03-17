using System;
using System.Linq;
using payment_gateway_core.Model;
using payment_gateway_repository.Model;

namespace payment_gateway_core.Helper
{
    public static class PaymentsFilterHelper
    {
        public static IQueryable<UserPayment> GetPaymentsQueryable(this IQueryable<UserPayment> payments, PaymentsFilter filter)
        {
            if(payments == null)
                throw new ArgumentOutOfRangeException(nameof(payments), "There are no payments to filter.");

            var result = payments.Where(x => x.UserId == new Guid(filter.UserId));

            if(filter.StartDate != default && filter.EndDate != default)
                result = result.Where(d => d.Created >= filter.StartDate && d.Created <= filter.EndDate);
            if (!string.IsNullOrEmpty(filter.CardNumber))
                result = result.Where(c => c.CardDetails.CardNumber == filter.CardNumber);
            if(!string.IsNullOrEmpty(filter.ProductId))
                result = result.Where(p => p.ProductId == filter.ProductId);
            if (!string.IsNullOrEmpty(filter.ProductName))
                result = result.Where(p => p.ProductName == filter.ProductName);

            return result;
        }
    }
}
