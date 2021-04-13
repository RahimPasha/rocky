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

namespace rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {

        private readonly ICategoryRepositoy _categoryRepositoy;
        public CategoryController(ICategoryRepositoy categoryRepositoy)
        {
            _categoryRepositoy = categoryRepositoy;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objList = _categoryRepositoy.GetAll();
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
                _categoryRepositoy.Add(obj);
                _categoryRepositoy.Save();
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
            var obj = _categoryRepositoy.Find(Id.GetValueOrDefault());
            if (obj == null)
                return NotFound();
            return View(obj);
        }

        //Get - Delete
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound();
            var obj = _categoryRepositoy.Find(Id.GetValueOrDefault());
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
                _categoryRepositoy.Update(obj);
                _categoryRepositoy.Save();
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
            var obj = _categoryRepositoy.Find(Id.GetValueOrDefault());
            if (obj == null)
                return NotFound();
            _categoryRepositoy.Remove(obj);
            _categoryRepositoy.Save();
            return RedirectToAction("Index");
        }

    }
}