//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types.Collections;

namespace Epsitec.Cresus.Core.Workflows
{
	public class WorkflowAction
	{
		internal WorkflowAction(IEnumerable<string> sourceLines = null)
		{
			if (sourceLines == null)
			{
				this.sourceLines = EmptyList<string>.Instance;
			}
			else
			{
				this.sourceLines = sourceLines.ToList ();
			}

			this.verbs = new List<ActionVerb> ();
		}


		/// <summary>
		/// Gets the source lines.
		/// </summary>
		/// <value>The source lines.</value>
		public IList<string>					SourceLines
		{
			get
			{
				return new Epsitec.Common.Types.Collections.ReadOnlyList<string> (this.sourceLines);
			}
		}

		public System.Action					Action
		{
			get
			{
				return this.ExecuteAction;
			}
		}

		public IEnumerable<ActionVerb>			ActionVerbs
		{
			get
			{
				return this.verbs;
			}
		}

		/// <summary>
		/// Executes the compiled code (if any).
		/// </summary>
		public void Execute()
		{
			this.ExecuteAction ();
		}

		private void UpdateActionList()
		{
			if ((this.actionList == null) &&
				(this.verbs.Count > 0))
			{
				//	If needed for performance reasons, we could compile the actions to real IL code
				//	in order to speed up execution:

				this.actionList = new List<System.Action> ();

				foreach (System.Action action in this.verbs.Select (x => x.MemberInfo).Cast<System.Reflection.MethodInfo> ().Select (x => System.Delegate.CreateDelegate (typeof (System.Action), x)))
				{
					this.actionList.Add (action);
				}
			}
		}
		
		private void ExecuteAction()
		{
			this.UpdateActionList ();

			if ((this.actionList != null) &&
				(this.actionList.Count > 0))
			{
				foreach (var action in this.actionList)
				{
					action ();
				}
			}
		}

		public void Add(ActionVerb verb)
		{
			this.verbs.Add (verb);
			this.actionList = null;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance. It is a serialized
		/// version of the source code.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Join ("\n", this.sourceLines);
		}

		private readonly IList<string>			sourceLines;
		private readonly List<ActionVerb>		verbs;
		private List<System.Action>				actionList;
	}

	/// <summary>
	/// The <c>WorkflowActionCompiler</c> class implements the compilation and execution
	/// of code associated with workflow edge transitions.
	/// </summary>
	public static class WorkflowActionCompiler
	{
		/// <summary>
		/// Validates the specified source code lines and returns the validation result.
		/// This also produces the compiled version of the code, which can then be
		/// executed through the <see cref="WorkflowAction"/>.
		/// </summary>
		/// <param name="lines">The source code lines.</param>
		/// <returns>The validation result.</returns>
		public static WorkflowActionValidationResult Validate(IEnumerable<string> lines)
		{
			return WorkflowActionCompiler.ValidateAndCompile (new WorkflowAction (lines));
		}


		/// <summary>
		/// Parses the specified serialized source code. Only parse code emitted by <see cref="ToString"/>.
		/// To parse plain source code, use the <see cref="Compile"/> method.
		/// </summary>
		/// <param name="source">The serialized source code.</param>
		/// <returns>A <see cref="WorkflowAction"/> ready for execution.</returns>
		public static WorkflowAction Compile(string source)
		{
			if (string.IsNullOrEmpty (source))
			{
				return new WorkflowAction ();
			}

			var lines  = source.Split ('\n');
			var result = WorkflowActionCompiler.Validate (lines);
			
			if (result.IsValid)
			{
				return result.WorkflowAction;
			}

			throw new System.FormatException ("Invalid WorkflowAction source");
		}


		private static WorkflowActionValidationResult ValidateAndCompile(WorkflowAction action)
		{
			var result = new WorkflowActionValidationResult (action);
			
			int lineNumber = 0;
			
			foreach (var line in action.SourceLines)
			{
				lineNumber++;

				if (WorkflowActionCompiler.IsEmptyLine (line))
				{
					continue;
				}
				
				string[] tokens = line.Split ('.');

				if (tokens.Length == 2)
				{
					string actionClass = tokens[0];
					string actionName  = tokens[1];

					var actionType = WorkflowActionResolver.ResolveActionClass (actionClass);
					var actionVerb = WorkflowActionResolver.GetActionVerbs (actionClass).Where (x => x.Name == actionName).FirstOrDefault ();

					if (actionType == null)
					{
						result.AddError (lineNumber, string.Concat ("cannot resolve class ", actionClass));
					}
					else if (actionVerb == null)
					{
						result.AddError (lineNumber, string.Concat ("cannot resolve verb ", actionName, " for class ", actionClass));
					}
					else
					{
						action.Add (actionVerb);
					}
				}
				else
				{
					result.AddError (lineNumber, "syntax error");
				}
			}

			return result;
		}

		private static bool IsEmptyLine(string line)
		{
			if ((line.Length == 0) ||
				(line.StartsWith ("//")) ||
				(line.StartsWith ("#")))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}