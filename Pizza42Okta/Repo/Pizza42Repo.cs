using Microsoft.EntityFrameworkCore;
using Pizza42Okta.Models;
using System.Collections.Generic;
using System.Linq;

namespace Pizza42Okta.Repo
{
	public class Pizza42Repo
	{
		private readonly PizzaContext _Context;
		public Pizza42Repo(PizzaContext context)
		{
			_Context = context;
		}

		public List<Order> GetHistory(string userId)
		{
			return _Context.Orders.Include(b => b.Type).Where(a => a.UserId == userId).ToList();
		}

		public List<Type> GetPizzaTypes()
		{
            var pizzaTypes = new List<Models.Type>();

            pizzaTypes = _Context.Types.ToList();

            if (!pizzaTypes.Any())
            {
                pizzaTypes = new List<Type>()
                {
                    new Models.Type()
                    {
                        Name = "Pepperoni"
                    },
                    new Models.Type()
                    {
                        Name = "Sausage"
                    },
                    new Models.Type()
                    {
                        Name = "Supreme"
                    }
                };

                _Context.Types.AddRange(pizzaTypes);
                _Context.SaveChanges();
                pizzaTypes = _Context.Types.ToList();
            }

            return pizzaTypes;
        }

        public Order AddOrder(int pizzaOrderTypeId, string userId)
		{
            var orderType = _Context.Types.Where(a => a.Id == pizzaOrderTypeId).FirstOrDefault();

            var order = new Order()
            {
                Type = orderType,
                UserId = userId,
                Created = System.DateTime.UtcNow
            };
            var result = _Context.Orders.Add(order);
            _Context.SaveChanges();

            return result.Entity;
		}

        public void RemoveAllOrders()
		{
            var types = _Context.Types ;

            foreach(var t in types)
			{
                _Context.Types.Remove(t);
			}
            _Context.SaveChanges();
		}
	}
}
