//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic
{
	public class UidGenerator
	{
		internal UidGenerator(string name, UidGeneratorPool pool)
		{
			var trace = new System.Diagnostics.StackTrace (skipFrames: 1);

			if (trace.GetFrame (0).GetMethod ().DeclaringType != typeof (UidGeneratorPool))
            {
				throw new System.InvalidOperationException ("UidGenerator cannot be created directly: use the UidGeneratorPool instead");
            }

			this.name = name;
			this.pool = pool;
		}

		


		private readonly string name;
		private readonly UidGeneratorPool pool;
	}
}