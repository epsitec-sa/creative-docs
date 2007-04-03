//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.UI;

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemPanelNavigator</c> class implements the keyboard navigation
	/// through an <see cref="ItemPanel"/>.
	/// </summary>
	public class ItemPanelNavigator
	{
		internal ItemPanelNavigator(ItemPanel rootPanel)
		{
			System.Diagnostics.Debug.Assert (rootPanel.IsRootPanel);

			this.rootPanel = rootPanel;
		}

		public void NavigateTo(ItemView view)
		{

		}


		delegate T Transform<T>(Widgets.Widget widget, T t);

		private Drawing.Point MapFromRoot(ItemPanel container, Drawing.Point position)
		{
			return this.MapCoordinates (container, position, MapMode.FromRoot,
				delegate (Widgets.Widget widget, Drawing.Point pos)
				{
					return widget.MapParentToClient (pos);
				});
		}

		private Drawing.Rectangle MapFromRoot(ItemPanel container, Drawing.Rectangle bounds)
		{
			return this.MapCoordinates (container, bounds, MapMode.FromRoot,
				delegate (Widgets.Widget widget, Drawing.Rectangle rect)
				{
					return widget.MapParentToClient (rect);
				});
		}

		private Drawing.Point MapToRoot(ItemPanel container, Drawing.Point position)
		{
			return this.MapCoordinates (container, position, MapMode.ToRoot,
				delegate (Widgets.Widget widget, Drawing.Point pos)
				{
					return widget.MapClientToParent (pos);
				});
		}

		private Drawing.Rectangle MapToRoot(ItemPanel container, Drawing.Rectangle bounds)
		{
			return this.MapCoordinates (container, bounds, MapMode.ToRoot,
				delegate (Widgets.Widget widget, Drawing.Rectangle rect)
				{
					return widget.MapClientToParent (rect);
				});
		}

		private enum MapMode
		{
			ToRoot,
			FromRoot,
		}

		private T MapCoordinates<T>(ItemPanel container, T coord, MapMode mode, Transform<T> transform)
		{
			List<Widgets.Widget> widgets = new List<Widgets.Widget> ();

			while (container != null)
			{
				if (container == this.rootPanel)
				{
					if (mode == MapMode.FromRoot)
					{
						widgets.Reverse ();
					}

					foreach (Widgets.Widget widget in widgets)
					{
						coord = transform (widget, coord);
					}

					return coord;
				}

				ItemPanelGroup group = container.ParentGroup;

				if (group == null)
				{
					break;
				}

				widgets.Add (container);
				widgets.Add (group);

				container = group.ItemView.Owner;
			}

			throw new System.InvalidOperationException ("Panel does not have a common root.");
		}


		ItemPanel rootPanel;
		ItemView currentView;

		double currentX;
		double currentY;
		bool isCurrentXValid;
		bool isCurrentYValid;
	}
}
