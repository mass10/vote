using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace fusens.Models
{
	public sealed class Content
	{
		[Key]
		public int ContentId { get; set; }
		public string GUID { get; set; }
		public string Text { get; set; }
		public int Count { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
