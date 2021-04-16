using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Rocky_Models;
using Rocky_Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rocky_Utility;
using Rocky_DataAccess.Repository;

namespace rocky.Controllers
{
    [Authorize(Roles =WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;

        }
        public IActionResult Index()
        {
            IEnumerable<Product> objList = _unitOfWork.Products.GetAll(includeProperties: "Category,ApplicationType");
            
            //foreach(var obj in objList)
            //{
            //    obj.Category = _db.Category.FirstOrDefault(c => c.Id == obj.CategoryId);
            //}
            return View(objList);
        }
        //Get-Upsert   
        public IActionResult Upsert(int? id)
        {

            //IEnumerable<SelectListItem> CategoryDropDown = _db.Category.Select(i => new SelectListItem
            //{
            //    Text = i.Name,
            //    Value = i.Id.ToString()
            //});
            ////ViewBag.CategoryDropDown = CategoryDropDown;
            //ViewData["CategoryDropDown"] = CategoryDropDown;
            //Product prod = new Product();
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _unitOfWork.Products.GetAllDropdownList(WC.CategoryName),
                ApplicationTypeSelectList = _unitOfWork.Products.GetAllDropdownList(WC.ApplicationTypeName)
            };

                
            if (id == null)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.Products.Find(id.GetValueOrDefault());
                if(productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }
            
        }

        //Post - Upsert
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                if (productVM.Product.Id == 0)
                {
                    //creating
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;
                    _unitOfWork.Products.Add(productVM.Product);
                }
                else
                {
                    //updating
                    var objFromDB = _unitOfWork.Products.FirstOrDefault(prod => prod.Id == productVM.Product.Id,isTracking:false);
                    if (objFromDB == null)
                        return NotFound();
                    if(files.Count > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);
                        var oldFile = Path.Combine(upload, objFromDB.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDB.Image;
                    }
                    objFromDB = productVM.Product;
                }
                _unitOfWork.Complete();
                return RedirectToAction("Index");
            }
            productVM.CategorySelectList = _unitOfWork.Products.GetAllDropdownList(WC.CategoryName);

            productVM.ApplicationTypeSelectList = _unitOfWork.Products.GetAllDropdownList(WC.ApplicationTypeName);
            return View(productVM);
        }

      

        //Get - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var product = _unitOfWork.Products.FirstOrDefault(predicate: p => p.Id == id, includeProperties: "Category,ApplicationType");
            //product.Category = _db.Category.Find(product.CategoryId);
            if (product == null)
                return NotFound();
            return View(product);
        }
    

        //Post
        [HttpPost, ActionName("Delete")]
        [AutoValidateAntiforgeryToken]
        public IActionResult DeleteConfirmed(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound();
            var obj = _unitOfWork.Products.Find(Id.GetValueOrDefault());
            if (obj == null)
                return NotFound();
            string webRootPath = _webHostEnvironment.WebRootPath;
            string address = webRootPath + WC.ImagePath;
            var fileAddress = Path.Combine(address, obj.Image);
            if (System.IO.File.Exists(fileAddress))
            {
                System.IO.File.Delete(fileAddress);
            }
            _unitOfWork.Products.Remove(obj);
            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }

    }
}
