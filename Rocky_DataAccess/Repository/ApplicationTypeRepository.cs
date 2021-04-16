using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_DataAccess.Repository
{
    public class ApplicationTypeRepository : Repository<ApplicationType>, IApplicationTypeRepositoy
    {
        public ApplicationTypeRepository(ApplicationDBContext dBContext):base(dBContext)
        {
         
        }
 
    }
}
