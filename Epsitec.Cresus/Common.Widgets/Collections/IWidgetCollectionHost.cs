//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// L'interface IWidgetCollectionHost permet d'offrir le support pour
	/// la classe WidgetCollection.
	/// </summary>
	public interface IWidgetCollectionHost
	{
		void NotifyInsertion(Widget widget);
		void NotifyRemoval(Widget widget);
		void NotifyPostRemoval(Widget widget);
		
		WidgetCollection GetWidgetCollection();
	}
}
