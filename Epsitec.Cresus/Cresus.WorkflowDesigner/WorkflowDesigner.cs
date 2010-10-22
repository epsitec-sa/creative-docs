//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.PlugIns;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner
{
	public sealed class WorkflowDesigner : System.IDisposable
	{
		public WorkflowDesigner(Core.Business.BusinessContext businessContext, WorkflowDefinitionEntity workflow)
		{
			this.businessContext = businessContext;
			this.workflow = workflow;
			this.businessContext.SavingChanges += this.HandleBusinessContextSavingChanges;
		}


		public Widget CreateUI()
		{
			this.editorUI = this.CreateWorkflowEditorUI (this.workflow);
			
			return this.editorUI;
		}

		private Widget CreateWorkflowEditorUI(WorkflowDefinitionEntity workflow)
		{
			var box = new FrameBox
			{
				Dock = DockStyle.Fill,
				Padding = new Margins (5),
			};

			this.mainController = new MainController (this.businessContext, workflow);
			this.mainController.CreateUI (box);

			return box;
		}


		private void HandleBusinessContextSavingChanges(object sender, CancelEventArgs e)
		{
			this.businessContext.DataContext.SaveChanges ();
			this.SaveDesign ();
		}

		private void SaveDesign()
		{
			//	Sérialise les réglages de l'entité 'workflow' dans le XmlBlob.
			this.mainController.SaveDesign ();
		}


		#region IDisposable Members

		public void Dispose()
		{
			if (this.editorUI != null)
			{
				this.editorUI.Dispose ();
				this.editorUI = null;
			}

			this.businessContext.SavingChanges -= this.HandleBusinessContextSavingChanges;
		}

		#endregion


		private readonly Core.Business.BusinessContext businessContext;
		private readonly WorkflowDefinitionEntity workflow;

		private Widget editorUI;
		private MainController mainController;
	}
}
