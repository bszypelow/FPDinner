﻿using System.Collections.Generic;

namespace FPDinner.Models
{
	public class Dinner
	{
		public int Id { get; set; }
		public string Provider { get; set; }
		public string Name { get; set; }
		public bool HasPotatoes { get; set; }
		public bool AvailableByDefault { get; set; }
	}

	public class Salad
	{
		public int Id { get; set; }
		public string Provider { get; set; }
		public string Name { get; set; }
	}

	public class DinnerAvailability
	{
		public Dinner Dinner { get; set; }
		public bool IsAvailable { get; set; }
	}

	public class SaladAvailability
	{
		public Salad Salad { get; set; }
		public bool IsAvailable { get; set; }
	}

	public class AdminViewModel
	{
		public IEnumerable<DinnerAvailability> Dinners { get; set; }
		public IEnumerable<SaladAvailability> Salads { get; set; }
	}
}