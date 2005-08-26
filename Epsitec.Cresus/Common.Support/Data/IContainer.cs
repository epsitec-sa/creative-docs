//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
