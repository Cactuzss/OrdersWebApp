using Microsoft.AspNetCore.Mvc;
using TestApp.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestApp.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class Items : ControllerBase
    {
        [HttpGet("{orderid}")]
        public IEnumerable<OrderItem> Get(int orderid)
        {
            return DBControllers.GenericController<OrderItem>.GetModel((db) => { return db.OrderItem.Where((o) => o.OrderId == orderid).ToList(); });
        }
    }
}
