//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	/// <summary>
	/// The <c>Accessor</c> class implements a getter which uses two functions to
	/// return a result: the first one gets the source object and the second one
	/// extracts the result from the source object.
	/// </summary>
	public abstract class Accessor
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
}