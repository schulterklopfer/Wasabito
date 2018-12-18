using NBitcoin.BouncyCastle.Math;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;

namespace WalletWasabi.Backend.Models.Requests
{
	public class OutputRequest
	{
		[Required]
		public string OutputAddress { get; set; }

		[Required]
		public WrappedBlindSignature Signature { get; set; }

		public StringContent ToHttpStringContent()
		{
			string jsonString = JsonConvert.SerializeObject(this, Formatting.None);
			return new StringContent(jsonString, Encoding.UTF8, "application/json");
		}
	}

	public class WrappedBlindSignature
	{
		[Required]
		public string C { get; set; }

		[Required]
		public string S { get; set; }

		public BlindSignature Unwrap()
		{
			return new BlindSignature(new BigInteger(C), new BigInteger(S));
		}

		public static WrappedBlindSignature Wrap(BlindSignature blindSignature)
		{
			return new WrappedBlindSignature { C = blindSignature.C.ToString(), S = blindSignature.S.ToString() };
		} 
	} 
}
