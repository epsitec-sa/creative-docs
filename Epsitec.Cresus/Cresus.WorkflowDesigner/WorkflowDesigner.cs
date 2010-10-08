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
	public sealed class WorkflowDesigner : System.IDisposable
	{
		public WorkflowDesigner(Core.Business.BusinessContext businessContext, WorkflowDefinitionEntity workflow)
		{
			this.businessContext = businessContext;
			this.workflow = workflow;
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

			var controller = new MainController (this.businessContext, workflow);
			controller.CreateUI (box);

			return box;
		}


		#region IDisposable Members

		public void Dispose()
		{
			if (this.editorUI != null)
			{
				this.editorUI.Dispose ();
				this.editorUI = null;
			}
		}

		#endregion


		private readonly Core.Business.BusinessContext businessContext;
		private readonly WorkflowDefinitionEntity workflow;

		private Widget editorUI;
	}
}
