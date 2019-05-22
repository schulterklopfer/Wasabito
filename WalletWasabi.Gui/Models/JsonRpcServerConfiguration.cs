namespace WalletWasabi.Gui.Models
{
	public class JsonRpcServerConfiguration
	{
		private Config _config;
		public bool IsEnabled => _config.JsonRpcServerEnabled.Value;
		public int Port => _config.JsonRpcServerPort.Value;
		public string JsonRpcUser => _config.JsonRpcUser;
		public string JsonRpcPassword => _config.JsonRpcPassword;

		public bool RequiresCredentials => !string.IsNullOrEmpty(JsonRpcUser) && !string.IsNullOrEmpty(JsonRpcPassword);

		public JsonRpcServerConfiguration(Config config)
		{
			_config = config;
		}
	}
}
