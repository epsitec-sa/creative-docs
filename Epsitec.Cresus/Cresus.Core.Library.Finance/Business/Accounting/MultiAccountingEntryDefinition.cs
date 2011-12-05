//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Business.Accounting
{
	public struct MultiAccountingEntryDefinition
	{
		/// <summary>
		/// Cette classe représente une écriture quelconque (c'est-à-dire simple ou multiple).
		/// </summary>
		public MultiAccountingEntryDefinition(Date date, string piece)
		{
			this.debits  = new List<AccountingEntryDefinition> ();
			this.credits = new List<AccountingEntryDefinition> ();

			this.date  = date;
			this.piece = piece;
		}


		public Date Date
		{
			get
			{
				return this.date;
			}
		}

		public List<AccountingEntryDefinition> Debits
		{
			get
			{
				return this.debits;
			}
		}

		public List<AccountingEntryDefinition> Credits
		{
			get
			{
				return this.credits;
			}
		}


		public string Piece
		{
			get
			{
				return this.piece;
			}
		}


		private readonly Date								date;
		private readonly string								piece;
		private readonly List<AccountingEntryDefinition>	debits;
		private readonly List<AccountingEntryDefinition>	credits;
	}
}
