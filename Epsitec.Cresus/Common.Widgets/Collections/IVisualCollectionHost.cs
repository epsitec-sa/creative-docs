//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// L'interface IVisualCollectionHost définit des méthodes de notification
	/// appelées par la classe VisualCollection. Cette manière de faire est bien
	/// plus efficace que des événements pour signaler des changements.
	/// </summary>
	public interface IVisualCollectionHost
	{
		void NotifyVisualCollectionBeforeInsertion(VisualCollection collection, Visual visual);
		void NotifyVisualCollectionAfterInsertion(VisualCollection collection, Visual visual);
		void NotifyVisualCollectionBeforeRemoval(VisualCollection collection, Visual visual);
		void NotifyVisualCollectionAfterRemoval(VisualCollection collection, Visual visual);
		void NotifyVisualCollectionChanged(VisualCollection collection);
	}
}
