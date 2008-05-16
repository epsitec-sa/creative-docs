//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// L'interface IWidgetCollectionHost permet d'offrir le support pour
	/// la classe WidgetCollection.
	/// </summary>
	public interface IWidgetCollectionHost<T> where T : Widget
	{
		void NotifyInsertion(T widget);
		void NotifyRemoval(T widget);
		void NotifyPostRemoval(T widget);
		
		WidgetCollection<T> GetWidgetCollection();
	}
}
