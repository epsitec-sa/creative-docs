//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class WorkflowAction
	{
		public WorkflowAction()
		{
		}


		public IEnumerable<string> SourceLines
		{
			get
			{
				return this.sourceLines;
			}
		}


		public static bool Validate(IEnumerable<string> lines)
		{
			WorkflowActionValidationResult result;
			return WorkflowAction.Validate (lines, out result);
		}

		public static bool Validate(IEnumerable<string> lines, out WorkflowActionValidationResult result)
		{
			System.Action action;
			return WorkflowAction.ValidateAndCompile (lines, out result, out action);
		}

		public WorkflowActionValidationResult Compile(IEnumerable<string> lines)
		{
			WorkflowActionValidationResult result;
			System.Action action;

			if (WorkflowAction.ValidateAndCompile (lines, out result, out action))
			{
				this.sourceLines = lines.ToArray ();
				this.action      = action;
			}

			return result;
		}

		public void Execute()
		{
			if (this.action == null)
			{
				//	Nothing to do - no action defined.
			}
			else
			{
				this.action ();
			}
		}
		
		
		public override string ToString()
		{
			return string.Join ("\n", this.sourceLines);
		}

		public static WorkflowAction Parse(string source)
		{
			var lines  = source.Split ('\n');
			var action = new WorkflowAction ();
			var result = action.Compile (lines);
			
			if (result.IsValid)
            {
				return action;
            }

			throw new System.FormatException ("Invalid WorkflowAction source");
		}


		private static bool ValidateAndCompile(IEnumerable<string> lines, out WorkflowActionValidationResult result, out System.Action action)
		{
			action = null;
			result = new WorkflowActionValidationResult ()
			{
				IsValid = true
			};

			return true;
		}

		private string[] sourceLines;
		private System.Action action;
	}

	public class WorkflowActionValidationResult
	{
		public bool IsValid
		{
			get;
			set;
		}
	}
}
