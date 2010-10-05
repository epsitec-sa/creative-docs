//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Accounting
{
	public struct BookAccountDefinition
	{
		public BookAccountDefinition(string accountNumber, FormattedText caption)
		{
			this.accountNumber = accountNumber;
			this.caption = caption;
		}

		internal BookAccountDefinition(Epsitec.CresusToolkit.CresusComptaCompte cresus)
		{
			this.accountNumber = cresus.Number;
			this.caption       = FormattedText.FromSimpleText (cresus.Caption);
		}


		public string AccountNumber
		{
			get
			{
				return this.accountNumber;
			}
		}

		public FormattedText Caption
		{
			get
			{
				return this.caption;
			}
		}


		private readonly string accountNumber;
		private readonly FormattedText caption;
	}
}
