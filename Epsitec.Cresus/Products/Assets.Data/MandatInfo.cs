//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public struct MandatInfo
	{
		public MandatInfo(int majRev, int minRev, int buildRev, int idProducer, Guid mandatGuid, MandatStatistics statistics)
		{
			this.MajRev     = majRev;
			this.MinRev		= minRev;
			this.BuildRev	= buildRev;
			this.IdProducer	= idProducer;
			this.MandatGuid	= mandatGuid;
			this.Statistics	= statistics;
		}

		public bool IsEmpty
		{
			get
			{
				return this.MandatGuid.IsEmpty;
			}
		}

		public static MandatInfo Empty = new MandatInfo (0, 0, 0, 0, Guid.Empty, MandatStatistics.Empty);

		public readonly int						MajRev;
		public readonly int						MinRev;
		public readonly int						BuildRev;

		public readonly int						IdProducer;
		public readonly Guid					MandatGuid;

		public readonly MandatStatistics		Statistics;
	}
}
