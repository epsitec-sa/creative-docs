﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data.Reports
{
	public class AssetsParams : AbstractReportParams
	{
		public AssetsParams(Timestamp timestamp, Guid rootGuid, int? level)
		{
			this.Timestamp = timestamp;
			this.RootGuid  = rootGuid;
			this.Level     = level;
		}

		public AssetsParams()
		{
			this.Timestamp = Timestamp.Now;
			this.RootGuid  = Guid.Empty;
			this.Level     = null;
		}


		public override string					Title
		{
			get
			{
				return "Liste des objets d'immobilisations";
			}
		}


		public override bool StrictlyEquals(AbstractReportParams other)
		{
			if (other is AssetsParams)
			{
				var o = other as AssetsParams;

				return this.Timestamp == o.Timestamp
					&& this.RootGuid  == o.RootGuid
					&& this.Level     == o.Level;
			}

			return false;
		}

		public override AbstractReportParams ChangePeriod(int direction)
		{
			var timestamp = new Timestamp (this.Timestamp.Date.AddYears (direction), 0);
			return new AssetsParams (timestamp, this.RootGuid, this.Level);
		}


		public readonly Timestamp				Timestamp;
		public readonly Guid					RootGuid;
		public readonly int?					Level;
	}
}