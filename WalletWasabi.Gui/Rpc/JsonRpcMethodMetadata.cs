using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WalletWasabi.Gui.Rpc
{
	public class JsonRpcMethodMetadata
	{
		public string Name { get; }
		public string Description { get;}
		public MethodInfo MethodInfo { get; }
		public List<(string name, Type type)> Parameters { get; }

		public JsonRpcMethodMetadata(string name, string description, MethodInfo mi, List<(string name, Type type)> parameters)
		{
			Name = name;
			Description = description;
			MethodInfo = mi;
			Parameters = parameters;
		}
	}
}