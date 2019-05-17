using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WalletWasabi.Gui.Rpc
{
	public class JsonRpcServer
	{
		private HttpListener _server;
		
		public JsonRpcServer()
		{
			_server = new HttpListener();
			_server.Prefixes.Add("http://127.0.0.1/");
			_server.Prefixes.Add("http://localhost/");
		}

		public void Start()
		{
			_server.Start();
			Task.Run(async ()=>{
				var service = new WasabiJsonRpcService();
				var handler = new JsonRpcRequestHandler(service);

				while (true)
				{
					var context = _server.GetContext();
					var request = context.Request;
					var response = context.Response;

					//if(request.HttpMethod != "post" || !request.HasEntityBody) // error
					var reader = new StreamReader(request.InputStream);
					var body = await reader.ReadToEndAsync();
					var result = await handler.HandleAsync(body);
					
					if(!string.IsNullOrEmpty(result))
					{
						var output = response.OutputStream;
						var buffer = Encoding.UTF8.GetBytes(result);
						await output.WriteAsync(buffer, 0, buffer.Length);
					}
					context.Response.Close();
				}
			});
		}
	}
}