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

		public ActionResult Regist()
		{
			//ViewBag.Message = "Your contact page.";
			string new_tag_name = "" + this.Request.Form["new_tag_name"];
			GlobalQueue.push(new_tag_name);
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