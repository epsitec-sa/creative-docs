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
			this.name = name;
			this.pool = pool;
		}


		private readonly string name;
		private readonly UidGeneratorPool pool;
	}
}