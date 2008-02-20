//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>AbstractObservableList</c> class is used only to make a few
	/// non generic methods available to the generic <c>ObservableList</c>
	/// users, without having to specify a generic type parameter.
	/// </summary>
	public abstract class AbstractObservableList
	{
		/// <summary>
		/// Gets target for the specified collection changing event handler.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The target handler instance or <c>null</c>.</returns>
		public abstract object GetCollectionChangingTarget(int index);

		/// <summary>
		/// Gets target for the specified collection changed event handler.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The target handler instance or <c>null</c>.</returns>
		public abstract object GetCollectionChangedTarget(int index);
	}
}
