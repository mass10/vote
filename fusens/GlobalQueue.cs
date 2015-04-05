using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fusens
{
	public sealed class GlobalQueue
	{
		private static readonly System.Data.DataTable _table = create_schema();

		static GlobalQueue()
		{
		}

		private GlobalQueue()
		{
		}

		private static System.Data.DataTable create_schema()
		{
			System.Data.DataTable t = new System.Data.DataTable();
			t.Columns.Add("tag", typeof(string));
			t.Columns.Add("count", typeof(int));
			t.PrimaryKey = new System.Data.DataColumn[]{ t.Columns["tag"] };
			return t;
		}

		private static void _fill_sample_items(System.Data.DataTable t)
		{
			System.Data.DataRow row = t.NewRow();
			row["tag"] = "クラウド";
			row["count"] = 0;
			t.Rows.Add(row);
		}

		public static ICollection<string> tags()
		{
			SortedSet<string> response = new SortedSet<string>();
			foreach (System.Data.DataRow row in _table.Rows)
				response.Add("" + row["tag"]);
			return response;
		}
	}
}
