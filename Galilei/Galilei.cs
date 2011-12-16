using System;
using System.Net;
using System.Web;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using Galilei.Core;

namespace Galilei
{
	public class Galilei
	{
		private Server srv;
		private HttpListener listener;
		private Thread workFlow;
		private Configurator config;
		private TreeBuilder builder;
		private Queue<Node> queueToSave;
		
		public Galilei (Server srv)
		{
			this.srv = srv;					
			config = new Configurator("galilei.conf", srv);
			
			builder = new TreeBuilder(srv);
			builder.ConfigChange += new TreeBuilder.ConfigChangeHandler(OnConfig);
			
			queueToSave = new Queue<Node>();
			
			listener = new HttpListener();
			workFlow = new Thread(new ThreadStart(WorkFlow));
		}
		 
		public void Start()			
		{	
			config.Load();
			Console.WriteLine("Load config");
			
			string prefix = String.Format("http://{0}:{1}/", srv.Host, srv.Port);
			listener.Prefixes.Clear();
			listener.Prefixes.Add(prefix);
			
			listener.Start();
			Console.WriteLine("Start listener on " + prefix);
			workFlow.Start();
			Console.WriteLine("Start Galilei");
		}
		
		public void Stop()
		{
			Console.WriteLine("Stop Galilei");
			workFlow.Abort();
			workFlow.Join();
			
		}
		
		private void WorkFlow()
		{
			IAsyncResult result = null;
			try {
				while(true){
					result = listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
	    			result.AsyncWaitHandle.WaitOne(); 
					
					// Save changes
					if (queueToSave.Count > 0) {
						while(queueToSave.Count > 0) {
							Console.WriteLine("Config changes in node:{0}", 
							                   queueToSave.Dequeue().FullName
							);	
						}
						config.Save();
						Console.WriteLine("Save config");
					}
				}
			}
			catch(ThreadAbortException)
        	{
				Thread.ResetAbort();
        	}
		}
		
		private void ListenerCallback(IAsyncResult result)
		{
			HttpListener listener = (HttpListener) result.AsyncState;
    		// Call EndGetContext to complete the asynchronous operation.
			HttpListenerContext context = listener.EndGetContext(result);
			
			DateTime t = DateTime.Now;
			Console.WriteLine("Request: {0} : {1}",
				context.Request.HttpMethod, 
				context.Request.RawUrl);
			
    		Process(context);
			
			Console.WriteLine("Response: {0} : {1} [{2} ms]",
				context.Response.StatusCode,
				context.Response.StatusDescription,
				(DateTime.Now - t).Milliseconds
			);
		}	
		
		private void Process(HttpListenerContext context)
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
			catch(XpcaTypeError ex) {
				response.StatusCode = 500;
				response.StatusDescription = ex.Message;
			}
			catch(XpcaPathError ex) {
				response.StatusCode = 400;
				response.StatusDescription = ex.Message;
			}
			catch {
				response.StatusCode = 500;

			}
			finally	{
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
		}
				
		void PostRespond (HttpListenerRequest request, HttpListenerResponse response)
		{
			NameValueCollection parms =  GetParams(request);
			try {
				builder.Update(request.RawUrl, parms);
				response.StatusCode = 200;
			} 
			catch (XpcaPathError) {
				builder.Build(request.RawUrl, parms);	
				response.StatusCode = 201;
			}
		}

		void PutRespond (HttpListenerRequest request, HttpListenerResponse response)
		{	
			builder.Update(request.RawUrl, GetParams(request));
			response.StatusCode = 200;
		}

		void DeleteRespond (HttpListenerRequest request, HttpListenerResponse response)
		{
			builder.Delete(request.RawUrl);			
			response.StatusCode = 200;
		}

		
		private NameValueCollection GetParams(HttpListenerRequest request)
		{
			byte[] buffer = new byte[request.ContentLength64];
			request.InputStream.Read(buffer,0,(int)request.ContentLength64);
			
			string data = HttpUtility.UrlDecode(System.Text.Encoding.UTF8.GetString(buffer));
			return HttpUtility.ParseQueryString(data);
		}
		
		private void OnConfig(Node node)
		{
			queueToSave.Enqueue(node);
		}
	}
}

