//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Override
{
	public class CoreDatabaseInitializer : DatabaseInitializer
	{
		public CoreDatabaseInitializer(BusinessContext businessContext)
			: base (businessContext)
		{
		}

		public override void Run()
		{
			base.Run ();

			this.CreateTestUsers ();
		}
	}
}
