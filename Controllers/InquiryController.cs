using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rocky.Controllers
{

    [Authorize(Roles = WC.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderRepository _inquiryHeaderRepository;
        private readonly IInquiryDetailRepository _inquiryDetailRepository;
        [BindProperty]
        public InquiryVM inquiryVM { get; set; }
        public InquiryController(IInquiryHeaderRepository inquiryHeaderRepository,
            IInquiryDetailRepository inquiryDetailRepository)
        {
            _inquiryDetailRepository = inquiryDetailRepository;
            _inquiryHeaderRepository = inquiryHeaderRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            inquiryVM = new InquiryVM()
            {
                InquiryHeader = _inquiryHeaderRepository.FirstOrDefault(inq => inq.Id == id),
                InquiryDetail = _inquiryDetailRepository.GetAll(inq => inq.InquiryHeaderId == id, includeProperties:"Product")
            };
            return View(inquiryVM);
        }

        [HttpPost]
        public IActionResult Delete()
        {

            _inquiryDetailRepository.RemoveRange(_inquiryDetailRepository.GetAll(
                inqdetail => inqdetail.InquiryHeaderId == inquiryVM.InquiryHeader.Id));
            _inquiryHeaderRepository.Remove(_inquiryHeaderRepository.FirstOrDefault(h => h.Id == inquiryVM.InquiryHeader.Id));
            _inquiryHeaderRepository.Save();

            
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Details()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            inquiryVM.InquiryDetail = _inquiryDetailRepository.GetAll(det => det.InquiryHeaderId == inquiryVM.InquiryHeader.Id);
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
            return Json(new { data = _inquiryHeaderRepository.GetAll() });
        }


        #endregion

    }
}
