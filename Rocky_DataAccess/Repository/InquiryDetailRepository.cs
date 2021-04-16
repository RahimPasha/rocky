using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_DataAccess.Repository
{
    public class InquiryDetailRepository : Repository<InquiryDetail>, IInquiryDetailRepository
    {
        public InquiryDetailRepository(ApplicationDBContext applicationDBContext):base(applicationDBContext)
        {
        }

    }
}
