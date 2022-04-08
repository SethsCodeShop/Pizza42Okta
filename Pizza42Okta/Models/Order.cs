using System;
using System.ComponentModel.DataAnnotations;

namespace Pizza42Okta.Models
{
    public class Order
    {
        [Key]
        public int PizzaOrderId { get; set; }
        public string UserId { get; set; }
        public Type Type { get; set; }  
        public DateTime Created { get; set; }
    }
}