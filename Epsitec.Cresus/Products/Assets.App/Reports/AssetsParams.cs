//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AssetsParams : AbstractParams
	{
		public AssetsParams(Timestamp timestamp, Guid rootGuid)
		{
			this.Timestamp = timestamp;
			this.RootGuid  = rootGuid;
		}

		public AssetsParams()
		{
			this.Timestamp = Timestamp.Now;
			this.RootGuid  = Guid.Empty;
		}

		public readonly Timestamp				Timestamp;
		public readonly Guid					RootGuid;
	}
}
