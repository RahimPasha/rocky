using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess;
using Rocky_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocky_Utility;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_DataAccess.Repository;

namespace rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objList = _unitOfWork.Categories.GetAll();
            return View(objList);
        }
        //Get
        public IActionResult Create()
        {
            return View();
        }

        //Post
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Categories.Add(obj);
                _unitOfWork.Complete();
                TempData[WC.Success] = "Category created successfully";
                return RedirectToAction("Index");
            }
            else
                TempData[WC.Error] = "Error while creating category";

            return View(obj);

        }

        //Get - Edit
        public IActionResult Edit(int? Id)
        {
            if(Id == null || Id == 0)
                return NotFound();
            var obj = _unitOfWork.Categories.Find(Id.GetValueOrDefault());
            if (obj == null)
                return NotFound();
            return View(obj);
        }

        //Get - Delete
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound();
            var obj = _unitOfWork.Categories.Find(Id.GetValueOrDefault());
            if (obj == null)
                return NotFound();
            return View(obj);
        }
        //Post
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                var category = _unitOfWork.Categories.FirstOrDefault(c => c.Id == obj.Id);
                if(category != null)
                {
                    category = obj;
                }
                _unitOfWork.Complete();
                return RedirectToAction("Index");
            }
            else
                return View(obj);

        }

        //Post
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult DeleteConfirmed(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound();
            var obj = _unitOfWork.Categories.Find(Id.GetValueOrDefault());
            if (obj == null)
                return NotFound();
            _unitOfWork.Categories.Remove(obj);
            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }

    }
}