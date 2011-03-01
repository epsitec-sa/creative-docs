//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	public class CoreComponentHostImplementation<TComponent> : ICoreComponentHost<TComponent>
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
			return (T) this.components[typeof (T).FullName];
		}

		public IEnumerable<TComponent> GetComponents()
		{
			return this.components.Select (x => x.Value);
		}

		public bool ContainsComponent<T>() where T : TComponent
		{
			return this.components.ContainsKey (typeof (T).FullName);
		}

		public void RegisterComponent<T>(T component) where T : TComponent
		{
			this.components[component.GetType ().FullName] = component;
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
