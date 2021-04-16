using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Repository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using System.Collections.Generic;


namespace rocky.Controllers
{

    [Authorize(Roles = WC.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public InquiryVM inquiryVM { get; set; }
        public InquiryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            inquiryVM = new InquiryVM()
            {
                InquiryHeader = _unitOfWork.InquiryHeaders.FirstOrDefault(inq => inq.Id == id),
                InquiryDetail = _unitOfWork.InquiryDetails.GetAll(inq => inq.InquiryHeaderId == id, includeProperties:"Product")
            };
            return View(inquiryVM);
        }

        [HttpPost]
        public IActionResult Delete()
        {

            _unitOfWork.InquiryDetails.RemoveRange(_unitOfWork.InquiryDetails.GetAll(
                inqdetail => inqdetail.InquiryHeaderId == inquiryVM.InquiryHeader.Id));
            _unitOfWork.InquiryHeaders.Remove(_unitOfWork.InquiryHeaders.FirstOrDefault(h => h.Id == inquiryVM.InquiryHeader.Id));
            _unitOfWork.Complete();

            
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Details()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            inquiryVM.InquiryDetail = _unitOfWork.InquiryDetails.GetAll(det => det.InquiryHeaderId == inquiryVM.InquiryHeader.Id);
            foreach(var detail in inquiryVM.InquiryDetail)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ProductId = detail.ProductId
                };
                shoppingCartList.Add(shoppingCart);
            }
            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            HttpContext.Session.Set(WC.SessionInquiryId, inquiryVM.InquiryHeader.Id);
            return RedirectToAction("Index", "Cart");
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = _unitOfWork.InquiryHeaders.GetAll() });
        }


        #endregion

    }
}
