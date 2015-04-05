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
			t.AcceptChanges();
			return t;
		}

		private static void _fill_sample_items(System.Data.DataTable t)
		{
			System.Data.DataRow row = t.NewRow();
			row["tag"] = "クラウド";
			row["count"] = 1;
			t.Rows.Add(row);
			t.AcceptChanges();
		}

		public static ICollection<string> tags()
		{
			SortedSet<string> response = new SortedSet<string>();

			foreach (System.Data.DataRow row in _get_table().Rows)
			{
				response.Add("" + row["tag"]);
			}
	
			return response;
		}

		public static System.Data.DataRowCollection rows()
		{
			return _get_table().Rows;
		}

		private static System.Data.DataTable _get_table()
		{
			return _table;
		}

		public static void push(string new_tag)
		{
			if (new_tag == null)
				return;
			if (new_tag == "")
				return;




			System.Data.DataTable t = _get_table();
	
			foreach (System.Data.DataRow row in t.Rows)
			{
				string tag = "" + row["tag"];
				if (tag == new_tag)
				{
					row["count"] = (int)row["count"] + 1;
					return;
				}
			}

			{
				System.Data.DataRow row = t.NewRow();
				row["tag"] = new_tag;
				row["count"] = 1;
				t.Rows.Add(row);
				t.AcceptChanges();
			}
		}

		public static void delete(string new_tag)
		{
			if (new_tag == null)
				return;
			if (new_tag == "")
				return;
			
			
			
			
			System.Data.DataTable t = _get_table();

			foreach (System.Data.DataRow row in t.Rows)
			{
				string tag = "" + row["tag"];
				if(tag == new_tag)
				{
					t.Rows.Remove(row);
					t.AcceptChanges();
					return;
				}
			}			
		}
	}
}
