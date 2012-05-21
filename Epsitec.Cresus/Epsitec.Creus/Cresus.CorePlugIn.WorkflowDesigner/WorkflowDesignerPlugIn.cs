//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Cresus.Core.PlugIns;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner
{
	/// <summary>
	/// The <c>WorkflowDesignerPlugIn</c> class implements the plug-in interface to
	/// communicate with <c>Cresus.Core</c>. It provides an editor for entities of
	/// type <see cref="WorkflowDefinitionEntity"/>.
	/// </summary>
	[PlugIn ("WorkflowDesigner", "1.0")]
	public sealed class WorkflowDesignerPlugIn : ICorePlugIn
	{
		public WorkflowDesignerPlugIn(PlugInFactory factory)
		{
			this.factory = factory;
			this.application = this.factory.Application;
			this.orchestrator = this.application.FindActiveComponent<DataViewOrchestrator> ();

//			this.application.CreatedUI       += sender => System.Diagnostics.Debug.WriteLine ("EVENT: Application finished creating the UI");
//			this.application.ShutdownStarted += sender => System.Diagnostics.Debug.WriteLine ("EVENT: Application shutting down");

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
					this.DisposeWorkflowDesigner ();
				}

				var key = entityKey.Value;

				if (key.EntityId == EntityInfo<WorkflowNodeEntity>.GetTypeId ())
				{
					this.orchestrator.ClearActiveEntity ();

					var businessContext    = this.orchestrator.DefaultBusinessContext;
					var workflowDefinition = businessContext.SetActiveEntity<WorkflowDefinitionEntity> (entityKey, navigationPathElement);

					System.Diagnostics.Debug.WriteLine ("EVENT: Edit workflow <" + workflowDefinition.Name + ">");

					this.CreateWorkflowDesigner (businessContext, workflowDefinition, navigationPathElement);

					e.Cancel = true;
				}
				else
				{
					this.DisposeWorkflowDesigner ();
				}
			}
			else
			{
				//	We may not dispose the designer, since it might still be needed in the very
				//	near future, for instance to save the design (layout) of the workflow. This
				//	is why we simply disable the designer for now :

				this.DisableWorkflowDesigner ();
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

		private void CreateWorkflowDesigner(Core.Business.BusinessContext businessContext, WorkflowDefinitionEntity workflow, NavigationPathElement navigationPath)
		{
			this.DisposeWorkflowDesigner ();

			businessContext.AcquireLock ();
			
			this.activeDesigner   = new WorkflowDesigner (this.orchestrator, businessContext, workflow);
			this.activeController = new DummyWorkflowController (this.orchestrator.Navigator, navigationPath);

			this.orchestrator.DataViewController.SetCustomUI (this.activeDesigner.CreateUI ());
		}

		private void DisableWorkflowDesigner()
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

		private void DisposeWorkflowDesigner()
		{
			if (this.activeDesigner != null)
			{
				this.activeDesigner.Dispose ();
				this.activeDesigner = null;
			}

			this.DisableWorkflowDesigner ();
			this.isDesignerDisabled = false;
		}

		
		private readonly PlugInFactory			factory;
		private readonly CoreApp				application;
		private readonly DataViewOrchestrator	orchestrator;

		private WorkflowDesigner				activeDesigner;
		private DummyWorkflowController			activeController;
		private bool							isDesignerDisabled;
	}
}
