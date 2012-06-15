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
		private class MenuLayout
		{
			public MenuLayout(SlimField host)
			{
				this.host  = host;
				this.items = new List<MenuItem> (MenuLayout.GetMenuItems (this.host.MenuItems));
				this.count = MenuLayout.GetMenuItemsTextVariantCount (this.items);
			}


			public IEnumerable<MenuItem> Items
			{
				get
				{
					return this.items;
				}
			}
			
			public SlimFieldMenuItem DetectMenuItem(double advance)
			{
				advance = advance / Font.DefaultFontSize;

				foreach (var item in this.items)
				{
					double prefix = item.PrefixAdvance;
					double width  = item.GetTextAdvance ();

					if ((advance <= width) &&
						(advance >= prefix))
					{
						return item.Item;
					}

					advance -= prefix + width;
				}

				return null;
			}

			private static IEnumerable<MenuItem> GetMenuItems(IEnumerable<SlimFieldMenuItem> collection)
			{
				bool first = true;

				foreach (var item in collection)
				{
					var menuItem = new MenuItem (item);

					if (first)
					{
						first = false;
					}
					else
					{
						menuItem.Prefix = SlimField.Strings.MenuSeparator;
					}

					yield return menuItem;
				}
			}

			private static int GetMenuItemsTextVariantCount(IEnumerable<MenuItem> collection)
			{
				return collection.Max (x => x.VariantCount);
			}

			private int SelectMenuItemsTextVariant(double maxWidth)
			{
				for (int i = 0; i < count; i++)
				{
					double valueWidth = this.MeasureMenuItems (i, this.items.Where (x => x.Style == SlimFieldMenuItemStyle.Value));
					double extraWidth = this.MeasureMenuItems (i, this.items.Where (x => x.Style == SlimFieldMenuItemStyle.Extra));

					if (valueWidth + extraWidth < maxWidth)
					{
						return i;
					}
				}

				return count-1;
			}

			private double MeasureMenuItems(int variant, IEnumerable<MenuItem> collection)
			{
				return collection.Sum (x => x.GetTextAdvance (variant)) * Font.DefaultFontSize;
			}

			private readonly SlimField host;
			private readonly List<MenuItem> items;
			private readonly int count;
		}
	}
}
