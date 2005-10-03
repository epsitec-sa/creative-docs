//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// IVisualCollectionHost.
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
