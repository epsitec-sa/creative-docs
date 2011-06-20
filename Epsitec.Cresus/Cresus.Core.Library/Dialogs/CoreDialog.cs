//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// The <c>CoreDialog</c> class implements the basic functionality for dialogs used
	/// in the Core projects (window persistence and basic command handling capabilities).
	/// </summary>
	public abstract class CoreDialog : AbstractDialog
	{
		protected CoreDialog(CoreApp app)
		{
			this.application = app;
			this.persisted = new List<DependencyObject> ();
		}

		
		private PersistenceManager PersistenceManager
		{
			get
			{
				return this.application.PersistenceManager;
			}
		}

		
		protected sealed override Window CreateWindow()
		{
			Window window = new Window ()
			{
				Name = this.GetType ().FullName,
				Icon = this.application.Window.Icon,
			};

			this.OwnerWindow = this.application.Window;

			this.SetupWindow (window);
			this.SetupWidgets (window);

			this.AutoCenterDialog = !this.PersistenceManager.Register (window);

			this.UpdateWidgets ();
			this.RegisterController (this);

			window.AdjustWindowSize ();

			return window;
		}

		protected void RegisterWithPersistenceManager(dynamic widget)
		{
			this.PersistenceManager.Register (widget);
			this.persisted.Add (widget);
		}

		protected override void OnDialogClosed()
		{
			foreach (var item in this.persisted)
			{
				this.application.PersistenceManager.Unregister (item);
			}

			this.application.PersistenceManager.Unregister (this.DialogWindow);

			base.OnDialogClosed ();
		}


		protected abstract void SetupWindow(Window window);
		protected abstract void SetupWidgets(Window window);

		protected virtual void UpdateWidgets()
		{
		}

		protected readonly CoreApp application;
		private readonly List<DependencyObject> persisted;
	}
}
