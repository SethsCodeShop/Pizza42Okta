﻿using System;
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
                var userId = User.Identity.Name.Replace("google-oauth2|", "").Replace("auth0|", "");

                var newOrder = _Repo.AddOrder(order.PizzaOrderTypeId, userId);

                return Ok(newOrder);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("RemoveAll")]
        [Authorize("remove:orders")]
        public IActionResult RemoveAll()
		{
            _Repo.RemoveAllOrders();
            return Ok(true);
		}

        [HttpGet("RemoveAllTypes")]
        [Authorize("remove:orders")]
        public IActionResult RemoveAllTypes()
        {
            _Repo.RemoveAllOrders();
            return Ok(true);
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
                var pipeIndex = User.Identity.Name.IndexOf("|");
                var userId = User.Identity.Name.Substring(pipeIndex + 1);
                var pizzaOrders = _Repo.GetHistory(userId);
                return Ok(pizzaOrders);
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
