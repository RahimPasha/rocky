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
using Rocky_DataAccess.Repository;

namespace rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ApplicationTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<ApplicationType> objList = _unitOfWork.ApplicationTypes.GetAll();
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
            var obj = _unitOfWork.ApplicationTypes.Find(Id.GetValueOrDefault());
            if (obj == null)
                return NotFound();
            return View(obj);
        }

        //Get-Delete
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound();
            var obj = _unitOfWork.ApplicationTypes.Find(Id.GetValueOrDefault());
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
                _unitOfWork.ApplicationTypes.Add(obj);
                _unitOfWork.Complete();
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
                var applicationType = _unitOfWork.ApplicationTypes.FirstOrDefault(a => a.Id == obj.Id);
                if(applicationType != null)
                {
                    applicationType = obj;
                }
                
                _unitOfWork.Complete();
                return RedirectToAction("Index");
            }
            else
                return View(obj);
        }

        public IActionResult DeleteConfirmed(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound();
            var obj = _unitOfWork.ApplicationTypes.Find(Id.GetValueOrDefault());
            if (obj == null)
                return NotFound();
            _unitOfWork.ApplicationTypes.Remove(obj);
            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }
    }
}
