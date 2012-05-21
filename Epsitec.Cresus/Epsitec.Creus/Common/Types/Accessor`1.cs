//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>Accessor</c> class wraps a getter function.
	/// </summary>
	public class Accessor<TResult> : Accessor
	{
		public Accessor(System.Func<TResult> getter)
		{
			this.getter = getter;
		}

		
		public System.Func<TResult>				Getter
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