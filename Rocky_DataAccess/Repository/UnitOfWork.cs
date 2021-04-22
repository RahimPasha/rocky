using Rocky_DataAccess.Repository.IRepository;
using System.Linq;


namespace Rocky_DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDBContext _db;
        public UnitOfWork(ApplicationDBContext dBContext)
        {
            _db = dBContext;
            Products = new ProductRepository(_db);
            ApplicationTypes = new ApplicationTypeRepository(_db);
            Categories = new CategoryRepositoy(_db);
            InquiryDetails = new InquiryDetailRepository(_db);
            InquiryHeaders = new InquiryHeaderRepository(_db);
            applicationUsers = new ApplicationUserRepository(_db);
            OrderHeaders = new OrderHeaderRepository(_db);
            OrderDetails = new OrderDetailRepository(_db);

        }

        public IProductRepository Products { get; private set; }
        public IApplicationTypeRepositoy ApplicationTypes { get; private set; }
        public ICategoryRepositoy Categories { get; private set; }

        public IInquiryDetailRepository InquiryDetails { get; private set; }

        public IInquiryHeaderRepository InquiryHeaders { get; private set; }

        public IApplicationUserRepository applicationUsers { get; private set; }

        public IOrderHeaderRepository OrderHeaders { get; set; }

        public IOrderDetailRepository OrderDetails { get; set; }

        public int Complete()
        {
            return _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
