using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Epsitec.Cresus.Bricks
{
	public sealed class ActionExecutor : AbstractExecutor
	{
		private ActionExecutor(Delegate action)
			: base (action)
		{
		}

		public void Call(IList<object> arguments)
		{
			try
			{
				this.Action.DynamicInvoke (arguments.ToArray ());
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

		public static ActionExecutor Create(Action action)
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T1>(Action<T1> action)
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T1, T2>(Action<T1, T2> action)
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T1, T2, T3>(Action<T1, T2, T3> action)
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action)
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action)
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action)
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
		{
			return new ActionExecutor (action);
		}

		public static ActionExecutor Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action)
		{
			return new ActionExecutor (action);
		}
	}
}