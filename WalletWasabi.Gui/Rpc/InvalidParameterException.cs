using System;
using System.Runtime.Serialization;

namespace WalletWasabi.Gui.Rpc
{
	[Serializable]
	internal class InvalidParameterException : Exception
	{
		public InvalidParameterException()
		{
		}

		public InvalidParameterException(string message) : base(message)
		{
		}

		public InvalidParameterException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}