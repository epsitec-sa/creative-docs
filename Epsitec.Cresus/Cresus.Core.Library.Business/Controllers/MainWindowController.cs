//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class MainWindowController : CoreViewController, IWidgetUpdater, ICoreManualComponent, ICoreComponentHost<MainWindowComponent>
	{
		public MainWindowController(DataViewOrchestrator orchestrator)
			: base ("MainWindow", orchestrator)
		{
			this.app = orchestrator.Host;

			this.components = new CoreComponentHostImplementation<MainWindowComponent> ();
			this.data           = this.app.FindComponent<CoreData> ();
			this.commandContext = this.app.CommandContext;

			Factories.MainWindowComponentFactory.RegisterComponents (this);
			
			this.mainViewController = this.Orchestrator.MainViewController;

			Library.UI.UpdateRequested += sender => this.Update ();
		}


		public CoreApp Host
		{
			get
			{
				return this.app;
			}
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			foreach (var component in this.components.GetComponents ())
			{
				yield return component;
			}
			
			yield return this.mainViewController;
		}

		public override void CreateUI(Widget container)
		{
			base.CreateUI (container);
			this.CreateUIRootBoxes (container);
			this.CreateUIControllers ();
		}

		private void CreateUIRootBoxes(Widget container)
		{
			this.ribbonBox = new FrameBox ()
			{
				Parent = container,
				Name = "RibbonBox",
				Dock = DockStyle.Top,
			};

			this.contentBox = new FrameBox ()
			{
				Parent = container,
				Name = "ContentBox",
				Dock = DockStyle.Fill,
			};
		}

		private void CreateUIControllers()
		{
			Factories.MainWindowComponentFactory.SetupComponents (this.components.GetComponents ());

			foreach (var component in this.components.GetComponents ())
			{
				//	TODO: clean up hack -- ribbon box should not be created in the main window controller !
				component.CreateUI (this.ribbonBox);
			}
			
			this.mainViewController.CreateUI (this.contentBox);
		}


		#region IWidgetUpdater Members

		public void Update()
		{
			this.mainViewController.GetSubControllers ().Select (x => x as IWidgetUpdater).Where (x => x != null).ForEach (x => x.Update ());
		}

		#endregion

		#region ICoreComponentHost<MainWindowComponent> Members

		public T GetComponent<T>()
			where T : MainWindowComponent
		{
			return this.components.GetComponent<T> ();
		}

		MainWindowComponent ICoreComponentHost<MainWindowComponent>.GetComponent(System.Type type)
		{
			return this.components.GetComponent (type);
		}

		IEnumerable<MainWindowComponent> ICoreComponentHost<MainWindowComponent>.GetComponents()
		{
			return this.components.GetComponents ();
		}

		public bool ContainsComponent<T>()
			where T : MainWindowComponent
		{
			return this.components.ContainsComponent<T> ();
		}

		bool ICoreComponentHost<MainWindowComponent>.ContainsComponent(System.Type type)
		{
			return this.components.ContainsComponent (type);
		}

		void ICoreComponentHost<MainWindowComponent>.RegisterComponent<T>(T component)
		{
			this.components.RegisterComponent<T> (component);
		}

		void ICoreComponentHost<MainWindowComponent>.RegisterComponent(System.Type type, MainWindowComponent component)
		{
			this.components.RegisterComponent (type, component);
		}

		void ICoreComponentHost<MainWindowComponent>.RegisterComponentAsDisposable(System.IDisposable component)
		{
			this.components.RegisterComponentAsDisposable (component);
		}

		#endregion

		private readonly CoreComponentHostImplementation<MainWindowComponent> components;

		private readonly CoreApp				app;
		private readonly CoreData				data;
		private readonly MainViewController		mainViewController;
		private readonly CommandContext			commandContext;
		
		private FrameBox						ribbonBox;
		private FrameBox						contentBox;
	}
}
