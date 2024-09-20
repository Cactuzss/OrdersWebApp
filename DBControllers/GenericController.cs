namespace TestApp.DBControllers
{
    public static class GenericController<T>
    {
        public static List<T> GetModel(Func<ApplicationContext, List<T>> dbSetAccessor)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return dbSetAccessor(db);
            }
        }

        public static List<T> Filter<T2>(List<T> items, List<T2> values, Func<T, bool> clause)
        {
            return items.Where(clause).ToList();
        }

        public static List<string> GetValuesAsStr(List<T> items, Func<T, string> func)
        {
            List<string> result = new List<string>();

            foreach (T item in items)
                result.Add(func(item));

            return result;
        }

        public static void Save(Func<ApplicationContext, int> func)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                func(db);
                db.SaveChanges();
            }
        }

        public static void DeleteOrderByID(int id)
        {
            DeleteItemsByOrder(id);
            using (ApplicationContext db = new ApplicationContext())
            {
                var todelete = db.Order.SingleOrDefault((o) => o.Id == id);
                if (todelete == null) return;

                db.Order.Remove(todelete);
                db.SaveChanges();
            }
        }

        public static void DeleteItemsByOrder(int orderId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var orderItems = db.OrderItem.Where((o) => o.OrderId == orderId);
                db.OrderItem.RemoveRange(orderItems);
                db.SaveChanges();
            }
        }
    }
}
