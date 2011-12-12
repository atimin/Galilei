using System;
using System.Net;
using System.Web;
using System.Collections.Specialized;
using System.Reflection;

using Galilei.Core;

namespace Galilei
{
	public class RestController
	{
		private HttpListenerContext context;
		private Server srv;
		
		public RestController (HttpListenerContext context, Server srv)
		{
			this.context = context;
			this.srv = srv;
		}
		
		public void Process()
		{
			HttpListenerRequest request = context.Request;
    		// Obtain a response object.
    		HttpListenerResponse response = context.Response;
			try {
				switch (request.HttpMethod) {
					case "GET":
						GetRespond (request, response);
						break;								
					case "POST":
						PostRespond (request, response);
						break;
					case "PUT":	
						PutRespond (request, response);
						break;
					case "DELETE":
						DeleteRespond (request, response);
						break;
					default:
					break;
				}
			}
			catch 
			{
				response.StatusCode = 500;
				response.Close();
			}
		}
		
		void GetRespond (HttpListenerRequest request, HttpListenerResponse response)
		{
			string[] rawUrl = request.RawUrl.Split('.');
			string url = rawUrl[0];
			string format = "json";
			if (rawUrl.Length == 2) {
				format = rawUrl[1];
			}
						
			Node node = srv[url];
			
			if (node == null)
			{
				response.StatusCode = 400;
				response.Close();
				return;
			}
			
			Serializer serializer = null;
			switch (format) {
			case "json":
				serializer = new JsonSerializer(node);
				response.ContentType = "application/json";

			break;
			case "xml":
				serializer = new XmlSerializer(node);
				response.ContentType = "application/xml";
			break;
			default:
				response.StatusCode = 400;
			break;
			}
			
			if (response.StatusCode == 200) {
				string data = serializer.Serialize();
				byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data);
				response.ContentLength64 = buffer.Length;
				response.OutputStream.Write(buffer, 0, buffer.Length);
				response.OutputStream.Flush();
				response.OutputStream.Close();
			}
			
			response.Close();
		}
				
		void PostRespond (HttpListenerRequest request, HttpListenerResponse response)
		{
			NameValueCollection parms = GetParams(request);
			
			Node node = srv[request.RawUrl];
			if (node == null) {
				// Make new node
				string[] splitUrl = request.RawUrl.Split('/');
				string name = splitUrl[splitUrl.Length -1];
				string parentPath = String.Join("/", splitUrl, 0, splitUrl.Length - 1);
				Node parent = srv[parentPath];
				
				if (parent != null) {
					if (parms["type"] == null) {
						node = new Node(name);
					} 
					else {					
							Type type = srv.Types.Find(delegate (Type t) {
								return t.Name == parms["type"];
							});
						if (type != null) {
							node = Activator.CreateInstance(type, new object[] {}) as Node;
						} 
						else {
							response.StatusCode = 500;
							response.StatusDescription = "Type `" + parms["type"] +"` is not supported";
							response.Close();
							return;
						}
					}
				
					parms.Add("name", name);
					parms.Add("parent", "xpca:/" + parent.FullName);
					response.StatusCode = 201;
				} 
				else {
					response.StatusCode = 400;
					response.Close();
					return;
				}
				
			}
		
			UpdateNode(parms, node);
										
			if (response.StatusCode != 201)
				response.StatusCode = 200;
			
			response.Close();
		}

		void PutRespond (HttpListenerRequest request, HttpListenerResponse response)
		{
			Node node = srv[request.RawUrl];
			if (node == null) {
				response.StatusCode = 400;
				response.Close();
				return;				
			}
			
			UpdateNode(GetParams(request), node);

			response.Close();
		}

		void DeleteRespond (HttpListenerRequest request, HttpListenerResponse response)
		{
			Node node = srv[request.RawUrl];
			
			if (node != null) {
				XpcaProxy proxy = new XpcaProxy(node);
				proxy["parent"] = null;				
				response.StatusCode = 200;
			}
			else {
				response.StatusCode = 400;
			}
			
			response.Close();
		}

		void UpdateNode(NameValueCollection parms, Node node)
		{
			XpcaProxy proxy = new XpcaProxy(node);
			foreach (string name in parms.AllKeys) {
				if (proxy.Properties.ContainsKey(name)) {
					Type type = proxy.Properties[name].PropertyType;
					object val = parms[name];
					// Get node by ref
					if (typeof(Node).IsAssignableFrom(type)) {
						val = srv[val.ToString().Replace("xpca:/","")];
					}
					else {
						try {
							val = type.InvokeMember("Parse", 
								BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public,
								null,
								type,
								new object[] {val});
						}
						catch {
							val = val.ToString();	
						}
					}
					
					proxy[name] = val;
				}
			}
		}		
		
		private NameValueCollection GetParams(HttpListenerRequest request)
		{
			byte[] buffer = new byte[request.ContentLength64];
			request.InputStream.Read(buffer,0,(int)request.ContentLength64);
			
			string data = HttpUtility.UrlDecode(System.Text.Encoding.UTF8.GetString(buffer));
			return HttpUtility.ParseQueryString(data);
		}
		
	}
}

