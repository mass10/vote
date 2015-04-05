using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fusens.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";
			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";
			return View();
		}

		public ActionResult Regist(string new_tag_name)
		{
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
			GlobalQueue.push(tag);
			return this.Redirect("/");
		}

		public ActionResult Delete(string tag)
		{
			ViewBag.Message = "removed.";
			GlobalQueue.delete(tag);
			return this.Redirect("/");
		}
	}
}