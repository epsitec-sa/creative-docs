//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

[assembly:DependencyClass (typeof (SlimField))]

namespace Epsitec.Common.Widgets
{
	public partial class SlimField
	{
		private class MenuItem
		{
			public MenuItem(Font font, string text)
			{
				this.font = font;
				this.text = text;
			}

			public MenuItem(SlimFieldMenuItem item)
			{
				this.font = SlimField.GetMenuItemFont (item);
				this.item = item;
			}

			
			public SlimFieldMenuItemHilite		Hilite
			{
				get
				{
					return this.item == null ? SlimFieldMenuItemHilite.None : this.item.Hilite;
				}
			}

			public EnableState					Enable
			{
				get
				{
					return this.item == null ? EnableState.Undefined : this.item.Enable;
				}
			}

			public Color						Color
			{
				get
				{
					if (this.item == null)
					{
						return SlimField.Colors.TextColor;
					}
					else
					{
						return this.item.Enable == EnableState.Enabled ? SlimField.Colors.TextColor : SlimField.Colors.DisabledColor;
					}
				}
			}

			public SlimFieldMenuItemStyle		Style
			{
				get
				{
					return this.item == null ? SlimFieldMenuItemStyle.Extra : this.item.Style;
				}
			}

			public Font							Font
			{
				get
				{
					return this.font;
				}
			}

			public string						Text
			{
				get
				{
					return this.text ?? this.GetText (this.variant);
				}
			}

			public double						TextAdvance
			{
				get
				{
					return this.Font.GetTextAdvance (this.Text);
				}
			}

			public string						Prefix
			{
				get;
				set;
			}

			public double						PrefixAdvance
			{
				get
				{
					if ((this.variant < 0) ||
						(string.IsNullOrEmpty (this.Prefix)))
					{
						return 0;
					}
					else
					{
						return SlimField.Fonts.TextFont.GetTextAdvance (this.Prefix);
					}
				}
			}

			public SlimFieldMenuItem			Item
			{
				get
				{
					return this.item;
				}
			}

			public int							VariantCount
			{
				get
				{
					return this.item == null ? 1 : this.item.Texts.Count;
				}
			}


			public void SelectVariant(int variant)
			{
				this.variant = variant;
			}

			public string GetText(int variant)
			{
				if (variant < 0)
				{
					return "";
				}

				var texts = this.item.Texts;
				var count = texts.Count;

				if (variant < count)
				{
					return texts[variant];
				}
				else
				{
					return texts[count-1];
				}
			}

			public double GetTextAdvance(int variant)
			{
				return this.Font.GetTextAdvance (this.GetText (variant));
			}



			private readonly SlimFieldMenuItem	item;
			private readonly Font				font;
			private readonly string				text;
			private int							variant;
		}
	}
}
