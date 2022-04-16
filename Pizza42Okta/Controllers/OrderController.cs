using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza42Okta.Models;

namespace Pizza42Okta.Controllers
{
    [Route("api/Order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly Repo.Pizza42Repo _Repo;
		public OrderController()
		{
            _Repo = new Repo.Pizza42Repo(new PizzaContext());
		}

        [HttpPost("add")]
        [Authorize("add:orders")]
        public IActionResult Add(PizzaOrderAdd order)
        {
            try
            {
                var pizzaType = _Repo.GetPizzaType(order.PizzaOrderTypeId);
                var oktaAPI = new Repo.OktaAPI(User.Identity.Name);
                var newOrder = oktaAPI.AddOrder(pizzaType.Name);
                return Ok(newOrder);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("pizzaTypes")]
        [Authorize]
        public IActionResult PizzaTypes()
        {
            try
            {
                var pizzaTypes = _Repo.GetPizzaTypes();

                return Ok(pizzaTypes);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("history")]
        [Authorize]
        public IActionResult History()
        {
            try
            {
                var oktaAPI = new Repo.OktaAPI(User.Identity.Name);
                var history = oktaAPI.GetOrderHistory();
                return Ok(history);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // This is a helper action. It allows you to easily view all the claims of the token.
        [HttpGet("claims")]
        [Authorize("read:users")]
        public IActionResult Claims()
        {
            return Ok(User.Claims.Select(c =>
                new
                {
                    c.Type,
                    c.Value
                }));
        }
    }
}
