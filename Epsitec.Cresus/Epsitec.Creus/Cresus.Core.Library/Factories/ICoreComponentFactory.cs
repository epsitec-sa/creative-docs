//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Factories
{
	/// <summary>
	/// The <c>ICoreComponentFactory</c> interface must be implemented by all factory
	/// classes which create components (see <see cref="ICoreComponent"/>) of any kind.
	/// </summary>
	/// <typeparam name="THost">The type of the host.</typeparam>
	/// <typeparam name="TComponent">The type of the component.</typeparam>
	public interface ICoreComponentFactory<THost, TComponent>
		where TComponent : ICoreComponent
	{
		/// <summary>
		/// Determines whether the host is in a sufficiently advanced state, so that the
		/// factory can create the component. This is useful if a component relies on another
		/// combonent to be already present (it will not, however, be fully initialized yet).
		/// </summary>
		/// <param name="host">The host.</param>
		/// <returns>
		///   <c>true</c> if the factory will be able to create the component at this point;
		///   otherwise, <c>false</c>.
		/// </returns>
		bool CanCreate(THost host);

		/// <summary>
		/// Creates a component. This may only be called if <see cref="CanCreate"/> returned
		/// <c>true</c>. See also <see cref="CoreComponentFactory"/>.
		/// </summary>
		/// <param name="host">The host.</param>
		/// <returns>The component.</returns>
		TComponent Create(THost host);

		/// <summary>
		/// Gets the type of the components created by this factory.
		/// </summary>
		/// <returns>The type of the components created by this factory.</returns>
		System.Type GetComponentType();
	}
}
