using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace fusens
{
	public sealed class GlobalQueue
	{
		private GlobalQueue()
		{
		}

		private static DataTable _create_schema()
		{
			DataTable t = new DataTable();
			t.Columns.Add("objectId", typeof(string));
			t.Columns.Add("tag", typeof(string));
			t.Columns.Add("count", typeof(int));
			t.PrimaryKey = new DataColumn[] { t.Columns["objectId"] };
			t.AcceptChanges();
			return t;
		}


		private static DataTable _extract(object unknown)
		{
			object first_entity = Util.TryPop(unknown);
			if(!(first_entity is JProperty))
				return null;

			JProperty property = (JProperty)first_entity;
			if (property.Name != "results")
				return null;
			
			DataTable t = _create_schema();
			
			foreach (var e in property.Value)
			{
				int count = Util.ParseInt("" + e["count"]);
				string content = "" + e["content"];
				if (content == "")
					continue;
				
				DataRow row = t.NewRow();
				row["objectId"] = "" + e["objectId"];
				row["tag"] = content;
				row["count"] = count;
				t.Rows.Add(row);
				//Debug.WriteLine("FETCH: CONTENT=[" + content + "], COUNT=[" + count + "]");
			}

			t.AcceptChanges();

			return t;
		}

		private static string _send_request(string url)
		{
			// 特別なヘッダーを付けて簡単な GET リクエスト
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add(Keys.APP_KEY, _get_applcation_id());
			client.DefaultRequestHeaders.Add(Keys.REST_API_KEY, _get_api_key());
			HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult();
			string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
			Debug.WriteLine(content);
			return content;
		}

		public static DataRow[] EnumAll()
		{
			// リクエスト(全件抽出)
			string all_content = _send_request("https://api.parse.com/1/classes/TestObject");

			// JSON の読み取り
			var json_tree = JsonConvert.DeserializeObject(all_content);
			DataTable t = _extract(json_tree);
	
			// レスポンス
			if(t == null)
				return new DataRow[] { };
			return t.Select("", "count desc");
		}

		public static void Delete(string s)
		{
			if (s == null)
				return;
			if (s == "")
				return;

			// TODO: 削除処理
		}

		private static void CreateNew(string content)
		{
			// ================================================================
			// creating a request...
			// ================================================================
			HttpWebRequest c = _create_requester("https://api.parse.com/1/classes/TestObject");
			c.Method = "POST";
			c.ContentType = "application/json";

			Hashtable t = new Hashtable();
			t["content"] = content;
			t["count"] = 1;
			string json_query = Util.ToJson(t);

			// ================================================================
			// request
			// ================================================================
			{
				Stream request_stream = c.GetRequestStream();
				byte[] form_data = Util.bytes(json_query);
				request_stream.Write(form_data, 0, form_data.Length);
				request_stream.Close();
			}

			// ================================================================
			// reading response...
			// ================================================================
			{
				WebResponse response = c.GetResponse();
				Stream stream = response.GetResponseStream();
				StreamReader reader = new StreamReader(stream, Encoding.UTF8);
				while (true)
				{
					string line = reader.ReadLine();
					if (line == null)
						break;
					Debug.WriteLine("Parse.com からのレスポンス: [" + line + "]");
				}
				stream.Close();
				Debug.WriteLine("新しいレコードを作成しました。content=[" + content + "]");
			}
		}

		private static string _get_applcation_id()
		{
			return "JycUHdjGxwuARBrDtJSD2yptpesBxyQDgzfN2aDE";
		}

		private static string _get_api_key()
		{
			return "Kz7JypdKJWxdHa8sGtUPIJpZdOl1GKb8nlCERSnV";
		}

		private static HttpWebRequest _create_requester(string url)
		{
			HttpWebRequest c = HttpWebRequest.CreateHttp(url);
			c.Headers.Add(Keys.APP_KEY, _get_applcation_id());
			c.Headers.Add(Keys.REST_API_KEY, _get_api_key());
			return c;
		}

		private static void Put(string object_id, string content, int count)
		{
			if (object_id == null || object_id == "")
			{
				CreateNew(content);
				return;
			}

			// ================================================================
			// creating a request...
			// ================================================================
			string url = "https://api.parse.com/1/classes/TestObject/" + object_id;
			HttpWebRequest c = _create_requester(url);
			c.Method = "PUT";
			c.ContentType = "application/json";

			Hashtable t = new Hashtable();
			t["count"] = count;
			string json_query = Util.ToJson(t);

			// ================================================================
			// sending request...
			// ================================================================
			{
				Stream request_stream = c.GetRequestStream();
				byte[] form_data = Util.bytes(json_query);
				request_stream.Write(form_data, 0, form_data.Length);
				request_stream.Close();
			}

			// ================================================================
			// reading response...
			// ================================================================
			{
				WebResponse response = c.GetResponse();
				Stream stream = response.GetResponseStream();
				StreamReader reader = new StreamReader(stream, Encoding.UTF8);
				while (true)
				{
					string line = reader.ReadLine();
					if (line == null)
						break;
					Debug.WriteLine("Parse.com からのレスポンス: [" + line + "]");
				}
				stream.Close();
				Debug.WriteLine("レコードを更新しました。content=[" + content + "], count=[" + count + "]");
			}
		}

		private static void _push(string requested_keyword)
		{
			if (requested_keyword == null)
				return;
			if (requested_keyword == "")
				return;

			// クエリ(全行)
			foreach(DataRow row in EnumAll())
			{
				string content = "" + row["tag"];
				if (content != requested_keyword)
					continue;
				string object_id = "" + row["objectId"];
				int count = Util.ParseInt(row["count"]);
				Put(object_id, content, count + 1);
				return;
			}

			Debug.WriteLine("キーワード [" + requested_keyword + "] は存在しません。");
			
			Put(null, requested_keyword, 1);
		}

		private static readonly object MUTEX = "Parse オブジェクトの排他用...";

		public static void push(string content)
		{
			Debug.WriteLine("インクリメント: キーワード=[" + content + "]");
			_push(content);
		}

		private static class Keys
		{
			public static string APP_KEY = "X-Parse-Application-Id";
			public static string REST_API_KEY = "X-Parse-REST-API-Key";
		};
	}
}
