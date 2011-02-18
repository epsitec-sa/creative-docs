//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// The <c>IWidgetCollectionHost</c> interface provides support for the
	/// generic <see cref="WidgetCollection"/> class.
	/// </summary>
	/// <typeparam name="T">Widget type.</typeparam>
	public interface IWidgetCollectionHost<T> where T : Widget
	{
		void NotifyInsertion(T widget);
		void NotifyRemoval(T widget);
		void NotifyPostRemoval(T widget);
		
		WidgetCollection<T> GetWidgetCollection();
	}
}
