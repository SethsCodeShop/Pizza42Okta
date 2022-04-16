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

        public Type GetPizzaType(int pizzaTypeId)
        {
            return _Context.Types.Where(a => a.Id == pizzaTypeId).FirstOrDefault();
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
