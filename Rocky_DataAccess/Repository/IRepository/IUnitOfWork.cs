using Rocky_DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_DataAccess.Repository
{
    public interface IUnitOfWork: IDisposable
    {
        IProductRepository Products { get; }
        IApplicationTypeRepositoy ApplicationTypes { get; }
        ICategoryRepositoy Categories { get; }
        IInquiryDetailRepository InquiryDetails { get; }
        IInquiryHeaderRepository InquiryHeaders { get; }
        IApplicationUserRepository applicationUsers { get; }
        IOrderHeaderRepository OrderHeaders { get; }
        IOrderDetailRepository OrderDetails { get; }
        int Complete();
    }
}
