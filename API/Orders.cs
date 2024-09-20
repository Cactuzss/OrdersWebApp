using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Web;
using TestApp.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestApp.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class Orders : ControllerBase
    {
        [HttpGet("{page}")]
        public IEnumerable<Order> Get(
            int page, int pageSize, string startDate, string endDate,
            [FromQuery(Name = "OrderNumber")] IEnumerable<string> numFilter,
            [FromQuery(Name = "OrderItemName")] IEnumerable<string> itemFilter,
            [FromQuery(Name = "OrderItemUnit")] IEnumerable<string> unitFilter,
            [FromQuery(Name = "ProviderName")] IEnumerable<string> providerFilter,
            [FromQuery(Name = "ProviderId")] IEnumerable<string> providerIdFilter
        )
        {
            pageSize = pageSize == 0 ? 10 : pageSize;
            List<Order> filtered = DBControllers.GenericController<Order>.GetModel((db) => { return db.Order.ToList(); });

            DateTime start = DateTime.ParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            filtered = DBControllers.GenericController<Order>.
                Filter<DateTime>(filtered,
                new List<DateTime> { start, end },
                (o) => { return o.Date >= start && o.Date <= end; });

            if (numFilter.Count() != 0)
                filtered = DBControllers.GenericController<Order>.Filter<string>(filtered, numFilter.ToList(), (o) => { return numFilter.Contains(o.Number); });

            if (itemFilter.Count() != 0)
            {
                var items = DBControllers.GenericController<OrderItem>.Filter<string>(
                    DBControllers.GenericController<OrderItem>.GetModel((db) => { return db.OrderItem.ToList(); }),
                    itemFilter.ToList(), (o) => { return itemFilter.Contains((o.Name ?? "").Replace(" ", "_")); });

                filtered = DBControllers.GenericController<Order>.Filter<OrderItem>(filtered, items, (o) => {
                    foreach (var item in items)
                        if (item.OrderId == o.Id)
                            return true;
                    return false;
                });
            }

            if (unitFilter.Count() != 0)
            {
                var items = DBControllers.GenericController<OrderItem>.Filter<string>(
                    DBControllers.GenericController<OrderItem>.GetModel((db) => { return db.OrderItem.ToList(); }),
                    unitFilter.ToList(), (o) => { return unitFilter.Contains((o.Unit ?? "").Replace(" ", "_")); });

                filtered = DBControllers.GenericController<Order>.Filter<OrderItem>(filtered, items, (o) => {
                    foreach (var item in items)
                        if (item.OrderId == o.Id)
                            return true;
                    return false;
                });
            }

            if (providerFilter.Count() != 0)
            {
                var providers = DBControllers.GenericController<Provider>.Filter<string>(
                    DBControllers.GenericController<Provider>.GetModel((db) => { return db.Provider.ToList(); }),
                    providerFilter.ToList(), (o) => { return providerFilter.Contains((o.Name ?? "").Replace(" ", "_")); });

                filtered = DBControllers.GenericController<Order>.Filter<Provider>(filtered, providers, (o) => {
                    foreach (var item in providers)
                        if (item.Id == o.ProviderID)
                            return true;
                    return false;
                });
            }

            if (providerIdFilter.Count() != 0)
            {
                filtered = DBControllers.GenericController<Order>.Filter<string>(filtered, providerIdFilter.ToList(), (o) => { return providerIdFilter.Contains(o.ProviderID.ToString()); });
            }


            var res = filtered.Chunk(pageSize);

            if (page >= res.ToList().Count) return new List<Order>();
            return res.ToList().Count == 0 ? filtered : res.ToList()[page];
        }

        [HttpGet("id/{id}")]
        public Order Get(int id)
        {
            return DBControllers.GenericController<Order>.GetModel((db) => { return db.Order.ToList(); }).SingleOrDefault((o) => o.Id == id ) ?? new Order();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            DBControllers.GenericController<Order>.DeleteOrderByID(id);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]OrderDTO vals)
        {
            Order order = new Order();

            order.Id = int.Parse(vals.id);
            order.Number = vals.number;
            order.Date = DateTime.Parse(vals.date.Replace("_", " "));

            if (DBControllers.GenericController<Order>.
                GetModel((db) => { return db.Order.ToList(); }).
                SingleOrDefault((o) => o.Number == order.Number && o.ProviderID == order.ProviderID) != null)
            {
                return new BadRequestObjectResult("ExistingOrderNumber");
            }

            vals.providerName = HttpUtility.UrlDecode(vals.providerName.Replace("_", " "));
            var provider = DBControllers.GenericController<Provider>.GetModel(
                    (db) => { return db.Provider.ToList(); }).
                    SingleOrDefault((o) => o.Name == vals.providerName);

            if (provider != null)
            {
                order.ProviderID = provider.Id;
            }

            List<OrderItem> items = new List<OrderItem>();

            foreach(var el in vals.orderItems)
            {
                var tmp = new OrderItem();

                tmp.OrderId = order.Id;
                //tmp.Id = el.id;
                tmp.Quantity = decimal.Parse(el.quantity.Replace(".", ","));
                tmp.Unit = HttpUtility.UrlDecode(el.unit);
                tmp.Name = HttpUtility.UrlDecode(el.name.Replace("_", " "));
                
                items.Add(tmp);
            }

            var item = items.SingleOrDefault((o) => o.Name == order.Number);
            if (item != null)
                return new BadRequestObjectResult("ItemNameError");

            DBControllers.GenericController<Order>.Save((db) =>
            {
                var toChange = db.Order.SingleOrDefault((o) => o.Id == order.Id);
                if (toChange == null)
                    return -1;

                toChange.ProviderID = order.ProviderID;
                toChange.Number = order.Number;
                toChange.Date = order.Date;

                DBControllers.GenericController<Order>.DeleteItemsByOrder(order.Id);
                
                foreach(var item in items)
                    db.OrderItem.Add(item);

                return 0;
            });

            return new OkResult();
        }

        [HttpPost("")]
        public IActionResult Post([FromBody] OrderDTO vals)
        {
            Order order = new Order();

            order.Date = DateTime.Parse(vals.date.Replace("_", " "));

            vals.providerName = HttpUtility.UrlDecode(vals.providerName.Replace("_", " "));
            var provider = DBControllers.GenericController<Provider>.GetModel(
                    (db) => { return db.Provider.ToList(); }).
                    SingleOrDefault((o) => o.Name == vals.providerName);

            if (provider != null)
            {
                order.ProviderID = provider.Id;
            }

            order.Number = vals.number;
            if (DBControllers.GenericController<Order>.
                GetModel((db) => { return db.Order.ToList(); }).
                SingleOrDefault((o) => o.Number == order.Number && o.ProviderID == order.ProviderID) != null)
            {
                return new BadRequestObjectResult("ExistingOrderNumber");
            }


            var items = new List<OrderItem>();
            foreach(var el in vals.orderItems)
            {
                var tmp = new OrderItem();

                tmp.Quantity = decimal.Parse(el.quantity.Replace(".", ","));
                tmp.Unit = HttpUtility.UrlDecode(el.unit);
                tmp.Name = HttpUtility.UrlDecode(el.name.Replace("_", " "));

                items.Add(tmp);
            }

            var item = items.SingleOrDefault((o) => o.Name == order.Number);
            if (item != null)
                return new BadRequestObjectResult("ItemNameError");

            DBControllers.GenericController<Order>.Save((db) =>
            {
                db.Order.Add(order);
                db.SaveChanges();
                return 0;
            });

            order = DBControllers.GenericController<Order>.GetModel((db) => { return db.Order.ToList(); }).SingleOrDefault((o) =>
            {
                return o.Number == order.Number && o.ProviderID == order.ProviderID;
            }) ?? new Order();

            DBControllers.GenericController<OrderItem>.Save((db) =>
            {
                foreach(var el in items)
                {
                    el.OrderId = order.Id;
                    db.OrderItem.Add(el);
                }

                db.SaveChanges();
                return 0;

            });

            return new OkResult();
        }
    }
}
