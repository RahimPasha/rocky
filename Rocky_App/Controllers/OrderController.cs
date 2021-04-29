using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Repository;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky_Models;
using Rocky_Utility.BrainTree;
using Braintree;
using Microsoft.AspNetCore.Authorization;

namespace rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IBrainTreeGate _brain;


        [BindProperty]
        public OrderDetailVM OrderDetailVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork, IBrainTreeGate brainTree)
        {
            _unitOfWork = unitOfWork;
            _brain = brainTree;
        }
        public IActionResult Index(string SearchName = null, string SearchEmail = null, string SearchPhone = null, string Status = null)
        {
            OrderVM orderVM = new OrderVM
            {
                orderHList = _unitOfWork.OrderHeaders.GetAll(),
                StatusList = WC.ListStatus.ToList().Select(i => new SelectListItem()
                {
                    Text = i,
                    Value = i
                })
            };

            if (!string.IsNullOrEmpty(SearchName))
            {
                orderVM.orderHList = orderVM.orderHList.Where(oh => oh.FullName.ToLower().Contains(SearchName.ToLower()));
            }
            if (!string.IsNullOrEmpty(SearchEmail))
            {
                orderVM.orderHList = orderVM.orderHList.Where(oh => oh.Email.ToLower().Contains(SearchEmail.ToLower()));
            }
            if (!string.IsNullOrEmpty(SearchPhone))
            {
                orderVM.orderHList = orderVM.orderHList.Where(oh => oh.PhoneNumber.ToLower().Contains(SearchPhone.ToLower()));
            }
            if (!string.IsNullOrEmpty(Status) && Status != "--Order Status--")
            {
                orderVM.orderHList = orderVM.orderHList.Where(oh => oh.OrderStatus.ToLower().Contains(Status.ToLower()));
            }

            return View(orderVM);
        }

        public IActionResult Details(int id)
        {
            OrderDetailVM = new OrderDetailVM()
            {
                orderHeader = _unitOfWork.OrderHeaders.FirstOrDefault(oh => oh.Id == id),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderHeaderId == id, includeProperties: "Product")
            };

            return View(OrderDetailVM);
        }
 
        [HttpPost]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeaders.FirstOrDefault(order => order.Id == OrderDetailVM.orderHeader.Id).OrderStatus = WC.StatusProcessing;
            _unitOfWork.Complete();
            TempData[WC.Success] = "Order In Process";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaders.FirstOrDefault(order => order.Id == OrderDetailVM.orderHeader.Id);
            orderHeader.OrderStatus = WC.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            _unitOfWork.Complete();
            TempData[WC.Success] = "Order Shipped Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult CancelOrder()
        {

            OrderHeader orderHeader = _unitOfWork.OrderHeaders.FirstOrDefault(order => order.Id == OrderDetailVM.orderHeader.Id);

            var gateway = _brain.GetGateWay();
            Transaction transaction = gateway.Transaction.Find(orderHeader.TransactionId);

            if(transaction.Status == TransactionStatus.AUTHORIZED || transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {
                //no refund
                Result<Transaction> resultVoid = gateway.Transaction.Void(orderHeader.TransactionId);
            }
            else
            {
                //refund
                Result<Transaction> resultRefund = gateway.Transaction.Refund(orderHeader.TransactionId);
            }


            orderHeader.OrderStatus = WC.StatusRefunded;
            _unitOfWork.Complete();
            TempData[WC.Success] = "Order Cancelled Successfully";

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult UpdateOrderDetails()
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaders.FirstOrDefault(order => order.Id == OrderDetailVM.orderHeader.Id);

            orderHeader.FullName = OrderDetailVM.orderHeader.FullName;
            orderHeader.PhoneNumber = OrderDetailVM.orderHeader.PhoneNumber;
            orderHeader.StreetAddress = OrderDetailVM.orderHeader.StreetAddress;
            orderHeader.City = OrderDetailVM.orderHeader.City;
            orderHeader.State = OrderDetailVM.orderHeader.State;
            orderHeader.PostalCode = OrderDetailVM.orderHeader.PostalCode;
            orderHeader.Email = OrderDetailVM.orderHeader.Email;

            _unitOfWork.Complete();
            TempData[WC.Success] = "Order Details Updated Successfully";

            return RedirectToAction("Details","Order", new { id = orderHeader.Id });
        }
    }
}
