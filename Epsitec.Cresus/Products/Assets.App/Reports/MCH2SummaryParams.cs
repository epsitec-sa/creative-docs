//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MCH2SummaryParams : AbstractParams
	{
		public MCH2SummaryParams(Timestamp timestamp, Guid rootGuid)
		{
			this.Timestamp = timestamp;
			this.RootGuid  = rootGuid;
		}

		public readonly Timestamp				Timestamp;
		public readonly Guid					RootGuid;
	}
}
