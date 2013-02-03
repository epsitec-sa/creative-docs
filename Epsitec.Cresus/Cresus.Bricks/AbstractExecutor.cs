using System;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Bricks
{
	public abstract class AbstractExecutor
	{
		protected AbstractExecutor(Delegate action)
		{
			this.action = action;
		}

		protected Delegate Action
		{
			get
			{
				return this.action;
			}
		}

		public IEnumerable<Type> GetArgumentTypes()
		{
			return this.action.Method
				.GetParameters ()
				.Select (p => p.ParameterType);
		}

		private readonly Delegate action;
	}
}
