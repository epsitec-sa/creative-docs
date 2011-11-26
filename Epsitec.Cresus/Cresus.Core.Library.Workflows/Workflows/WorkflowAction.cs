//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections;

using System.Collections.Generic;
using System.Linq;
using System;

namespace Epsitec.Cresus.Core.Workflows
{
	/// <summary>
	/// The <c>WorkflowAction</c> class wraps a sequence of <see cref="ActionVerb"/> verbs,
	/// which can be executed just like a <see cref="System.Action"/>.
	/// </summary>
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

		public bool								IsInvalid
		{
			get
			{
				return this.verbs.Count == 0;
			}
		}

        /// <summary>
		/// Executes the compiled code (if any).
		/// </summary>
		public void Execute()
		{
			this.ExecuteAction ();
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

		private readonly IList<string>			sourceLines;
		private readonly List<ActionVerb>		verbs;
		private List<System.Action>				actionList;
	}
}
