//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 08/12/2003

namespace Epsitec.Common.Support.Data
{
	public interface IComponent : System.IDisposable
	{
		event System.EventHandler	Disposed;
	}
}
