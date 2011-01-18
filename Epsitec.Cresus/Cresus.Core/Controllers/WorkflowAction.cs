//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>WorkflowAction</c> class implements the compilation and execution
	/// of code associated with workflow edge transitions.
	/// </summary>
	public class WorkflowAction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WorkflowAction"/> class.
		/// </summary>
		public WorkflowAction()
		{
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


		/// <summary>
		/// Validates the specified source code lines.
		/// </summary>
		/// <param name="lines">The source code lines.</param>
		/// <returns><c>true</c> if the source code is valid; otherwise, <c>false</c>.</returns>
		public static bool Validate(IEnumerable<string> lines)
		{
			WorkflowActionValidationResult result;
			return WorkflowAction.Validate (lines, out result);
		}

		/// <summary>
		/// Validates the specified source code lines.
		/// </summary>
		/// <param name="lines">The source code lines.</param>
		/// <param name="result">The validation result, see <see cref="WorkflowActionValidationResult"/>.</param>
		/// <returns>
		/// 	<c>true</c> if the source code is valid; otherwise, <c>false</c>.
		/// </returns>
		public static bool Validate(IEnumerable<string> lines, out WorkflowActionValidationResult result)
		{
			System.Action action;
			return WorkflowAction.ValidateAndCompile (lines.ToArray (), out result, out action);
		}

		/// <summary>
		/// Compiles the specified source code lines. If there is no error,
		/// then the current <see cref="WorkflowAction"/> will be updated
		/// and ready for execution.
		/// </summary>
		/// <param name="lines">The source code lines.</param>
		/// <returns>The validation result, see <see cref="WorkflowActionValidationResult"/>.</returns>
		public WorkflowActionValidationResult Compile(IEnumerable<string> lines)
		{
			WorkflowActionValidationResult result;
			System.Action action;
			string[] copy = lines.ToArray ();

			if (WorkflowAction.ValidateAndCompile (copy, out result, out action))
			{
				this.sourceLines = copy;
				this.action      = action;
			}

			return result;
		}

		/// <summary>
		/// Executes the compiled code (if any).
		/// </summary>
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

		/// <summary>
		/// Parses the specified serialized source code. Only parse code emitted by <see cref="ToString"/>.
		/// To parse plain source code, use the <see cref="Compile"/> method.
		/// </summary>
		/// <param name="source">The serialized source code.</param>
		/// <returns>A <see cref="WorkflowAction"/> ready for execution.</returns>
		public static WorkflowAction Parse(string source)
		{
			var action = new WorkflowAction ();
			
			if (string.IsNullOrEmpty (source))
			{
				return action;
			}

			var lines  = source.Split ('\n');
			var result = action.Compile (lines);
			
			if (result.IsValid)
            {
				return action;
            }

			throw new System.FormatException ("Invalid WorkflowAction source");
		}


		private static bool ValidateAndCompile(string[] lines, out WorkflowActionValidationResult result, out System.Action action)
		{
			List<System.Reflection.MethodInfo> code = new List<System.Reflection.MethodInfo> ();

			action = null;
			result = new WorkflowActionValidationResult ();

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i].Trim ();

				if (WorkflowAction.IsEmptyLine (line))
				{
					continue;
				}
				
				string[] tokens = line.Split ('.');

				if (tokens.Length == 2)
				{
					string actionClass = tokens[0];
					string actionVerb  = tokens[1];

					var actionType = BusinessActionResolver.Resolve (actionClass);
					var memberInfo = BusinessActionResolver.GetActionVerbs (actionClass).Where (x => x.Name == actionVerb).Select (x => x.MemberInfo).FirstOrDefault () as System.Reflection.MethodInfo;

					if (actionType == null)
					{
						result.AddError (i+1, string.Concat ("cannot resolve class ", actionClass));
                    }
					else if (memberInfo == null)
					{
						result.AddError (i+1, string.Concat ("cannot resolve verb ", actionVerb, " for class ", actionClass));
					}
					else
					{
						code.Add (memberInfo);
					}
				}
				else
				{
					result.AddError (i+1, "syntax error");
				}
			}

			if (result.HasErrors)
			{
				return false;
			}

			//	If needed for performance reasons, we could compile the actions to real IL code
			//	in order to speed up execution:
			
			action = 
				delegate
				{
					code.ForEach (x => x.Invoke (null, WorkflowAction.emptyParams));
				};

			return true;
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

		private static readonly object[]		emptyParams = new object[0];

		private string[]						sourceLines;
		private System.Action					action;
	}
}