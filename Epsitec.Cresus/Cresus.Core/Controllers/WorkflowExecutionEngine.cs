//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class WorkflowExecutionEngine
	{
		public WorkflowExecutionEngine(WorkflowController controller, WorkflowEdge edge)
		{
			this.controller = controller;
			this.edge = edge;
		}

		public static WorkflowExecutionEngine Current
		{
			get
			{
				return WorkflowExecutionEngine.current;
			}
		}


		public WorkflowEdge Edge
		{
			get
			{
				return this.edge;
			}
		}

		public WorkflowController Controller
		{
			get
			{
				return this.controller;
			}
		}


		public void Execute()
		{
			var previousExecutionEngine = WorkflowExecutionEngine.current;

			try
			{
				WorkflowExecutionEngine.current = this;
				System.Diagnostics.Debug.WriteLine ("Executed " + edge.Edge.TransitionAction);
			}
			finally
			{
				WorkflowExecutionEngine.current = previousExecutionEngine;
			}
		}

		[System.ThreadStatic]
		private static WorkflowExecutionEngine current;

		private readonly WorkflowEdge edge;
		private readonly WorkflowController controller;
	}
}
