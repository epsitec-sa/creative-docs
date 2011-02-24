//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>WorkflowActionValidationResult</c> class represents the validation result
	/// associated with a <see cref="WorkflowAction"/>, i.e. the error messages associated
	/// with the validation or compilation of a piece of source code.
	/// </summary>
	public class WorkflowActionValidationResult : IValidationResult
	{
		public WorkflowActionValidationResult()
		{
			this.errors = new List<LineError> ();
		}

		public bool								HasErrors
		{
			get
			{
				return this.errors.Count > 0;
			}
		}

		public IList<LineError>					Errors
		{
			get
			{
				return this.errors.AsReadOnly ();
			}
		}

		#region IValidationResult Members

		public bool IsValid
		{
			get
			{
				return this.HasErrors == false;
			}
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
			get
			{
				return FormattedText.Join ("<br/>", this.errors.Select (x => FormattedText.FromSimpleText (string.Format ("{0}: {1}", x.LineNumber, x.Message))));
			}
		}

		#endregion

		public void AddError(int lineNumber, string message)
		{
			this.errors.Add (new LineError (lineNumber, message));
		}



		#region LineError Structure

		public struct LineError
		{
			public LineError(int lineNumber, string message)
			{
				this.lineNumber = lineNumber;
				this.message    = message;
			}

			public int							LineNumber
			{
				get
				{
					return this.lineNumber;
				}
			}

			public string						Message
			{
				get
				{
					return this.message;
				}
			}

			private readonly int				lineNumber;
			private readonly string				message;
		}

		#endregion

		private List<LineError> errors;
	}
}
