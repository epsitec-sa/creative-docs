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
			compta.Nom           = "vide";
			compta.Description   = null;
			compta.Derni�rePi�ce = null;

			compta.PlanComptable.Clear ();
			compta.P�riodes.Clear ();
			compta.Journaux.Clear ();
		}

		public void NewEmpty(ComptaEntity compta)
		{
			compta.Nom           = "vide";
			compta.Description   = null;
			compta.Derni�rePi�ce = "0";

			compta.PlanComptable.Clear ();
			compta.P�riodes.Clear ();
			compta.Journaux.Clear ();

			this.CreateP�riodes (compta);
			compta.Journaux.Add (this.CreateJournal (compta));
		}

		public void CreateP�riodes(ComptaEntity compta, int pastCount = -1, int postCount = 10)
		{
			compta.P�riodes.Clear ();

			var now = Date.Today;
			for (int year = now.Year+pastCount; year < now.Year+postCount; year++)
			{
				compta.P�riodes.Add (this.CreateP�riode (year));
			}
		}

		private ComptaP�riodeEntity CreateP�riode(int year)
		{
			//	Cr�e une p�riode couvrant une ann�e.
			var beginDate = new Date (year,  1,  1);  // du 1 janvier
			var endDate   = new Date (year, 12, 31);  // au 31 d�cembre

			var p�riode = new ComptaP�riodeEntity ();
			p�riode.DateD�but    = beginDate;
			p�riode.DateFin      = endDate;
			p�riode.Derni�reDate = beginDate;

			return p�riode;
		}

		private ComptaJournalEntity CreateJournal(ComptaEntity compta)
		{
			//	Cr�e un journal principal.
			var journal = new ComptaJournalEntity ();
			journal.Id = compta.GetJournalId ();
			journal.Nom = "Principal";

			return journal;
		}

		public void NewModel(ComptaEntity compta)
		{
			// TODO...
			this.NewEmpty (compta);
		}
	}
}
