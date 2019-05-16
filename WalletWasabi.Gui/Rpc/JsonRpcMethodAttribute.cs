using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WalletWasabi.Gui.Rpc
{
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public class JsonRpcMethodAttribute : Attribute
	{
		public string Name { get; } 
		public string Description { get; }
		public JsonRpcMethodAttribute(string name, string description)
		{
			Name = name;
			Description = description;
		}
	}
}