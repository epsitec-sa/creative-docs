//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support.Data
{
	public interface IContainer
	{
		ComponentCollection Components { get; }
		
		void NotifyComponentInsertion(ComponentCollection collection, IComponent component);
		void NotifyComponentRemoval(ComponentCollection collection, IComponent component);
	}
}
