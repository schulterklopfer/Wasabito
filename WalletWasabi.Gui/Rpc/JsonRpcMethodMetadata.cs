using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WalletWasabi.Gui.Rpc
{
	///<summary>
	/// Represents the collection of metadata needed to execute the remote procedure.
	///</summary>
	public class JsonRpcMethodMetadata
	{
		// The name of the remote procedure. This is NOT the name of the method to be invoked.
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