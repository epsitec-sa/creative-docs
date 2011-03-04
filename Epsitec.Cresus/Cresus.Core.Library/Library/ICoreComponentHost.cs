//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Text;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>ICoreComponentHost</c> interface provides the glue to bind components with
	/// a host. The component classes implement <see cref="ICoreComponent"/>.
	/// </summary>
	/// <typeparam name="TComponent">The type of the component.</typeparam>
	public interface ICoreComponentHost<TComponent>
		where TComponent : class, ICoreComponent
	{
		/// <summary>
		/// Gets the component of the specified type. This should throw an exception
		/// if the component cannot be found.
		/// </summary>
		/// <typeparam name="T">The specific type of the component.</typeparam>
		/// <returns></returns>
		T GetComponent<T>()
			where T : TComponent;

		TComponent GetComponent(System.Type type);

		/// <summary>
		/// Gets a collection of all components.
		/// </summary>
		/// <returns>The collection of all components.</returns>
		IEnumerable<TComponent> GetComponents();

		/// <summary>
		/// Determines whether this host contains the specified component.
		/// </summary>
		/// <typeparam name="T">The specific type of the component.</typeparam>
		/// <returns>
		///   <c>true</c> if this host contains the specified component; otherwise, <c>false</c>.
		/// </returns>
		bool ContainsComponent<T>()
			where T : TComponent;

		bool ContainsComponent(System.Type type);

		/// <summary>
		/// Registers the specified component with the host.
		/// </summary>
		/// <typeparam name="T">The specific type of the component.</typeparam>
		/// <param name="component">The component.</param>
		void RegisterComponent<T>(T component)
			where T : TComponent;

		void RegisterComponent(System.Type type, TComponent component);
		
		/// <summary>
		/// Registers the component as a disposable component.
		/// </summary>
		/// <param name="component">The component.</param>
		void RegisterComponentAsDisposable(System.IDisposable component);
	}
}
