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
			
			// Get types
			xpcaTypes = new List<Type>();
			xpcaTypes.AddRange(Assembly.GetAssembly(typeof(Node)).GetTypes());
				xpcaTypes.AddRange(Assembly.GetAssembly(typeof(Galilei.Simulator.Simulator)).GetTypes());
			xpcaTypes.AddRange(Assembly.GetExecutingAssembly().GetTypes());
			
			xpcaTypes.RemoveAll(delegate(Type type) {
				object o = Array.Find(type.GetCustomAttributes(false), delegate(object attr) {
					return attr is NodeAttribute;
				});
				
				return o == null;
			});
			
			foreach (Type t in xpcaTypes) {
				Console.WriteLine(t.Name);
			}
			
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
		
		public List<Type> Types
		{
			get 
			{ 
				return xpcaTypes;
			} 
		}
		
		
		
		public void Start()			
		{	
			listener.Prefixes.Clear();
			listener.Prefixes.Add(
				String.Format("http://{0}:{1}/", host, port)
			);
			listener.Start();
			Console.WriteLine("Start listener");
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
			
					Console.WriteLine("Waiting for request to be processed asyncronously.");
	    			result.AsyncWaitHandle.WaitOne();    				
		//			config.Save();
		//			Console.WriteLine("Save config");
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
   			RestController controller = new RestController(listener.EndGetContext(result), this);
    		controller.Process();
		}		
	}
}

