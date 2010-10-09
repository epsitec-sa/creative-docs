//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.PlugIns;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner
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
			this.orchestrator = this.application.MainWindowOrchestrator;

			this.application.CreatedUI       += sender => System.Diagnostics.Debug.WriteLine ("EVENT: Application finished creating the UI");
			this.application.ShutdownStarted += sender => System.Diagnostics.Debug.WriteLine ("EVENT: Application shutting down");

			this.orchestrator.SettingActiveEntity += this.HandleOrchestratorSettingActiveEntity;
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
				var key = entityKey.Value;
				
				if (key.EntityId == EntityInfo<WorkflowNodeEntity>.GetTypeId ())
				{
					var businessContext = this.orchestrator.DefaultBusinessContext;

					businessContext.SetActiveEntity (entityKey, navigationPathElement);

					var workflow = businessContext.ActiveEntity as WorkflowDefinitionEntity;
					
					System.Diagnostics.Debug.WriteLine ("EVENT: Edit workflow <" + workflow.Name + ">");
					
					this.CreateWorkflowDesigner (businessContext, workflow);
					e.Cancel = true;
				}
			}
		}

		private void CreateWorkflowDesigner(Core.Business.BusinessContext businessContext, WorkflowDefinitionEntity workflow)
		{
			if (this.activeDesigner != null)
            {
				this.activeDesigner.Dispose ();
				this.activeDesigner = null;
            }

			this.activeDesigner = new WorkflowDesigner (businessContext, workflow);

			this.orchestrator.DataViewController.SetCustomUI (this.activeDesigner.CreateUI ());
		}

		
		private readonly PlugInFactory			factory;
		private readonly CoreApplication		application;
		private readonly DataViewOrchestrator	orchestrator;

		private WorkflowDesigner				activeDesigner;
	}
}
