//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Bricks
{
	public static class BridgeSpy
	{
		public static void ExecuteSetter<T1, T2>(T1 entity, LambdaExpression lambdaExpression, T2 value, System.Action<T1, T2> setter, System.Func<T1, T2> getter)
			where T1 : AbstractEntity
		{
			T2 oldValue = getter (entity);
			T2 newValue = value;

			var handler = BridgeSpy.ExecutingSetter;

			if (handler != null)
			{
				handler (null, new BridgeSpyEventArgs (entity, lambdaExpression, oldValue, newValue));
			}

			setter (entity, value);
		}


		public static event EventHandler<BridgeSpyEventArgs> ExecutingSetter;
	}
}