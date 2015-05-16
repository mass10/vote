using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Parse;
using Newtonsoft.Json;

namespace fusens
{
	public sealed class GlobalQueue
	{
		private static readonly string TABLE_NAME = "TestObject";

		static GlobalQueue()
		{
		}

		private GlobalQueue()
		{
		}

		private static System.Data.DataTable _create_schema()
		{
			System.Data.DataTable t = new System.Data.DataTable();
			t.Columns.Add("objectId", typeof(string));
			t.Columns.Add("tag", typeof(string));
			t.Columns.Add("count", typeof(int));
			t.PrimaryKey = new System.Data.DataColumn[] { t.Columns["objectId"] };
			t.AcceptChanges();
			return t;
		}

		private static object _head(System.Collections.ICollection list)
		{
			foreach (object e in list)
				return e;
			return null;
		}

		private static System.Data.DataTable _extract(object unknown)
		{
			if(!(unknown is System.Collections.ICollection))
				return null;

			System.Collections.ICollection list = (System.Collections.ICollection)unknown;
			if(list.Count == 0)
				return null;

			object first_entity = _head(list);
			if(!(first_entity is Newtonsoft.Json.Linq.JProperty))
				return null;

			Newtonsoft.Json.Linq.JProperty property = (Newtonsoft.Json.Linq.JProperty)first_entity;
			if (property.Name != "results")
				return null;
			
			System.Data.DataTable t = _create_schema();
			
			foreach (var e in property.Value)
			{
				int count = Util.ParseInt("" + e["count"]);
				string content = "" + e["content"];
				if (content == "")
					continue;
				
				System.Data.DataRow row = t.NewRow();
				row["objectId"] = "" + e["objectId"];
				row["tag"] = content;
				row["count"] = count;
				t.Rows.Add(row);
				//Debug.WriteLine("FETCH: CONTENT=[" + content + "], COUNT=[" + count + "]");
			}

			t.AcceptChanges();

			return t;
		}

		public static System.Data.DataRow[] EnumAll()
		{
			System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
			client.DefaultRequestHeaders.Add("X-Parse-Application-Id", "JycUHdjGxwuARBrDtJSD2yptpesBxyQDgzfN2aDE");
			client.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", "Kz7JypdKJWxdHa8sGtUPIJpZdOl1GKb8nlCERSnV");
			System.Net.Http.HttpResponseMessage response = client.GetAsync("https://api.parse.com/1/classes/TestObject").GetAwaiter().GetResult();

			string all_content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
			Debug.WriteLine(all_content);

			var json_tree = JsonConvert.DeserializeObject(all_content);
			System.Data.DataTable t = _extract(json_tree);
			if(t == null)
				return new System.Data.DataRow[] { };
			return t.Select("", "count desc");
		}

		public static async Task<System.Data.DataRow[]> rows()
		{
			ParseClient.Initialize("JycUHdjGxwuARBrDtJSD2yptpesBxyQDgzfN2aDE", "TblUMNuA3fSoguGs7QA7jPNyIx40qfA3fslpc89t");

			System.Data.DataTable t = _create_schema();

			//
			// クエリ(全行)
			//
			ParseQuery<ParseObject> query = new ParseQuery<ParseObject>(TABLE_NAME);
			IEnumerable<ParseObject> result = await query.FindAsync().ConfigureAwait(false);
			foreach (var e in result)
			{
				string content = "";
				int count = 0;
				e.TryGetValue("content", out content);
				if (content == null)
					continue;
				if (content == "")
					continue;
				e.TryGetValue("count", out count);
				System.Data.DataRow row = t.NewRow();
				row["tag"] = content;
				row["count"] = count;
				t.Rows.Add(row);
			}

			t.AcceptChanges();
			return t.Select("", "count desc");
		}

		public static void delete(string key)
		{
			_delete(key);
		}

		private static void CreateNew(string content)
		{
			string url = "https://api.parse.com/1/classes/TestObject";
			System.Net.HttpWebRequest c = System.Net.HttpWebRequest.CreateHttp(url);
			c.Method = "POST";
			c.ContentType = "application/json";
			c.Headers.Add("X-Parse-Application-Id", "JycUHdjGxwuARBrDtJSD2yptpesBxyQDgzfN2aDE");
			c.Headers.Add("X-Parse-REST-API-Key", "Kz7JypdKJWxdHa8sGtUPIJpZdOl1GKb8nlCERSnV");

			System.Collections.Hashtable t = new System.Collections.Hashtable();
			t["content"] = content;
			t["count"] = 1;
			string json_query = JsonConvert.SerializeObject(t);

			// request
			System.IO.Stream request_stream = c.GetRequestStream();
			//System.IO.StreamWriter writer = new System.IO.StreamWriter(request_stream);
			byte[] form_data = Util.bytes(json_query);
			request_stream.Write(form_data, 0, form_data.Length);
			//writer.Write(json_query);
			request_stream.Close();

			// reading response...
			System.Net.WebResponse response = c.GetResponse();
			System.IO.Stream stream = response.GetResponseStream();
			System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);
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

