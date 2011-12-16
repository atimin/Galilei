using System;
using System.Collections.Specialized;
using System.Reflection;

namespace Galilei.Core
{
	public class TreeBuilder
	{
		private bool hasConfigChanges;
		private Server srv;
		
		public delegate void ConfigChangeHandler(Node node);
		public event ConfigChangeHandler ConfigChange;
		
		
		public TreeBuilder(Server srv)
		{
			this.srv = srv;
		}
		
		public void Build(string fullName, NameValueCollection parms)
		{
			Node node;
			string[] splitUrl = fullName.Split('/');
			string name = splitUrl[splitUrl.Length -1];
			string parentPath = String.Join("/", splitUrl, 0, splitUrl.Length - 1);
			
			if (parms["type"] == null) {
				node = new Node();
			} 
			else {					
					Type type = srv.Types.Find(delegate (Type t) {
						return t.Name == parms["type"];
					});
				if (type != null) {
					node = Activator.CreateInstance(type, new object[] {}) as Node;
				} 
				else {
					throw new XpcaTypeError(parms["type"]);
				}
			}
		
			node.Name = name;
			srv[parentPath] = node;
			hasConfigChanges = true;
			
			Update(fullName, parms);
		}

		public void Update(string fullname, NameValueCollection parms)
		{
			XpcaProxy proxy = new XpcaProxy(srv[fullname]);
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
			
			proxy.ConfigChange -= NodeConfig;
			OnConfig(proxy.Node);
		}
		
		public void Delete(string fullName)
		{
			Node node = srv[fullName];
			srv[fullName] = null;
			hasConfigChanges = true;
			OnConfig(node);
		}
		
		private void OnConfig(Node node)
		{
			if (ConfigChange != null) {
				ConfigChange(node);
				hasConfigChanges = false;
			}
		}
		
		private void NodeConfig(object sender, ChangeEventArgs e)
		{
			hasConfigChanges = true;
		}
	}
}

