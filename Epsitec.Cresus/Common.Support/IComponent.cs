namespace Epsitec.Common.Support
{
	public interface IComponent : System.IDisposable
	{
		event System.EventHandler	Disposed;
	}
}
