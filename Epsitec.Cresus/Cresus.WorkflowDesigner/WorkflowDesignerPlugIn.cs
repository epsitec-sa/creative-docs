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
	[PlugIn ("WorkflowDesigner", "1.0")]
	public class WorkflowDesignerPlugIn : ICorePlugIn
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
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
		}

		private void HandleOrchestratorSettingActiveEntity(object sender, ActiveEntityCancelEventArgs e)
		{
			var entityKey = e.EntityKey;
			var navigationPathElement = e.NavigationPathElement;

            if (entityKey.HasValue)
			{
				var key = entityKey.Value;
				
				if (key.EntityId == EntityInfo<WorkflowDefinitionEntity>.GetTypeId ())
				{
					var businessContext = this.orchestrator.DefaultBusinessContext;

					businessContext.SetActiveEntity (entityKey, navigationPathElement);

					var workflow = businessContext.ActiveEntity as WorkflowDefinitionEntity;
					this.EditWorkflowDefinition (businessContext, workflow);
					e.Cancel = true;
				}
			}
		}

		private void EditWorkflowDefinition(Core.Business.BusinessContext businessContext, WorkflowDefinitionEntity workflow)
		{
			System.Diagnostics.Debug.WriteLine ("EVENT: Edit workflow <" + workflow.Name + ">");

			if (this.editorUI != null)
            {
				this.editorUI.Dispose ();
				this.editorUI = null;
            }

			//	TODO: créer le vrai widget d'édition; pour le moment, on crée simplement une zone
			//	vert limette avec le nom du workflow pris dans l'entité...

			this.editorUI = this.CreateWorkflowEditorUI (workflow);
			
			this.orchestrator.DataViewController.SetCustomUI (this.editorUI);
		}


		private Widget CreateWorkflowEditorUI(WorkflowDefinitionEntity workflow)
		{
			StaticText customUI = new StaticText ()
			{
				BackColor = Color.FromName ("Lime"),
				Dock = DockStyle.Fill,
				FormattedText = TextFormatter.FormatText ("Workflow <b>", workflow.Name, "</b>"),
			};

			return customUI;
		}

		private readonly PlugInFactory factory;
		private readonly CoreApplication application;
		private readonly DataViewOrchestrator orchestrator;

		private Widget editorUI;
	}
}
