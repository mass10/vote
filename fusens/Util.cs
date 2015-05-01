using System;
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
	}
}
