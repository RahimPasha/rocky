﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_DataAccess.Repository.IRepository
{
    public interface IProductRepository:IRepository<Product>
    {
        IEnumerable<SelectListItem> GetAllDropdownList(string obj);
    }
}