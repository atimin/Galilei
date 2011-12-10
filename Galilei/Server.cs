using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;

using Galilei.Core;

namespace Galilei
{
	[Node]
	public class Server : Root
	{	
		private HttpListener listener;
		private string host;
		private int port;
		private Configurator config;
		private Thread workFlow;
		private List<Type> xpcaTypes;
		
		public Server() : base()
		{
			host = "127.0.0.1";
			port = 3001;
			
			GetTypes();
			
			config = new Configurator("config.json", this);
			
			listener = new HttpListener();
			workFlow = new Thread(new ThreadStart(WorkFlow));
		}
		
		[Property]
		public string Version {
			get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}
		
		[Property]
		public string ServerName {
			get { return Assembly.GetExecutingAssembly().GetName().Name; }
		}
		
		[Property]
		public int Port
		{
			get { return port; }
			set { port = value; }
		}
		
		[Property]
		public string[] XpcaTypes
		{
			get 
			{ 
				return Array.ConvertAll<Type, string>(Types.ToArray(),delegate(Type t) {
					return t.Name;
				});
			}
		}
		
		public List<Type> Types
		{
			get { return xpcaTypes;	} 
		}
		
		
		
		public void Start()			
		{	
			string prefix = String.Format("http://{0}:{1}/", host, port);
			
			listener.Prefixes.Clear();
			listener.Prefixes.Add(prefix);
			
			listener.Start();
			Console.WriteLine("Start listener on " + prefix);
			workFlow.Start();
			Console.WriteLine("Start server");
		}
		
		public void Stop()
		{
			Console.WriteLine("Stop server");
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
   			RestController controller = new RestController(context, this);
			
			DateTime t = DateTime.Now;
			Console.WriteLine("Request: {0} : {1}",
				context.Request.HttpMethod, 
				context.Request.RawUrl);
			
    		controller.Process();
			
			Console.WriteLine("Response: {0} : {1} [{2} ms]",
				context.Response.StatusCode,
				context.Response.StatusDescription,
				(DateTime.Now - t).Milliseconds
			);
		}		

		void GetTypes()
		{
			xpcaTypes = new List<Type>(Assembly.GetExecutingAssembly().GetTypes());
			
			string[] assemblies = Directory.GetFiles(Directory.GetCurrentDirectory(), "Galilei.*.dll");
			
			
			foreach (string a in assemblies){
				Console.WriteLine("Load types form {0}", a);
				xpcaTypes.AddRange(Assembly.LoadFile(a).GetTypes());
			}
			
			xpcaTypes.RemoveAll(delegate(Type type) {
				object o = Array.Find(type.GetCustomAttributes(false), delegate(object attr) {
					return attr is NodeAttribute;
				});
				
				return o == null;
			});
			
			Console.WriteLine("Found XPCA types:");
			foreach (Type t in xpcaTypes) {
				Console.WriteLine(t.Name);
			}
		}
	}
}