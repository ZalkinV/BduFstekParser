using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BduFstekParser
{
	public class EntryDiff
	{
		public ThreatEntry Before { get; set; }
		public ThreatEntry After { get; set; }

		public EntryDiff() { }

		public EntryDiff(ThreatEntry before, ThreatEntry after)
		{
			Before = before;
			After = after;
		}
	}
}
