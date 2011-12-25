using System;
using System.Net;

using Galilei.Core;

namespace Galilei
{
	public class Helper
	{
		public delegate void ResponseHandler(HttpWebResponse response);
		
		public static void Get(string url, ResponseHandler handler)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			request.Method = "GET";

			HttpWebResponse response = TryResponse(request);
			handler(response);
			response.Close();
		}
		
		public static void Post(string url, string parametrs, ResponseHandler handler)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			request.Method = "POST";
			
			request.ContentLength = System.Text.Encoding.UTF8.GetBytes(parametrs).Length;
			WriteToStream (request.GetRequestStream(), parametrs);
			
			HttpWebResponse response = TryResponse(request);
			handler(response);
			response.Close();
		}
		
		public static void Put(string url, string parametrs, ResponseHandler handler) 
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			request.Method = "PUT";
			
			request.ContentLength = request.ContentLength = System.Text.Encoding.UTF8.GetBytes(parametrs).Length;
			WriteToStream (request.GetRequestStream(), parametrs);
			
			HttpWebResponse response = TryResponse(request);
			handler(response);
			response.Close();
		}
		
		public static void Delete(string url, ResponseHandler handler)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			request.Method = "DELETE";

			HttpWebResponse response = TryResponse(request);
			handler(response);
			response.Close();
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

		static HttpWebResponse TryResponse(HttpWebRequest request)
		{
			try {
			 	return (HttpWebResponse)request.GetResponse();
			} 
			catch (WebException e) {
				return (HttpWebResponse)e.Response;
			}
		}
	}
}

