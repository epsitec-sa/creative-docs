//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 08/12/2003

namespace Epsitec.Common.Support
{
	public interface IComponent : System.IDisposable
	{
		event System.EventHandler	Disposed;
	}
}
