//	Copyright � 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.IO
{
	/// <summary>
	/// Cette classe s'occupe de cr�er une nouvelle comptabilit� de toutes pi�ces.
	/// </summary>
	public class NewCompta
	{
		public void NewNull(ComptaEntity compta)
		{
			compta.Name          = "vide";
			compta.Description   = null;
			compta.BeginDate     = null;
			compta.EndDate       = null;
			compta.Derni�reDate  = null;
			compta.Derni�rePi�ce = null;

			compta.Journal.Clear ();
			compta.PlanComptable.Clear ();
			compta.Journaux.Clear ();
		}

		public void NewEmpty(ComptaEntity compta)
		{
			var now = Date.Today;
			var beginDate = new Date (now.Year, 1, 1);  // 1 janvier
			var endDate   = new Date (now.Year, 12, 31);  // 31 d�cembre

			compta.Name          = "vide";
			compta.Description   = null;
			compta.BeginDate     = beginDate;
			compta.EndDate       = endDate;
			compta.Derni�reDate  = beginDate;
			compta.Derni�rePi�ce = "0";

			compta.Journal.Clear ();
			compta.PlanComptable.Clear ();
			compta.Journaux.Clear ();

			//	Cr�e un journal principal.
			var journal = new ComptaJournalEntity ();
			journal.Name = "Principal";

			compta.Journaux.Add (journal);
		}

		public void NewModel(ComptaEntity compta)
		{
			// TODO...
			this.NewEmpty (compta);
		}
	}
}
