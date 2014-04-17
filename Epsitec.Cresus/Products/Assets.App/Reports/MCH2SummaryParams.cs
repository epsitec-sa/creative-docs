//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MCH2SummaryParams : AbstractParams
	{
		public MCH2SummaryParams(Timestamp initialTimestamp, Timestamp finalTimestamp, Guid rootGuid)
		{
			this.InitialTimestamp = initialTimestamp;
			this.FinalTimestamp   = finalTimestamp;
			this.RootGuid         = rootGuid;
		}

		public readonly Timestamp				InitialTimestamp;
		public readonly Timestamp				FinalTimestamp;
		public readonly Guid					RootGuid;
	}
}
