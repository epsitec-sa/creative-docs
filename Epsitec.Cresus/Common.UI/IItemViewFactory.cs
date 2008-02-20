//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>IItemViewFactory</c> interface can be used to initialize <see cref="ItemView"/>
	/// instances based on the item which should be represented.
	/// </summary>
	public interface IItemViewFactory
	{
		/// <summary>
		/// Creates the user interface for the specified item view.
		/// </summary>
		/// <param name="itemView">The item view.</param>
		/// <returns>The widget which represents the data stored in the item view.</returns>
		ItemViewWidget CreateUserInterface(ItemView itemView);

		/// <summary>
		/// Disposes the user interface created by <c>CreateUserInterface</c>.
		/// </summary>
		/// <param name="widget">The widget to dispose.</param>
		void DisposeUserInterface(ItemViewWidget widget);

		/// <summary>
		/// Gets the preferred size of the user interface associated with the
		/// specified item view.
		/// </summary>
		/// <param name="itemView">The item view.</param>
		/// <returns>The preferred size.</returns>
		Drawing.Size GetPreferredSize(ItemView itemView);
	}
}
