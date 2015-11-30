using System;
using System.Linq;

namespace ShutdownTimeList
{
	class Program
	{
		static void Main(string[] args)
		{
			var dic = Enumerable.Range(0, 0).Select(x => new { Day = 0, Begin = DateTime.Now, End = DateTime.Now }).ToDictionary(x => x.Day);

			var dt_month_begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
			var dt_month_end = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1);
			var logs = System.Diagnostics.EventLog.GetEventLogs();
			foreach(var log in logs) {
				if (log.Log != "System") continue;
				foreach(System.Diagnostics.EventLogEntry entry in log.Entries) {
					var dt = entry.TimeGenerated;
					if (dt_month_begin <= dt && dt < dt_month_end) {
						if (!dic.ContainsKey(dt.Day)) {
							dic.Add(dt.Day, new { Day = dt.Day, Begin = dt, End = dt });
						}
						var val = dic[dt.Day];
						if (dt < val.Begin) { val = new { Day = val.Day, Begin = dt, End = val.End }; }
						if (val.End < dt) { val = new { Day = val.Day, Begin = val.Begin, End = dt }; }
						dic[dt.Day] = val;
                    }
				}
			}

			var items = dic.Values.ToList();
			items.Sort((a, b) => { return a.Day - b.Day; });

			foreach(var item in items) {
				Console.WriteLine($"{item.Day:00}  {item.Begin.Hour:00}:{item.Begin.Minute:00} - {item.End.Hour:00}:{item.End.Minute:00}");
			}

			Console.WriteLine();
		}
	}
}
