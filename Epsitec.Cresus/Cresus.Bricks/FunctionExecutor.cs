using Epsitec.Common.Support.EntityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Epsitec.Cresus.Bricks
{
	public sealed class FunctionExecutor : AbstractExecutor
	{
		private FunctionExecutor(Delegate action)
			: base (action)
		{
		}

		public AbstractEntity Call(IList<object> arguments)
		{
			try
			{
				var result = this.Action.DynamicInvoke (arguments.ToArray ());

				return (AbstractEntity) result;
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

		public static FunctionExecutor Create<T>(Func<T> func)
			where T : AbstractEntity
		{
			return new FunctionExecutor (func);
		}

		public static FunctionExecutor Create<T1, T>(Func<T1, T> func)
			where T : AbstractEntity
		{
			return new FunctionExecutor (func);
		}

		public static FunctionExecutor Create<T1, T2, T>(Func<T1, T2, T> func)
			where T : AbstractEntity
		{
			return new FunctionExecutor (func);
		}

		public static FunctionExecutor Create<T1, T2, T3, T>(Func<T1, T2, T3, T> func)
			where T : AbstractEntity
		{
			return new FunctionExecutor (func);
		}

		public static FunctionExecutor Create<T1, T2, T3, T4, T>(Func<T1, T2, T3, T4, T> func)
			where T : AbstractEntity
		{
			return new FunctionExecutor (func);
		}

		public static FunctionExecutor Create<T1, T2, T3, T4, T5, T>(Func<T1, T2, T3, T4, T5, T> func)
			where T : AbstractEntity
		{
			return new FunctionExecutor (func);
		}

		public static FunctionExecutor Create<T1, T2, T3, T4, T5, T6, T>(Func<T1, T2, T3, T4, T5, T6, T> func)
			where T : AbstractEntity
		{
			return new FunctionExecutor (func);
		}

		public static FunctionExecutor Create<T1, T2, T3, T4, T5, T6, T7, T>(Func<T1, T2, T3, T4, T5, T6, T7, T> func)
			where T : AbstractEntity
		{
			return new FunctionExecutor (func);
		}

		public static FunctionExecutor Create<T1, T2, T3, T4, T5, T6, T7, T8, T>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T> func)
			where T : AbstractEntity
		{
			return new FunctionExecutor (func);
		}

		public static FunctionExecutor Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T> func)
			where T : AbstractEntity
		{
			return new FunctionExecutor (func);
		}

		public static FunctionExecutor Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T> func)
			where T : AbstractEntity
		{
			return new FunctionExecutor (func);
		}
	}
}
