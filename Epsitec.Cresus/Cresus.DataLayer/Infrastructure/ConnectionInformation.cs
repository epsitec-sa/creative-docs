//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.DataLayer.Infrastructure
{
	public sealed class ConnectionInformation
	{
		internal ConnectionInformation()
		{
			//	TODO: ...
		}
		
		public long ConnectionId
		{
			get
			{
				return this.connectionId;
			}
		}

		public ConnectionStatus Status
		{
			get
			{
				return ConnectionStatus.Active;
			}
		}


		private readonly long connectionId;
	}
}