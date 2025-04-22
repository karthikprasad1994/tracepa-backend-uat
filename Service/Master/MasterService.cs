using TracePca.Data;
using TracePca.Interface.Master;

namespace TracePca.Service.Master
{
    public class MasterService: MasterInterface
    {
        private readonly Trdmyus1Context _context;

        public MasterService(Trdmyus1Context context)
        {
            _context = context;
        }




    }
}
