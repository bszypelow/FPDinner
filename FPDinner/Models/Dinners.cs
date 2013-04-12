using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FPDinner.Models
{
	public class Dinner
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public bool HasPotatoes { get; set; }
	}

	public class Salad
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	public class AdminViewModel
	{
		public IEnumerable<Dinner> Dinners { get; set; }
		public IEnumerable<Salad> Salads { get; set; }
	}
}