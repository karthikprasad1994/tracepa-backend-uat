using Microsoft.AspNetCore.Mvc;
using TracePca.Interface;
using TracePca.Interface.FixedAssetsInterface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FixedAssets
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetTransactionAdditionController : ControllerBase
    {
    

            private AssetTransactionAdditionInterface _AssetTransactionInterface;
            public AssetTransactionAdditionController(AssetTransactionAdditionInterface AssetTransactionInterface)
            {
                _AssetTransactionInterface = AssetTransactionInterface;

            }
            // GET: api/<AssetTransactionController>
            [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AssetTransactionController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AssetTransactionController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AssetTransactionController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AssetTransactionController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
