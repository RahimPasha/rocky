using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;


namespace Rocky_DataAccess.Repository
{
    public class InquiryHeaderRepository : Repository<InquiryHeader>, IInquiryHeaderRepository
    {

        public InquiryHeaderRepository(ApplicationDBContext dBContext) : base(dBContext)
        {
        }

    }
}
