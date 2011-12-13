using System;
using System.IO;
using System.Net;

using System.Reflection;
using System.Collections.Generic;

using Galilei.Core;

namespace Galilei
{
	[Node]
	public class Server : Root
	{	

		private string host;
		private int port;
		private Configurator config;
		private List<Type> xpcaTypes;
		
		public Server() : base()
		{
			host = "127.0.0.1";
			port = 3001;
			
			GetTypes();
			
			config = new Configurator("config.json", this);
			

		}
		
		[Property]
		public string Version {
			get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}
		
		[Property]
		public string ServerName {
			get { return Assembly.GetExecutingAssembly().GetName().Name; }
		}
		
		[Config]
		public string Host 
		{
			get { return host; }
			set { host = value; }
		}

		[Config]
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