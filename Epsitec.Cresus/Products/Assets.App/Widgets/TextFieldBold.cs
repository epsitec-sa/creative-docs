//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Types.Converters;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class TextFieldBold : TextField
	{
		public bool Bold
		{
			get
			{
				return this.bold;
			}
			set
			{
				if (this.bold != value)
				{
					this.bold = value;
					this.Invalidate ();
				}
			}
		}


		protected override TextLayout GetPaintTextLayout()
		{
			var original = this.TextLayout;

			if (this.Bold)
			{
				return TextFieldBold.GetPaintTextLayoutBold (original);
			}

			return original;
		}

		private static TextLayout GetPaintTextLayoutBold(TextLayout original)
		{
			var text = TextConverter.ConvertToSimpleText (original.Text);

			return new TextLayout (original)
			{
				Text = string.Concat ("<b>", TextConverter.ConvertToTaggedText (text), "</b>")
			};
		}


		private bool bold;
	}
}
