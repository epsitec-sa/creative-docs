using Epsitec.Common.Support;

using System.Text;


namespace Epsitec.Common.Text
{


	public sealed class EncodingHelper
	{


		public EncodingHelper(Encoding encoding)
		{
			this.encoding = Encoding.GetEncoding
			(
				encoding.CodePage,
				new EncoderReplacementFallback (EncodingHelper.replacement),
				new DecoderReplacementFallback (EncodingHelper.replacement)
			);
		}


		public string ConvertToEncoding(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return value;
			}

			var builder = new StringBuilder (value.Length);

			foreach (var c in value)
			{
				var s = c.ToString ();

				if (!this.IsWithinEncoding (s))
				{
					s = StringUtils.RemoveDiacritics (s);
				}

				if (this.IsWithinEncoding (s))
				{
					builder.Append (s);
				}
			}

			return builder.ToString ();
		}


		public bool IsWithinEncoding(string value)
		{
			var convertedBytes = this.encoding.GetBytes (value);
			var convertedValue = this.encoding.GetString (convertedBytes);

			return value == convertedValue;
		}


		private readonly Encoding encoding;


		private static readonly string replacement = "";


	}


}
