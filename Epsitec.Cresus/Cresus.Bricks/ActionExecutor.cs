using Epsitec.Common.Support.EntityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Epsitec.Cresus.Bricks
{
	public sealed class ActionExecutor
	{
		private ActionExecutor(Delegate action)
		{
			this.action = action;
		}

		public IEnumerable<Type> GetArgumentTypes()
		{
			return this.action.Method
				.GetParameters ()
				.Skip (1)
				.Select (p => p.ParameterType);
		}

		public void Call(AbstractEntity entity, IList<object> arguments)
		{
			var args = new object[arguments.Count + 1];

			args[0] = entity;

			for (int i = 0; i < arguments.Count; i++)
			{
				args[i + 1] = arguments[i];
			}

			try
			{
				this.action.DynamicInvoke (args);
			}
			catch (TargetInvocationException e)
			{
				if (e.InnerException != null)
				{
					throw e.InnerException;
				}

				throw;
			}
		}

		public static ActionExecutor Create<T>(Action<T> action)
			where T : AbstractEntity, new ()
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T, T1>(Action<T, T1> action)
			where T : AbstractEntity, new ()
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T, T1, T2>(Action<T, T1, T2> action)
			where T : AbstractEntity, new ()
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T, T1, T2, T3>(Action<T, T1, T2, T3> action)
			where T : AbstractEntity, new ()
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T, T1, T2, T3, T4>(Action<T, T1, T2, T3, T4> action)
			where T : AbstractEntity, new ()
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T, T1, T2, T3, T4, T5>(Action<T, T1, T2, T3, T4, T5> action)
			where T : AbstractEntity, new ()
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T, T1, T2, T3, T4, T5, T6>(Action<T, T1, T2, T3, T4, T5, T6> action)
			where T : AbstractEntity, new ()
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T, T1, T2, T3, T4, T5, T6, T7>(Action<T, T1, T2, T3, T4, T5, T6, T7> action)
			where T : AbstractEntity, new ()
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T, T1, T2, T3, T4, T5, T6, T7, T8>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8> action)
			where T : AbstractEntity, new ()
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
			where T : AbstractEntity, new ()
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action)
			where T : AbstractEntity, new ()
		{
			return new ActionExecutor (action);
		}

		private readonly Delegate action;
	}
}
