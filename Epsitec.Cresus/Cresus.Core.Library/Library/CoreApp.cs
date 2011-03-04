//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Library
{
	public abstract class CoreApp : Epsitec.Common.Widgets.Application, ICoreComponentHost<CoreAppComponent>
	{
		protected CoreApp()
		{
			this.components = new CoreComponentHostImplementation<CoreAppComponent> ();
			
			Factories.CoreAppComponentFactory.RegisterComponents (this);
		}

		#region ICoreComponentHost<CoreAppComponent> Members

		public bool ContainsComponent<T>() where T : CoreAppComponent
		{
			return this.components.ContainsComponent<T> ();
		}

		public T GetComponent<T>() where T : CoreAppComponent
		{
			return this.components.GetComponent<T> ();
		}

		IEnumerable<CoreAppComponent> ICoreComponentHost<CoreAppComponent>.GetComponents()
		{
			return this.components.GetComponents ();
		}

		bool ICoreComponentHost<CoreAppComponent>.ContainsComponent(System.Type type)
		{
			return this.components.ContainsComponent (type);
		}

		void ICoreComponentHost<CoreAppComponent>.RegisterComponent<T>(T component)
		{
			this.components.RegisterComponent<T> (component);
		}

		void ICoreComponentHost<CoreAppComponent>.RegisterComponent(System.Type type, CoreAppComponent component)
		{
			this.components.RegisterComponent (type, component);
		}

		void ICoreComponentHost<CoreAppComponent>.RegisterComponentAsDisposable(System.IDisposable component)
		{
			this.components.RegisterComponentAsDisposable (component);
		}

		#endregion


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.components.Dispose ();
			}

			base.Dispose (disposing);
		}

		private readonly CoreComponentHostImplementation<CoreAppComponent> components;
	}
}
