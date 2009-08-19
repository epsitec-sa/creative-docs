//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Actions
{
	public abstract class Action
	{
		protected Action(System.Delegate action)
		{
			System.Diagnostics.Debug.Assert (action.Method.GetParameters ().Length < 2);

			this.action = action;

			Factory.RegisterUserAction (this);
		}

		public string Tag
		{
			get
			{
				string targetTypeName   = this.Target.GetType ().FullName;
				string targetMethodName = this.Method.Name;

				return string.Concat (targetTypeName, ".", targetMethodName);
			}
		}

		public object Target
		{
			get
			{
				return this.action.Target;
			}
		}

		public System.Reflection.MethodInfo Method
		{
			get
			{
				return this.action.Method;
			}
		}

		public System.Type ArgumentType
		{
			get
			{
				var parameters = this.Method.GetParameters ();

				if (parameters.Length == 0)
				{
					return typeof (void);
				}
				else
				{
					return parameters[0].ParameterType;
				}
			}
		}

		public void DynamicInvoke(params object[] args)
		{
			this.action.DynamicInvoke (args);
		}

		private readonly System.Delegate action;
	}
}
