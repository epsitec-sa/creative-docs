//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Resolvers;
using Epsitec.Common.Types;

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
			string[] clean;
			return WorkflowAction.ValidateAndCompile (lines, out result, out action, out clean);
		}

		public WorkflowActionValidationResult Compile(IEnumerable<string> lines)
		{
			WorkflowActionValidationResult result;
			System.Action action;
			string[] clean;

			if (WorkflowAction.ValidateAndCompile (lines, out result, out action, out clean))
			{
				this.sourceLines = clean;
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


		private static bool ValidateAndCompile(IEnumerable<string> lines, out WorkflowActionValidationResult result, out System.Action action, out string[] clean)
		{
			List<string> errors = new List<string> ();
			List<System.Reflection.MethodInfo> code = new List<System.Reflection.MethodInfo> ();

			clean = lines.Select (x => x.Trim ()).ToArray ();

			action = null;
			result = new WorkflowActionValidationResult ()
			{
				IsValid = true
			};

			for (int i = 0; i < clean.Length; i++)
			{
				string line = clean[i];

				if ((line.Length == 0) ||
					(line.StartsWith ("//")) ||
					(line.StartsWith ("#")))
				{
					continue;
				}
				
				string[] tokens = line.Split ('.');

				if (tokens.Length == 2)
				{
					string actionClass = tokens[0];
					string actionVerb  = tokens[1];

					var memberInfo = BusinessActionResolver.GetActionVerbs (actionClass).Where (x => x.Name == actionVerb).Select (x => x.MemberInfo).FirstOrDefault () as System.Reflection.MethodInfo;

					if (memberInfo != null)
					{
						code.Add (memberInfo);
					}
					else
					{
						errors.Add (string.Format ("{0}: cannot resolve {1}.{2}", i+1, actionClass, actionVerb));
					}
				}
				else
				{
					errors.Add (string.Format ("{0}: syntax error", i+1));
				}
			}

			if (errors.Count > 0)
			{
				result.IsValid = false;
				result.ErrorMessage = FormattedText.Join ("<br/>", errors.Select (x => FormattedText.FromSimpleText (x)));
				return false;
			}

			action = 
				delegate
				{
					code.ForEach (x => x.Invoke (null, WorkflowAction.emptyParams));
				};

			return true;
		}

		private static readonly object[] emptyParams = new object[0];

		private string[] sourceLines;
		private System.Action action;
	}

	public class WorkflowActionValidationResult : IValidationResult
	{
		#region IValidationResult Members

		public bool IsValid
		{
			get;
			set;
		}


		public ValidationState State
		{
			get
			{
				return this.IsValid ? ValidationState.Ok : ValidationState.Error;
			}
		}

		public FormattedText ErrorMessage
		{
			get;
			set;
		}

		#endregion
	}
}
