using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Text.Exchange
{
	class StyleDefinition
	{
		public StyleDefinition(string caption, int ident, TextStyleClass textStyleClass, string[] baseStyleCaptions, string serialized, bool isDefaultStyle)
		{
			this.caption = caption;
			this.ident = ident;
			this.textStyleClass = textStyleClass;
			this.baseStyleCaptions = baseStyleCaptions;
			this.serialized = serialized;
			this.isDefaultStyle = isDefaultStyle;
		}

		private TextStyleClass textStyleClass;

		public TextStyleClass TextStyleClass
		{
			get
			{
				return textStyleClass;
			}
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

		private bool isDefaultStyle;

		public bool IsDefaultStyle
		{
			get
			{
				return isDefaultStyle;
			}
		}

		private int ident;

		public int Ident
		{
			get
			{
				return ident;
			}
		}
	}
}
