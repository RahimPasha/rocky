using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_Models.ViewModels
{
   public class OrderDetailVM
    {
        public OrderHeader orderHeader { get; set; }

        public IEnumerable<OrderDetail> OrderDetails { get; set; }


    }
}
