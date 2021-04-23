using Braintree;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_Utility.BrainTree
{
    public class BrainTreeGate : IBrainTreeGate
    {
        public BrainTreeSettings _options { get; set; }
        private IBraintreeGateway braintreeGateway { get; set; }
        public BrainTreeGate(IOptions<BrainTreeSettings> options)
        {
            _options = options.Value;
        }
        public IBraintreeGateway CreateGateWay()
        {
            return new BraintreeGateway(_options.Environment, _options.MerchantId,
                _options.PublicKey, _options.PrivateKey);
        }

        public IBraintreeGateway GetGateWay()
        {
            if(braintreeGateway == null)
            {
                braintreeGateway = CreateGateWay();
            }
            return braintreeGateway;
        }
    }
}
