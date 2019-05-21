
namespace WalletWasabi.Gui.Models
{
	public class JsonRpcServerConfiguration
	{
		private Config _config;
		public bool IsEnabled => _config.JsonRpcServerEnabled.Value;
		public int Port => _config.JsonRpcServerPort.Value;

		public JsonRpcServerConfiguration(Config config)
		{
			_config = config;
		}
	}
}
