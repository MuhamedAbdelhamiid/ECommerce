using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Entities.OrderModule;

namespace ECommerce.Services.Specifications
{
    public class OrderSpecifications : BaseSpecifications<Order, Guid>
    {
        public OrderSpecifications(string userEmail)
            : base(order => order.UserEmail == userEmail)
        {
            AddInclude(order => order.DeliveryMethod);
            AddInclude(order => order.Items);
            AddOrderByDescending(order => order.OrderDate);
        }

        public OrderSpecifications(Guid orderId, string userEmail)
            : base(order => order.Id == orderId && order.UserEmail == userEmail)
        {
            AddInclude(order => order.DeliveryMethod);
            AddInclude(order => order.Items);
        }

        public OrderSpecifications(string paymentIntentId, string userEmail = "")
            : base(order =>
                order.PaymentIntentId == paymentIntentId && order.UserEmail != ""
                    ? order.UserEmail == userEmail
                    : true
            ) { }
    }
}
