using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {

        public ProductRepository(ApplicationDBContext dBContext):base(dBContext)
        {
        }

        public IEnumerable<SelectListItem> GetAllDropdownList(string obj)
        {
            if (obj == WC.CategoryName)
            {
                return (_db as ApplicationDBContext).Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }
            else if (obj == WC.ApplicationTypeName)
            {
                return (_db as ApplicationDBContext).ApplicationType.Select(app => new SelectListItem
                {
                    Text = app.Name,
                    Value = app.Id.ToString()
                });
            }
            else return null;
        }


    }
}
