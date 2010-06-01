//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	public class AccessorBinding
	{
		public AccessorBinding(System.Action action)
		{
			this.action = action;
		}

		public void Execute()
		{
			this.action ();
		}

		public static AccessorBinding Create<T>(Accessor<T> accessor, System.Action<T> setter)
		{
			return new AccessorBinding (() => setter (accessor.ExecuteGetter ()));
		}

		private readonly System.Action action;
	}
}
