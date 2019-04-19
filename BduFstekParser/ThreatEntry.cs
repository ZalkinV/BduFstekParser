using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BduFstekParser
{
	public class ThreatEntry
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
			return
					$"Описание УБИ. {Id}:\n" +
					$" - Наименование: {Name}\n" +
					$" - Описание: {Description}\n" +
					$" - Источник: {Intruder}\n" +
					$" - Объект воздействия: {Target}\n" +
					$" - Нарушение конфиденциальности: {(IsConfident ? "есть" : "отсутствует")}\n" +
					$" - Нарушение целостности: {(IsIntegrity ? "есть" : "отсутствует")}\n" +
					$" - Нарушение доступности: {(IsAvailability ? "есть" : "отсутствует")}";
		}

		public override string ToString()
		{
			return $"{Id}. {Name}; {Description}; {Intruder}; {Target}; {IsConfident}; {IsIntegrity}; {IsAvailability}";
		}

		public override bool Equals(object obj)
		{
			var entry = obj as ThreatEntry;
			return entry != null &&
				   Id == entry.Id &&
				   Name == entry.Name &&
				   Description == entry.Description &&
				   Intruder == entry.Intruder &&
				   Target == entry.Target &&
				   IsConfident == entry.IsConfident &&
				   IsIntegrity == entry.IsIntegrity &&
				   IsAvailability == entry.IsAvailability;
		}
	}
}
