//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support.Data
{
	public interface IComponent : System.IDisposable
	{
		event Support.EventHandler	Disposed;
	}
}
