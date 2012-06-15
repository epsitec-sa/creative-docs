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

			public MenuLayout Update(double maxWidth)
			{
				this.SelectMenuItemsTextVariant (maxWidth / Font.DefaultFontSize);
				
				return this;
			}
			
			public SlimFieldMenuItem DetectMenuItem(double advance)
			{
				advance = advance / Font.DefaultFontSize;

				foreach (var item in this.items)
				{
					double prefix = item.PrefixAdvance;
					double width  = item.TextAdvance;

					if ((advance <= width) &&
						(advance >= prefix))
					{
						return item.Item;
					}

					advance -= prefix + width;
				}

				return null;
			}


			/// <summary>
			/// Gets a collection of <see cref="MenuItem"/>s, based on the <see cref="SlimField"/>
			/// collection of menu items; this will generate items with separations.
			/// </summary>
			/// <param name="collection">The <see cref="SlimField"/> collection of menu items.</param>
			/// <returns>The collection of <see cref="MenuItem"/>s.</returns>
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

			private void SelectMenuItemsTextVariant(double maxWidth)
			{
				int bestTextCount   = 0;
				int bestTextVariant = 0;

				this.items.ForEach (x => x.SelectVariant (0));

				var valueItems = this.items.Where (x => x.Style == SlimFieldMenuItemStyle.Value).ToArray ();
				var fixedItems = this.items.Where (x => x.Style == SlimFieldMenuItemStyle.Extra || x.Style == SlimFieldMenuItemStyle.Symbol).ToArray ();
				
				for (int i = 0; i < count; i++)
				{
					var valueWidths = this.MeasureMenuItems (i, valueItems);
					var fixedWidth  = this.MeasureMenuItems (i, fixedItems).Sum ();

					int numTextCount = MenuLayout.GetMaxWidthCount (maxWidth - fixedWidth, valueWidths);

					if (numTextCount > bestTextCount)
					{
						bestTextVariant = i;
						bestTextCount   = numTextCount;
					}
				}

				for (int i = 0; i < bestTextCount; i++)
				{
					valueItems[i].SelectVariant (bestTextVariant);
				}
				for (int i = bestTextCount; i < valueItems.Length; i++)
				{
					valueItems[i].SelectVariant (-1);
				}

				fixedItems.ForEach (x => x.SelectVariant (bestTextVariant));
			}

			private static int GetMaxWidthCount(double maxWidth, IEnumerable<double> widths)
			{
				var count = 0;
				var total = 0.0d;

				foreach (var width in widths)
				{
					total += width;
					
					if (total > maxWidth)
					{
						break;
					}

					count++;
				}

				return count;
			}

			private IEnumerable<double> MeasureMenuItems(int variant, IEnumerable<MenuItem> collection)
			{
				return collection.Select (x => x.PrefixAdvance + x.GetTextAdvance (variant));
			}

			private readonly SlimField host;
			private readonly List<MenuItem> items;
			private readonly int count;
		}
	}
}
