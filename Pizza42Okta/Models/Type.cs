using System.ComponentModel.DataAnnotations;

namespace Pizza42Okta.Models
{
	public class Type
	{
		[Key]
		public int Id { get; set; }	
		public string Name { get; set; }	
	}
}
