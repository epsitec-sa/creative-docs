//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Cresus.Core.PlugIns;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.CorePlugIn.TableDesigner
{
	[PlugIn ("TableDesigner", "1.0")]
	public class TableDesignerPlugIn : ICorePlugIn
	{
		public TableDesignerPlugIn(PlugInFactory factory)
		{
			this.factory = factory;
			this.application = this.factory.Application;
			this.orchestrator = this.application.MainWindowOrchestrator;

			this.application.CreatedUI       += sender => System.Diagnostics.Debug.WriteLine ("EVENT: Application finished creating the UI");
			this.application.ShutdownStarted += sender => System.Diagnostics.Debug.WriteLine ("EVENT: Application shutting down");

			this.orchestrator.SettingActiveEntity += this.HandleOrchestratorSettingActiveEntity;
			CommandDispatcher.CommandDispatching  += this.HandleCommandDispatcherCommandDispatching;
		}

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion

		private void HandleOrchestratorSettingActiveEntity(object sender, ActiveEntityCancelEventArgs e)
		{
			var entityKey = e.EntityKey;
			var navigationPathElement = e.NavigationPathElement;

			if (entityKey.HasValue)
			{
				if (this.isDesignerDisabled)
				{
					this.DisposeTableDesigner ();
				}

				var key = entityKey.Value;

				if (key.EntityId == EntityInfo<PriceCalculatorEntity>.GetTypeId ())
				{
					this.orchestrator.ClearActiveEntity ();

					var businessContext = this.orchestrator.DefaultBusinessContext;
					var priceCalculator = businessContext.SetActiveEntity<PriceCalculatorEntity> (entityKey, navigationPathElement);

					System.Diagnostics.Debug.WriteLine ("EVENT: Edit price calculator table <" + priceCalculator.Name + ">");

					this.CreateTableDesigner (businessContext, priceCalculator, navigationPathElement);

					e.Cancel = true;
				}
				else
				{
					this.DisposeTableDesigner ();
				}
			}
			else
			{
				//	We may not dispose the designer, since it might still be needed in the very
				//	near future, for instance to save the design of the table. This is why we
				//	simply disable the designer for now :

				this.DisableTableDesigner ();
			}
		}

		private void HandleCommandDispatcherCommandDispatching(object sender, CommandEventArgs e)
		{
			if (this.activeDesigner != null)
			{
				if (e.Command == ApplicationCommands.New)
				{
					//	No need to do anything: the WorkflowDesigner intercepts the save event generated
					//	by the active BusinessContext.
				}
			}
		}

		private void CreateTableDesigner(Core.Business.BusinessContext businessContext, PriceCalculatorEntity priceCalculator, NavigationPathElement navigationPath)
		{
			this.DisposeTableDesigner ();

			businessContext.AcquireLock ();

			this.activeDesigner   = new TableDesigner (this.orchestrator, businessContext, priceCalculator);
			this.activeController = new DummyTableController (this.orchestrator.Navigator, navigationPath);

			this.orchestrator.DataViewController.SetCustomUI (this.activeDesigner.CreateUI ());
		}

		private void DisableTableDesigner()
		{
			if (this.activeDesigner != null)
			{
				this.isDesignerDisabled = true;
			}

			if (this.activeController != null)
			{
				this.activeController.Dispose ();
				this.activeController = null;
			}
		}

		private void DisposeTableDesigner()
		{
			if (this.activeDesigner != null)
			{
				this.activeDesigner.Dispose ();
				this.activeDesigner = null;
			}

			this.DisableTableDesigner ();
			this.isDesignerDisabled = false;
		}


		private readonly PlugInFactory			factory;
		private readonly CoreApplication		application;
		private readonly DataViewOrchestrator	orchestrator;

		private TableDesigner					activeDesigner;
		private DummyTableController			activeController;
		private bool							isDesignerDisabled;
	}
}
