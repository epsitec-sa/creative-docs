//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public class EntryProperties
	{
		public System.DateTime					Date;
		public string							Debit;
		public string							Credit;
		public string							Stamp;
		public string							Title;
		public decimal							Amount;
	}
}
