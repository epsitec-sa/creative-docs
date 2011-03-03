//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>CoreComponentHostImplementation</c> class provides a standard implementation
	/// of <see cref="ICoreComponentHost"/>.
	/// </summary>
	/// <typeparam name="TComponent">The base type of all the <see cref="ICoreComponent"/> derived components.</typeparam>
	public sealed class CoreComponentHostImplementation<TComponent> : ICoreComponentHost<TComponent>
		where TComponent : class, ICoreComponent
	{
		public CoreComponentHostImplementation()
		{
			this.components = new Dictionary<string, TComponent> ();
		}

		#region ICoreComponentHost<TComponent> Members

		public T GetComponent<T>()
			where T : TComponent
		{
			TComponent component;

			if (this.components.TryGetValue (typeof (T).FullName, out component))
			{
				return (T) component;
			}

			throw new System.ArgumentException (string.Format ("The specified component {0} does not exist", typeof (T).FullName));
		}

		public IEnumerable<TComponent> GetComponents()
		{
			return this.components.Select (x => x.Value);
		}

		public bool ContainsComponent<T>()
			where T : TComponent
		{
			return this.components.ContainsKey (typeof (T).FullName);
		}

		public void RegisterComponent<T>(T component)
			where T : TComponent
		{
			this.components[typeof (T).FullName] = component;
		}

		#endregion

		public void RegisterComponents(IEnumerable<TComponent> components)
		{
			foreach (var component in components)
			{
				this.components[component.GetType ().FullName] = component;
			}
		}
		
		
		private readonly Dictionary<string, TComponent> components;
	}
}
