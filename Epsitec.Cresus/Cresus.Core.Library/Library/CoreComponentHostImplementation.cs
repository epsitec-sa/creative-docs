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
	public sealed class CoreComponentHostImplementation<TComponent> : ICoreComponentHost<TComponent>, System.IDisposable
		where TComponent : class, ICoreComponent
	{
		public CoreComponentHostImplementation()
		{
			this.components = new Dictionary<string, TComponent> ();
			this.registeredComponents = new List<TComponent> ();
			this.disposableComponents = new Stack<System.IDisposable> ();
			this.activeComponents = new List<TComponent> ();
		}

		public void ActivateComponent(TComponent component)
		{
			System.Diagnostics.Debug.Assert (this.registeredComponents.Contains (component));

			this.activeComponents.Remove (component);
			this.activeComponents.Insert (0, component);
		}

		public T FindActiveComponent<T>()
			where T : TComponent
		{
			return this.activeComponents.OfType<T> ().FirstOrDefault ();
		}
		
		public void RegisterComponents(IEnumerable<TComponent> components)
		{
			foreach (var component in components)
			{
				this.RegisterComponent (component.GetType (), component);
			}
		}

		#region ICoreComponentHost<TComponent> Members

		public T GetComponent<T>()
			where T : TComponent
		{
			return (T) this.GetComponent (typeof (T));
		}

		public TComponent GetComponent(System.Type type)
		{
			TComponent component;

			if (this.components.TryGetValue (type.FullName, out component))
			{
				return component;
			}

			throw new System.ArgumentException (string.Format ("The specified component {0} does not exist", type.FullName));
		}

		public IEnumerable<TComponent> GetComponents()
		{
			return this.registeredComponents;
		}

		public bool ContainsComponent<T>()
			where T : TComponent
		{
			return this.ContainsComponent (typeof (T));
		}

		public void RegisterComponent<T>(T component)
			where T : TComponent
		{
			this.RegisterComponent (typeof (T), component);
		}

		public bool ContainsComponent(System.Type type)
		{
			return this.components.ContainsKey (type.FullName);
		}

		public void RegisterComponent(System.Type type, TComponent component)
		{
			this.registeredComponents.Add (component);
			this.components[type.FullName] = component;
		}

		public void RegisterComponentAsDisposable(System.IDisposable component)
		{
			this.disposableComponents.Push (component);
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			while (this.disposableComponents.Count > 0)
			{
				this.disposableComponents.Pop ().Dispose ();
			}
		}

		#endregion
		
		
		private readonly Dictionary<string, TComponent>	components;
		private readonly List<TComponent>				registeredComponents;
		private readonly Stack<System.IDisposable>		disposableComponents;
		private readonly List<TComponent>				activeComponents;
	}
}
