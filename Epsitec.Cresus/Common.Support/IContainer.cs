//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 08/12/2003

namespace Epsitec.Common.Support
{
	public interface IContainer
	{
		ComponentCollection Components { get; }
		
		void NotifyComponentInsertion(ComponentCollection collection, IComponent component);
		void NotifyComponentRemoval(ComponentCollection collection, IComponent component);
	}
}
