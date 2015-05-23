using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fusens
{
	internal sealed class Util
	{
		private Util()
		{

		}

		public static object Head(ICollection list)
		{
			foreach (object e in list)
				return e;
			return null;
		}

		public static object TryPop(object unknown)
		{
			if (!(unknown is ICollection))
				return null;
			ICollection list = (ICollection)unknown;
			if (list.Count == 0)
				return null;
			return Head(list);
		}

		public static int ParseInt(object unknown)
		{
			if (unknown == null)
				return 0;
			try
			{
				return int.Parse("" + unknown);
			}
			catch
			{
				return 0;
			}
		}

		public static byte[] bytes(string s)
		{
			return System.Text.Encoding.UTF8.GetBytes("" + s);
		}

		public static string ToJson(System.Collections.IDictionary dict)
		{
			string json_query = Newtonsoft.Json.JsonConvert.SerializeObject(dict);
			return json_query;
		}
	}
}
