using System;
using System.Linq;

namespace ShutdownTimeList
{
	class Program
	{
		static void Main(string[] args)
		{
			// ShutdownTimeList.exe					今月のPCシャットダウン時刻列挙
			// ShutdownTimeList.exe [year] [month]	指定した年月のシャットダウン時刻列挙
			// ShutdownTimeList.exe [month]			今年で指定した月のシャットダウン時刻列挙
			// ShutdownTimeList.exe [負数]			-1のとき先月のPCシャットダウン時刻列挙

			// 空の辞書を作っておく
			var dic = Enumerable.Range(0, 0).Select(x => new { Day = 0, Begin = DateTime.Now, End = DateTime.Now }).ToDictionary(x => x.Day);

			var dt_month_begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
			if (args.Length == 2) {
				dt_month_begin = new DateTime(int.Parse(args[0]), int.Parse(args[1]), 1);
			}
			else if (args.Length == 1) {
				int month = int.Parse(args[0]);
				if (month > 0) {
					dt_month_begin = new DateTime(DateTime.Now.Year, month, 1);
				}
				else {
					dt_month_begin = dt_month_begin.AddMonths(month);
				}
			}
			var dt_month_end = dt_month_begin.AddMonths(1);
			var logs = System.Diagnostics.EventLog.GetEventLogs();
			foreach (var log in logs) {
				if (log.Log != "System") continue;
				foreach (System.Diagnostics.EventLogEntry entry in log.Entries) {
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

			foreach (var item in items) {
				Console.WriteLine($"{item.Begin.Year:0000}/{item.Begin.Month:00}/{item.Day:00}  {item.Begin.Hour:00}:{item.Begin.Minute:00} - {item.End.Hour:00}:{item.End.Minute:00}");
			}

			Console.WriteLine();
		}
	}
}
