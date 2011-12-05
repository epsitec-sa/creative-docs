//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Business.Accounting
{
	public struct AccountingEntryDefinition
	{
		/// <summary>
		/// Cette classe représente une écriture simple (c'est-à-dire non multiple).
		/// </summary>
		public AccountingEntryDefinition(string debit, string credit, string description, decimal montant, string typeTva = null, string numeroTva = null, string codeTva = null, string codeAnalytique = null)
		{
			this.debit          = debit;
			this.credit         = credit;
			this.description    = description;
			this.montant        = montant;
			this.typeTva        = typeTva;
			this.numeroTva      = numeroTva;
			this.codeTva        = codeTva;
			this.codeAnalytique = codeAnalytique;
		}


		public string Debit
		{
			get
			{
				return this.debit;
			}
		}

		public string Credit
		{
			get
			{
				return this.credit;
			}
		}


		public string Description
		{
			get
			{
				return this.description;
			}
		}

		public decimal Montant
		{
			get
			{
				return this.montant;
			}
		}

		public string TypeTva
		{
			get
			{
				return this.typeTva;
			}
		}

		public string NumeroTva
		{
			get
			{
				return this.numeroTva;
			}
		}

		public string CodeTva
		{
			get
			{
				return this.codeTva;
			}
		}

		public string CodeAnalytique
		{
			get
			{
				return this.codeAnalytique;
			}
		}


		private readonly string				debit;
		private readonly string				credit;
		private readonly string				description;
		private readonly decimal			montant;
		private readonly string				typeTva;
		private readonly string				numeroTva;
		private readonly string				codeTva;
		private readonly string				codeAnalytique;
	}
}
