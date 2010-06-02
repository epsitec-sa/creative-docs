//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	public class Accessor
	{
		public static Accessor<TResult> Create<T, TResult>(System.Func<T> sourceValueFunction, System.Func<T, TResult> resultValueFunction)
			where T : new ()
		{
			return new Accessor<TResult> (() => resultValueFunction (sourceValueFunction ()));
		}
	}
	
	public class Accessor<TResult> : Accessor
	{
		public Accessor(System.Func<TResult> getter)
		{
			this.getter = getter;
		}

		public TResult ExecuteGetter()
		{
			return this.getter ();
		}

		private readonly System.Func<TResult> getter;
	}
}
