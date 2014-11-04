//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public struct MandatInfo
	{
		public MandatInfo(string softwareId, string softwareVersion, string softwareLanguage,
			string fileName, Guid fileGuid, string fileVersion, MandatStatistics statistics)
		{
			this.SoftwareId       = softwareId;
			this.SoftwareVersion  = softwareVersion;
			this.SoftwareLanguage = softwareLanguage;
			this.FileName	      = fileName;
			this.FileGuid	      = fileGuid;
			this.FileVersion	  = fileVersion;
			this.Statistics	      = statistics;
		}

		public bool IsEmpty
		{
			get
			{
				return this.FileGuid.IsEmpty;
			}
		}

		public static MandatInfo Empty = new MandatInfo (null, null, null, null, Guid.Empty, null, MandatStatistics.Empty);

		public readonly string					SoftwareId;
		public readonly string					SoftwareVersion;
		public readonly string					SoftwareLanguage;

		public readonly string					FileName;
		public readonly Guid					FileGuid;
		public readonly string					FileVersion;

		public readonly MandatStatistics		Statistics;
	}
}
