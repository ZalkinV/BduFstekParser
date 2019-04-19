using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BduFstekParser
{
	class ThreatEntry
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Intruder { get; set; }
		public string Target { get; set; }
		public bool IsConfident { get; set; }
		public bool IsIntegrity { get; set; }
		public bool IsAvailability { get; set; }

		public ThreatEntry(int id, string name, string description, string intruder, string target, bool isConfident, bool isIntegrity, bool isAvailability)
		{
			Id = id;
			Name = name;
			Description = description;
			Intruder = intruder;
			Target = target;
			IsConfident = isConfident;
			IsIntegrity = isIntegrity;
			IsAvailability = isAvailability;
		}

		public string GetFullInfo()
		{
			return Id.ToString();
		}
	}
}
