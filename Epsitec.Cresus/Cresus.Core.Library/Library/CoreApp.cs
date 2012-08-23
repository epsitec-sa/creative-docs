//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>CoreApp</c> class is the base for the application class (interactive application)
	/// or for the session class (server-based application).
	/// </summary>
	public abstract class CoreApp : Epsitec.Common.Widgets.Application, ICoreComponentHost<CoreAppComponent>, ICoreComponentHost<ICoreManualComponent>
	{
		protected CoreApp()
		{
			System.Diagnostics.Debug.Assert (CoreApp.current == null);

			CoreApp.current = this;

			this.components = new CoreComponentHostImplementation<CoreAppComponent> ();
			this.manualComponents = new CoreComponentHostImplementation<ICoreManualComponent> ();
			this.policy = this.CreateDefaultPolicy ();
			
			Factories.CoreAppComponentFactory.RegisterComponents (this);
		}


		public PersistenceManager				PersistenceManager
		{
			get
			{
				return this.FindComponent<PersistenceManager> ();
			}
		}

		public SettingsManager					SettingsManager
		{
			get
			{
				return this.FindComponent<SettingsManager> ();
			}
		}

		public CoreAppPolicy					Policy
		{
			get
			{
				return this.policy;
			}
		}


		public virtual void SetupApplication()
		{
			Factories.CoreAppComponentFactory.SetupComponents (this.components.GetComponents ());
			CoreContext.ExecutePendingSetupFunctions ();
		}

		public virtual IEnumerable<ICoreComponent> FindAllComponents()
		{
			var compo1 = this.components.GetComponents ().Cast<ICoreComponent> ();
			var compo2 = this.manualComponents.GetComponents ().Cast<ICoreComponent> ();

			return compo1.Concat (compo2).Distinct ();
		}

		public virtual T FindComponent<T>()
			where T : class, ICoreComponent
		{
			ICoreComponentHost<CoreAppComponent> host = this;

			if (host.ContainsComponent (typeof (T)))
			{
				return host.GetComponent (typeof (T)) as T;
			}

			if (this.manualComponents.ContainsComponent (typeof (T)))
			{
				return this.manualComponents.GetComponent (typeof (T)) as T;
			}

			return null;
		}

		public virtual T FindActiveComponent<T>()
			where T : class, ICoreManualComponent
		{
			return this.manualComponents.FindActiveComponent<T> ();
		}

		public void ActivateComponent(ICoreManualComponent component)
		{
			this.manualComponents.ActivateComponent (component);
		}

		public void RegisterComponentAsDisposable(ICoreManualComponent component)
		{
			var disposable = component as System.IDisposable;

			if (disposable == null)
			{
				throw new System.ArgumentException ("The component does not implement IDisposable");
			}

			ICoreComponentHost<ICoreManualComponent> self = this as ICoreComponentHost<ICoreManualComponent>;

			self.RegisterComponentAsDisposable (disposable);
		}


		/// <summary>
		/// Gets the application or session instance, active in the current context.
		/// </summary>
		/// <typeparam name="T">The specific type of <see cref="CoreApp"/> class to retrieve.</typeparam>
		/// <returns>The active application or session instance.</returns>
		public static T GetCurrentAppSession<T>()
			where T : CoreApp
		{
			T session = CoreApp.current as T;

			System.Diagnostics.Debug.Assert (session != null);

			return session;
		}

		/// <summary>
		/// Finds the specified component in the current context (application or session).
		/// </summary>
		/// <typeparam name="T">The component type to retrieve.</typeparam>
		/// <returns>The component or <c>null</c>.</returns>
		public static T FindCurrentAppSessionComponent<T>()
			where T : class, ICoreComponent
		{
			return CoreApp.GetCurrentAppSession<CoreApp> ().FindComponent<T> ();
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

		CoreAppComponent ICoreComponentHost<CoreAppComponent>.GetComponent(System.Type type)
		{
			return this.components.GetComponent (type);
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

		#region ICoreComponentHost<ICoreComponent> Members

		T ICoreComponentHost<ICoreManualComponent>.GetComponent<T>()
		{
			return this.manualComponents.GetComponent<T> ();
		}

		ICoreManualComponent ICoreComponentHost<ICoreManualComponent>.GetComponent(System.Type type)
		{
			return this.manualComponents.GetComponent (type);
		}

		IEnumerable<ICoreManualComponent> ICoreComponentHost<ICoreManualComponent>.GetComponents()
		{
			return this.manualComponents.GetComponents ();
		}

		bool ICoreComponentHost<ICoreManualComponent>.ContainsComponent<T>()
		{
			return this.manualComponents.ContainsComponent<T> ();
		}

		bool ICoreComponentHost<ICoreManualComponent>.ContainsComponent(System.Type type)
		{
			return this.manualComponents.ContainsComponent (type);
		}

		void ICoreComponentHost<ICoreManualComponent>.RegisterComponent(System.Type type, ICoreManualComponent component)
		{
			this.manualComponents.RegisterComponent (type, component);
		}

		public void RegisterComponent<T>(T component)
			where T : ICoreManualComponent
		{
			this.manualComponents.RegisterComponent<T> (component);
		}

		void ICoreComponentHost<ICoreManualComponent>.RegisterComponentAsDisposable(System.IDisposable disposable)
		{
			//	Register the manual component just like any other component, so that it will be
			//	disposed in the proper sequence:

			this.components.RegisterComponentAsDisposable (disposable);
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

		protected virtual CoreAppPolicy CreateDefaultPolicy()
		{
			return new CoreAppPolicy ()
			{
				RequiresCoreCommandHandlers = true,
			};
		}

		[System.ThreadStatic]
		protected static CoreApp				current;

		private readonly CoreComponentHostImplementation<CoreAppComponent> components;
		private readonly CoreComponentHostImplementation<ICoreManualComponent> manualComponents;
		private readonly CoreAppPolicy			policy;
	}
}
