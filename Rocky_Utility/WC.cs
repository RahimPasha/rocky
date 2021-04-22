using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky_Utility
{
    public static class WC
    {
        public static string ImagePath = @"\images\product\";
        public static string SessionCart = "ShoppingCartSession";
        public static string SessionInquiryId = "InquirySession";

        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";
        public static string EmailAdmin = "rpkhajei@gmail.com";
        public static string CategoryName = "Category";
        public static string ApplicationTypeName = "ApplicationType";
        public static string Success = "Success";
        public static string Error = "Error";
        
        public static string StatusPending = "Pending";
        public static string StatusApproved = "Approved";
        public static string StatusProcessing = "Processing";
        public static string StatusShipped = "Shipped";
        public static string StatusCanceled = "Canceled";
        public static string StatusRefunded = "Refunded";
    }
}
