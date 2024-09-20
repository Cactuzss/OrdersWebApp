using Microsoft.AspNetCore.Mvc;
using TestApp.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestApp.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class Filters : ControllerBase
    {
        // GET /api/filters/type
        [HttpGet("{type}")]
        public IEnumerable<string> Get(string type)
        {
            switch (type)
            {
                case "OrderNumber":
                    return DBControllers.GenericController<Order>.
                        GetValuesAsStr(DBControllers.GenericController<Order>.GetModel((db) => { return db.Order.ToList(); }),
                        (o) => { return o.Number ?? ""; })
                        .Distinct();

                case "OrderItemName":
                    return DBControllers.GenericController<OrderItem>.
                        GetValuesAsStr(DBControllers.GenericController<OrderItem>.GetModel((db) => { return db.OrderItem.ToList(); }),
                        (o) => { return o.Name ?? ""; })
                        .Distinct();

                case "OrderItemUnit":
                    return DBControllers.GenericController<OrderItem>.
                        GetValuesAsStr(DBControllers.GenericController<OrderItem>.GetModel((db) => { return db.OrderItem.ToList(); }),
                        (o) => { return o.Unit ?? ""; })
                        .Distinct();

                case "ProviderName":
                    return DBControllers.GenericController<Provider>.
                        GetValuesAsStr(DBControllers.GenericController<Provider>.GetModel((db) => { return db.Provider.ToList(); }),
                        (o) => { return o.Name ?? ""; })
                        .Distinct();

                case "ProviderId":
                    return DBControllers.GenericController<Provider>.
                        GetValuesAsStr(DBControllers.GenericController<Provider>.GetModel((db) => { return db.Provider.ToList(); }),
                        (o) => { return o.Id.ToString(); })
                        .Distinct();

                default:
                    break;
            }

            return new string[] { "Wrong Filter type. Available: OrderNumber, OrderItemName. OrderItemUnit, ProviderName. ProviderId" };
        }

    }
}
