//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Core
{
	public sealed class CoreJobStatus
	{
		private CoreJobStatus(string value)
		{
			this.Value = value;
		}

		
		public string							Value
		{
			get;
			private set;
		}


		public static readonly CoreJobStatus	Ordered  = new CoreJobStatus ("en attente");
		public static readonly CoreJobStatus	Waiting  = new CoreJobStatus ("en attente");
		public static readonly CoreJobStatus	Running  = new CoreJobStatus ("en cours");
		public static readonly CoreJobStatus	Ended    = new CoreJobStatus ("terminé");
		public static readonly CoreJobStatus	Cancelled = new CoreJobStatus ("annulé");
	}
}

