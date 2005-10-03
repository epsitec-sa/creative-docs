//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// IVisualCollectionHost.
	/// </summary>
	public interface IVisualCollectionHost
	{
		void NotifyVisualCollectionInsertion(VisualCollection collection, Visual visual);
		void NotifyVisualCollectionRemoval(VisualCollection collection, Visual visual);
		void NotifyVisualCollectionChanged(VisualCollection collection);
	}
}
