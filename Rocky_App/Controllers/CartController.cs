using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Repository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace rocky.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;


        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }
        public CartController(IWebHostEnvironment webHostEnvironment, 
            IEmailSender emailSender, IUnitOfWork unitOfWork)
        {

            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            if(HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart)!= null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCarts = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            List<int> prodInCart = shoppingCarts.Select(p => p.ProductId).ToList();
            IEnumerable<Product> products = _unitOfWork.Products.GetAll(p => prodInCart.Contains(p.Id));

            foreach(var prod in products)
            {
                prod.TempSqft = shoppingCarts.FirstOrDefault(s => s.ProductId == prod.Id) != null ?
                    shoppingCarts.FirstOrDefault(s => s.ProductId == prod.Id).Sqft : prod.TempSqft;
            }

            return View(products);
        }

        public IActionResult RemoveItem(int id)
        {
            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCarts = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
                var item = shoppingCarts.FirstOrDefault(item => item.ProductId == id);
                if (item != null)
                {
                    shoppingCarts.Remove(item);
                }
                HttpContext.Session.Set(WC.SessionCart, shoppingCarts);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> products)
        {
            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            foreach (var prod in products)
            {
                shoppingCarts.Add(new ShoppingCart { ProductId = prod.Id, Sqft = prod.TempSqft });
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCarts);
            return RedirectToAction(nameof(Summary));

        }

        public IActionResult Summary()
        {
            ApplicationUser applicationUser;
            if (User.IsInRole(WC.AdminRole))
            {
                if (HttpContext.Session.Get<int>(WC.SessionInquiryId) != 0)
                {
                    //cart has been loaded using an inquiry
                    InquiryHeader inquiryHeader = _unitOfWork.InquiryHeaders.
                        FirstOrDefault(inq => inq.Id == HttpContext.Session.Get<int>(WC.SessionInquiryId));
                    applicationUser = new ApplicationUser()
                    {
                        Email = inquiryHeader.Email,
                        FullName = inquiryHeader.FullName,
                        PhoneNumber = inquiryHeader.PhoneNumber
                    };
                }
                else
                {
                    applicationUser = new ApplicationUser();
                }
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                //var userId = User.FindFirstValue(ClaimTypes.Name);
                applicationUser = _unitOfWork.applicationUsers.FirstOrDefault(au => au.Id == claim.Value);
            }

            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCarts = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            List<int> prodInCart = shoppingCarts.Select(p => p.ProductId).ToList();
            IEnumerable<Product> products = _unitOfWork.Products.GetAll(p => prodInCart.Contains(p.Id));
            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = applicationUser,
                ProductList = products.ToList()
            };
            foreach(var item in shoppingCarts)
            {
                ProductUserVM.ProductList.First(p => p.Id == item.ProductId).TempSqft = item.Sqft;
            }

            return View(ProductUserVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (User.IsInRole(WC.AdminRole))
            {
                //we need to create an order
                //var orderTotal = 0.0;
                //foreach(var prod in ProductUserVM.ProductList)
                //{
                //    orderTotal += prod.Price * prod.TempSqft;
                //}
                OrderHeader orderHeader = new OrderHeader()
                {
                    CreatedByUserId = claim.Value,
                    //FinalOrderTotal = orderTotal,
                    FinalOrderTotal = ProductUserVM.ProductList.Sum(x => x.TempSqft * x.Price),
                    City = ProductUserVM.ApplicationUser.City,
                    StreetAddress = ProductUserVM.ApplicationUser.StreetAddress,
                    State = ProductUserVM.ApplicationUser.State,
                    PostalCode = ProductUserVM.ApplicationUser.PostalCode,
                    FullName = ProductUserVM.ApplicationUser.FullName,
                    Email = ProductUserVM.ApplicationUser.Email,
                    PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                    OrderDate = DateTime.Now,
                    OrderStatus = WC.StatusPending
                };
                _unitOfWork.OrderHeaders.Add(orderHeader);
                _unitOfWork.Complete();

                foreach (var prod in ProductUserVM.ProductList)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        OrderHeaderId = orderHeader.Id,
                        PricePerSqFt = prod.Price,
                        Sqft = prod.TempSqft,
                        ProductId = prod.Id
                    };
                    _unitOfWork.OrderDetails.Add(orderDetail);
                }
                _unitOfWork.Complete();
                return RedirectToAction(nameof(InquiryConfirmation), new { id = orderHeader.Id});

            }
            else
            {
                //we need to create an inquiry
                var pathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                    + "Templates" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";
                var subject = "New Inquiry";
                string HtmlBody = "";
                using (StreamReader sr = System.IO.File.OpenText(pathToTemplate))
                {
                    HtmlBody = sr.ReadToEnd();
                }

                StringBuilder productListSB = new StringBuilder();
                foreach (var prod in ProductUserVM.ProductList)
                {
                    productListSB.Append($" - Name: { prod.Name} <span style='font-size:14px;'> (ID: { prod.Id})</span><br/>");
                }
                string messageBody = string.Format(HtmlBody,
                    ProductUserVM.ApplicationUser.FullName,
                    ProductUserVM.ApplicationUser.Email,
                    ProductUserVM.ApplicationUser.PhoneNumber,
                    productListSB.ToString());
                await _emailSender.SendEmailAsync(WC.EmailAdmin, subject, messageBody);

                InquiryHeader inquiryHeader = new InquiryHeader()
                {
                    ApplicationUserId = claim.Value,
                    FullName = ProductUserVM.ApplicationUser.FullName,
                    Email = ProductUserVM.ApplicationUser.Email,
                    PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                    InquiryDate = DateTime.Now
                };
                _unitOfWork.InquiryHeaders.Add(inquiryHeader);
                _unitOfWork.Complete();

                foreach (var prod in ProductUserVM.ProductList)
                {
                    InquiryDetail inquiryDetail = new InquiryDetail()
                    {
                        InquiryHeaderId = inquiryHeader.Id,
                        ProductId = prod.Id
                    };
                    _unitOfWork.InquiryDetails.Add(inquiryDetail);
                }
                _unitOfWork.Complete();
            }
            return RedirectToAction(nameof(InquiryConfirmation));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UPdateCart(IEnumerable<Product> products)
        {
            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            foreach(var prod in products)
            {
                shoppingCarts.Add(new ShoppingCart { ProductId = prod.Id, Sqft = prod.TempSqft });
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCarts);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult InquiryConfirmation(int id = 0)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaders.FirstOrDefault(order => order.Id == id);
            HttpContext.Session.Clear();
            return View(orderHeader);
        }
    }
}