		private static void Put(string object_id, string content, int count)
		{
			//System.Web.HttpRequest request = new System.Web.HttpRequest();
			//System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
			//client.DefaultRequestHeaders.Add("X-Parse-Application-Id", "JycUHdjGxwuARBrDtJSD2yptpesBxyQDgzfN2aDE");
			//client.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", "Kz7JypdKJWxdHa8sGtUPIJpZdOl1GKb8nlCERSnV");
			//client.DefaultRequestHeaders.Add("Content-Type", "application/json");
			//System.Net.Http.HttpResponseMessage response = client.GetAsync("https://api.parse.com/1/classes/TestObject/" + object_id).GetAwaiter().GetResult();

			if (object_id == null || object_id == "")
			{
				CreateNew(content);
				return;
			}

			string url = "https://api.parse.com/1/classes/TestObject/" + object_id;
			System.Net.HttpWebRequest c = System.Net.HttpWebRequest.CreateHttp(url);
			c.Method = "PUT";
			c.ContentType = "application/json";
			c.Headers.Add("X-Parse-Application-Id", "JycUHdjGxwuARBrDtJSD2yptpesBxyQDgzfN2aDE");
			c.Headers.Add("X-Parse-REST-API-Key", "Kz7JypdKJWxdHa8sGtUPIJpZdOl1GKb8nlCERSnV");
			string json_query = "{\"count\": " + count + "}";

			// request
			System.IO.Stream request_stream = c.GetRequestStream();
			//System.IO.StreamWriter writer = new System.IO.StreamWriter(request_stream);
			byte[] form_data = Util.bytes(json_query);
			request_stream.Write(form_data, 0, form_data.Length);
			//writer.Write(json_query);
			request_stream.Close();

			// reading response...
			System.Net.WebResponse response = c.GetResponse();
			System.IO.Stream stream = response.GetResponseStream();
			System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);
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

		private static void _push(string requested_keyword)
		{
			if (requested_keyword == null)
				return;
			if (requested_keyword == "")
				return;

			// クエリ(全行)
			foreach(System.Data.DataRow row in EnumAll())
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

			if (true)
				return;

			ParseQuery<ParseObject> query = new ParseQuery<ParseObject>(TABLE_NAME);
			IEnumerable<ParseObject> result = query.FindAsync().GetAwaiter().GetResult();
			foreach (var e in result)
			{
				string content = "";
				int count = 0;
				e.TryGetValue("content", out content);
				e.TryGetValue("count", out count);
				if (content == requested_keyword)
				{
					// 同一の行がみつかれば点数を加算して終了
					e["count"] = count + 1;
					e.SaveAsync().GetAwaiter().GetResult();
					return;
				}
			}

			//
			// 見つからなかった場合は新しい行を作成
			//
			{
				var new_record = new ParseObject(TABLE_NAME);
				new_record["content"] = requested_keyword;
				new_record["count"] = 1;
				new_record.SaveAsync().GetAwaiter().GetResult();
			}
		}

		private static void _delete(string requested_keyword)
		{
			if (requested_keyword == null)
				return;
			if (requested_keyword == "")
				return;

			//
			// クエリ(全行)
			//
			ParseQuery<ParseObject> query = new ParseQuery<ParseObject>(TABLE_NAME);
			IEnumerable<ParseObject> result = query.FindAsync().GetAwaiter().GetResult();
			foreach (var e in result)
			{
				string content = "";
				int count = 0;
				e.TryGetValue("content", out content);
				e.TryGetValue("count", out count);
				if (content == requested_keyword)
				{
					// 同一の行がみつかれば削除して終了
					e.DeleteAsync();
					return;
				}
			}
		}

		private static readonly object MUTEX = "Parse オブジェクトの排他用...";

		public static void push(string content)
		{
			Debug.WriteLine("インクリメント: キーワード=[" + content + "]");
			_push(content);
		}
	}
}
