using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using System.Linq;



namespace Rocky_DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {

        public OrderHeaderRepository(ApplicationDBContext dBContext) : base(dBContext)
        {
        }

    }
}
