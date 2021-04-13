using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess;
using Rocky_Models;
using System;
using System.Collections.Generic;
using Rocky_Utility;
using System.Linq;
using System.Threading.Tasks;
using Rocky_DataAccess.Repository.IRepository;

namespace rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly  IApplicationTypeRepositoy _applicationTypeRepositoy;
        public ApplicationTypeController(IApplicationTypeRepositoy applicationTypeRepositoy)
        {
            _applicationTypeRepositoy = applicationTypeRepositoy;
        }
        public IActionResult Index()
        {
            IEnumerable<ApplicationType> objList = _applicationTypeRepositoy.GetAll();
            return View(objList);
        }
        //Get
        public IActionResult Create()
        {
            return View();
        }
        //Get-Edit
        public IActionResult Edit(int? Id)
        {

            if (Id == null || Id == 0)
                return NotFound();
            var obj = _applicationTypeRepositoy.Find(Id.GetValueOrDefault());
            if (obj == null)
                return NotFound();
            return View(obj);
        }

        //Get-Delete
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound();
            var obj = _applicationTypeRepositoy.Find(Id.GetValueOrDefault());
            if (obj == null)
                return NotFound();
            return View(obj);
        }

        //Post
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _applicationTypeRepositoy.Add(obj);
                _applicationTypeRepositoy.Save();
                return RedirectToAction("Index");
            }
            else
                return View(obj);
        }

        //Post-Edit
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult EditPost(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _applicationTypeRepositoy.Update(obj);
                _applicationTypeRepositoy.Save();
                return RedirectToAction("Index");
            }
            else
                return View(obj);
        }

        public IActionResult DeleteConfirmed(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound();
            var obj = _applicationTypeRepositoy.Find(Id.GetValueOrDefault());
            if (obj == null)
                return NotFound();
            _applicationTypeRepositoy.Remove(obj);
            _applicationTypeRepositoy.Save();
            return RedirectToAction("Index");
        }
    }
}
