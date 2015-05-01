using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Parse;

namespace fusens.Controllers
{
	public class HomeController : AsyncController //Controller
	{
		//public ActionResult Index()
		//{
		//	this._init_parse();
		//	return View();
		//}

		public ActionResult Index()
		{
			this._init_parse();
			return View();
		}

		public ActionResult About()
		{
			this._init_parse();
			ViewBag.Message = "Your application description page.";
			return View();
		}

		public ActionResult Contact()
		{
			this._init_parse();
			ViewBag.Message = "Your contact page.";
			return View();
		}

		private void _init_parse()
		{
			ParseClient.Initialize("JycUHdjGxwuARBrDtJSD2yptpesBxyQDgzfN2aDE", "TblUMNuA3fSoguGs7QA7jPNyIx40qfA3fslpc89t");
		}

		public ActionResult Regist(string new_tag_name)
		{
			this._init_parse();

			//ViewBag.Message = "Your contact page.";

	
			
			if (new_tag_name == null)
				return this.Redirect("/");
			if (new_tag_name == "")
				return this.Redirect("/");

			if (new_tag_name.StartsWith("/del "))
			{
				GlobalQueue.delete(new_tag_name.Substring(5));
			}
			else if (new_tag_name.StartsWith("/delete "))
			{
				GlobalQueue.delete(new_tag_name.Substring(8));
			}
			else
			{
				GlobalQueue.push(new_tag_name);
			}
			return this.Redirect("/");
		}

		public ActionResult Vote(string tag)
		{
			this._init_parse();
			GlobalQueue.push(tag);
			return this.Redirect("/");
		}

		public ActionResult Delete(string tag)
		{
			this._init_parse();
			ViewBag.Message = "removed.";
			GlobalQueue.delete(tag);
			return this.Redirect("/");
		}
	}
}