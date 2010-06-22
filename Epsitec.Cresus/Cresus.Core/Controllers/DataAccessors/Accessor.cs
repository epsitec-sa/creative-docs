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

		public System.Func<TResult> Getter
		{
			get
			{
				return this.getter;
			}
		}

		public virtual TResult ExecuteGetter()
		{
			return this.getter ();
		}

		private readonly System.Func<TResult> getter;
	}
	
	public class AccessorWithDefaultValue<TResult> : Accessor<TResult>
	{
		public AccessorWithDefaultValue(System.Func<TResult> getter, TResult defaultResult, System.Predicate<TResult> isEmptyPredicate)
			: base (getter)
		{
			this.defaultResult    = defaultResult;
			this.isEmptyPredicate = isEmptyPredicate;
		}

		public override TResult ExecuteGetter()
		{
			TResult result = base.ExecuteGetter ();

			if (this.isEmptyPredicate (result))
			{
				return this.defaultResult;
			}
			else
			{
				return result;
			}
		}

		private readonly TResult defaultResult;
		private readonly System.Predicate<TResult> isEmptyPredicate;
	}
}
