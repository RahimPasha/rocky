using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rocky_DataAccess;
using Rocky_Models;
using Rocky_Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Rocky_Utility;
using Rocky_DataAccess.Repository;
using Rocky_DataAccess.Repository.IRepository;

namespace rocky.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            HomeVM obj = new HomeVM()
            {
                Categories = _unitOfWork.Categories.GetAll(),
                Products = _unitOfWork.Products.GetAll(includeProperties:"Category,ApplicationType")
            };
            return View(obj);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Details(int id)
        {
             List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCarts = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            DetailsVM DetailsVM = new DetailsVM()
            {
                Product = _unitOfWork.Products.FirstOrDefault(p => p.Id == id, "Category,ApplicationType"),
                ExistsInCart = false
            };
            foreach(ShoppingCart item in shoppingCarts)
            {
                if(item.ProductId == id)
                {
                    DetailsVM.ExistsInCart = true;
                }
            }
            return View(DetailsVM);
        }

        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id, DetailsVM detailsVM)
        {
            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCarts = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);                
            }
            shoppingCarts.Add(new ShoppingCart { ProductId = id, Sqft=detailsVM.Product.TempSqft});
            HttpContext.Session.Set(WC.SessionCart, shoppingCarts);
            return RedirectToAction(nameof(Index));
        }



        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCarts = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            var item = shoppingCarts.SingleOrDefault(itm => itm.ProductId == id);
            if(item != null)
            {
                shoppingCarts.Remove(item);
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCarts);
            return RedirectToAction(nameof(Index));
        }
    }
}
