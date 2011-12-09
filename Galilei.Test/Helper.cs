using System;
using System.Net;

using Galilei;
using Galilei.Core;

namespace Galilei.Test
{
	public class Helper
	{
		public static Server InitServer(int port)
		{
			Server srv = new Server();
			srv.Types.Add(typeof(TestNode));
			
			srv.Port = port;
			srv["/"] = new Node("node_1");
			srv["/node_1"] = new Node("node_2");
			srv["/"] = new TestNode("test_node");
			
			return srv;
		}
		public static HttpWebResponse Get(string url)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			request.Method = "GET";

			return tryResponse(request);
		}
		
		public static HttpWebResponse Post(string url, string parametrs)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			request.Method = "POST";
			
			request.ContentLength = System.Text.Encoding.UTF8.GetBytes(parametrs).Length;
			WriteToStream (request.GetRequestStream(), parametrs);
			
			return tryResponse(request);
		}
		
		public static HttpWebResponse Put(string url, string parametrs)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			request.Method = "PUT";
			
			request.ContentLength = request.ContentLength = System.Text.Encoding.UTF8.GetBytes(parametrs).Length;
			WriteToStream (request.GetRequestStream(), parametrs);
			
			return tryResponse(request);
		}
		
		public static HttpWebResponse Delete(string url)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			request.Method = "DELETE";

			return tryResponse(request);
		}
		
		public static void WriteToStream(System.IO.Stream stream, string data)
		{
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data);
			
			stream.Write(buffer, 0, buffer.Length);
			stream.Close();

		}
		
		public static string ReadFromStream(System.IO.Stream stream, long count)
		{
			byte[] buffer = new byte[count];
			stream.Read(buffer, 0, (int)count);
			stream.Close();
			return System.Text.Encoding.UTF8.GetString(buffer);
		}

		static HttpWebResponse tryResponse(HttpWebRequest request)
		{
			try {
			 	return(HttpWebResponse)request.GetResponse();
			} 
			catch (WebException e) {
				return (HttpWebResponse)e.Response;
			}
		}
	}
}

