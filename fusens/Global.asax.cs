using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Parse;
using System.Diagnostics;

namespace fusens
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
			ParseClient.Initialize("JycUHdjGxwuARBrDtJSD2yptpesBxyQDgzfN2aDE", "TblUMNuA3fSoguGs7QA7jPNyIx40qfA3fslpc89t");
			//DumpAll2();
        }

		private static void DumpAll2()
		{
			ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("TestObject");
			Debug.WriteLine("<DumpAll2()> 抽出中...");
			IEnumerable<ParseObject> result = query.FindAsync().GetAwaiter().GetResult();
			Debug.WriteLine("<DumpAll2()> 抽出OK。表示を始めます！");
			foreach (var e in result)
			{
				string content = "";
				int count = 0;
				e.TryGetValue("content", out content);
				e.TryGetValue("count", out count);
				Debug.WriteLine("<DumpAll2()> content=[" + content + "], count=[" + count + "]");
			}
			Debug.WriteLine("<DumpAll2()> 終了！");
		}
	}
}
