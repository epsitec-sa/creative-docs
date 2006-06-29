using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Text.Exchange
{
	class StyleDefinition
	{
		public StyleDefinition(string caption, string[] baseStyleCaptions, string serialized)
		{
			this.caption = caption;
			this.baseStyleCaptions = baseStyleCaptions;
			this.serialized = serialized;
		}

		private string caption;

		public string Caption
		{
			get
			{
				return caption;
			}
		}


		private string serialized;

		public string Serialized
		{
			get
			{
				return serialized;
			}
		}


		private string[] baseStyleCaptions;

		public string[] BaseStyleCaptions
		{
			get
			{
				return baseStyleCaptions;
			}
		}
	}
}
