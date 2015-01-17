//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

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

		public bool IsValid
		{
			//	Retourne true si tout est ok pour passer l'écriture.
			get
			{
				return !string.IsNullOrEmpty (this.Debit)   // le compte au débit doit exister
					&& !string.IsNullOrEmpty (this.Credit)  // le compte au crédit doit exister
					&& !string.IsNullOrEmpty (this.Title)   // le libellé doit exister
					&& this.Amount != 0.0m;                 // le montant ne doit pas être nul
			}
		}

		public static EntryProperties Empty = new EntryProperties
		{
			Date   = System.DateTime.MinValue,
			Debit  = null,
			Credit = null,
			Stamp  = null,
			Title  = null,
			Amount = 0.0m,
		};
	}
}
