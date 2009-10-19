//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Actions
{
	/// <summary>
	/// The <c>Action</c> class encapsulates a delegate which will be used
	/// to execute user actions (see <cref="GenericAction"/> for a concrete
	/// class).
	/// </summary>
	public abstract class Action
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Action"/> class.
		/// </summary>
		/// <param name="action">The action.</param>
		protected Action(System.Delegate action)
		{
			System.Diagnostics.Debug.Assert (action.Method.GetParameters ().Length < 3);

			this.action = action;
		}

		
		/// <summary>
		/// Gets the tag of the action, which is the concatenation of the target
		/// type name and target method name.
		/// </summary>
		/// <value>The tag.</value>
		public string Tag
		{
			get
			{
				string targetTypeName   = this.Target.GetType ().FullName;
				string targetMethodName = this.Method.Name;

				return string.Concat (targetTypeName, ".", targetMethodName);
			}
		}

		/// <summary>
		/// Gets the argument type.
		/// </summary>
		/// <value>The argument type (or <c>typeof (void)</c> if the action has no argument).</value>
		public System.Type ArgumentType1
		{
			get
			{
				var parameters = this.Method.GetParameters ();

				if (parameters.Length < 1)
				{
					return typeof (void);
				}
				else
				{
					return parameters[0].ParameterType;
				}
			}
		}

		public System.Type ArgumentType2
		{
			get
			{
				var parameters = this.Method.GetParameters ();

				if (parameters.Length < 2)
				{
					return typeof (void);
				}
				else
				{
					return parameters[1].ParameterType;
				}
			}
		}

		
		private object Target
		{
			get
			{
				return this.action.Target;
			}
		}

		private System.Reflection.MethodInfo Method
		{
			get
			{
				return this.action.Method;
			}
		}


		/// <summary>
		/// Invokes the action delegate with the specified arguments.
		/// </summary>
		/// <param name="args">The arguments.</param>
		public void DynamicInvoke(params object[] args)
		{
			this.action.DynamicInvoke (args);
		}

		
		private readonly System.Delegate action;
	}
}
