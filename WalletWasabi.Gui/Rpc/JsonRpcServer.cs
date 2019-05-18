using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WalletWasabi.Gui.Rpc
{
	public class JsonRpcServer
	{
		private long _running;
		public bool IsRunning => Interlocked.Read(ref _running) == 1;
		public bool IsStopping => Interlocked.Read(ref _running) == 2;
		private CancellationTokenSource cts { get; }

		private HttpListener _server;
		
		public JsonRpcServer()
		{
			_server = new HttpListener();
			_server.Prefixes.Add("http://127.0.0.1:18099/");
			_server.Prefixes.Add("http://localhost:18099/");
		}

		public void Start()
		{
			Interlocked.Exchange(ref _running, 1);
			_server.Start();

			Task.Run(async ()=>{
				try
				{
					var service = new WasabiJsonRpcService();
					var handler = new JsonRpcRequestHandler(service);

					while (IsRunning)
					{
						var context = _server.GetContext();
						var request = context.Request;
						var response = context.Response;

						//if(request.HttpMethod != "post" || !request.HasEntityBody) // error
						string body;
						using(var reader = new StreamReader(request.InputStream))
							body = await reader.ReadToEndAsync();

						var result = await handler.HandleAsync(body, cts);
						
						if(!string.IsNullOrEmpty(result))
						{
							var output = response.OutputStream;
							var buffer = Encoding.UTF8.GetBytes(result);
							await output.WriteAsync(buffer, 0, buffer.Length);
						}

						context.Response.Close();
					}
				}
				finally
				{
					Interlocked.CompareExchange(ref _running, 3, 2); // If IsStopping, make it stopped.
				}
			});
		}

		internal void Stop()
		{
			Interlocked.CompareExchange(ref _running, 2, 1); // If running, make it stopping.;
			cts.Cancel();
			while (IsStopping)
			{
				Task.Delay(50).GetAwaiter().GetResult();
			}
			cts.Dispose();
		}
	}
}