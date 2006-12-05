//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>IItemViewFactory</c> interface can be used to initialize <see cref="ItemView"/>
	/// instances based on the item which should be represented.
	/// </summary>
	public interface IItemViewFactory
	{
		Widgets.Widget CreateUserInterface(ItemPanel panel, ItemView itemView);
		void DisposeUserInterface(ItemView itemView, Widgets.Widget widget);
		Drawing.Size GetPreferredSize(ItemPanel panel, ItemView itemView);
	}
}
