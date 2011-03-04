using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Factories
{
	/// <summary>
	/// The <c>ICoreAppComponentFactory</c> interface must be implemented by all factories
	/// which provide <see cref="CoreAppComponent"/> instances for <see cref="CoreApp"/>.
	/// </summary>
	public interface ICoreAppComponentFactory : ICoreComponentFactory<CoreApp, CoreAppComponent>
	{
	}
}
