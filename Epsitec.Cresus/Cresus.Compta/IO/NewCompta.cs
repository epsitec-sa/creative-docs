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

			compta.PlanComptable.Clear ();
			compta.P�riodes.Clear ();
			compta.Journaux.Clear ();
		}

		public void NewEmpty(ComptaEntity compta)
		{
			compta.Nom         = "vide";
			compta.Description = null;

			compta.PlanComptable.Clear ();
			compta.P�riodes.Clear ();
			compta.Journaux.Clear ();

			this.CreateP�riodes (compta);
			compta.Pi�cesGenerator.Add (this.CreatePi�cesGeneratorEntity ());
			compta.Journaux.Add (this.CreateJournal (compta));
			compta.Utilisateurs.Add (this.CreateUtilisateur ());
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

			journal.Id  = compta.GetJournalId ();
			journal.Nom = "Principal";

			return journal;
		}

		private ComptaUtilisateurEntity CreateUtilisateur()
		{
			//	Cr�e l'utilisteur administrateur.
			var utilisateur = new ComptaUtilisateurEntity ();

			utilisateur.Nom        = "Admin";
			utilisateur.MotDePasse = "epsitec";

			return utilisateur;
		}

		private ComptaPi�cesGeneratorEntity CreatePi�cesGeneratorEntity()
		{
			//	Cr�e le g�n�rateur de num�ros de pi�ces principal.
			var pi�ce = new ComptaPi�cesGeneratorEntity ();

			pi�ce.Nom       = "Base";
			pi�ce.Num�ro    = 1;
			pi�ce.Incr�ment = 1;

			return pi�ce;
		}


		public void NewModel(ComptaEntity compta)
		{
			// TODO...
			this.NewEmpty (compta);
		}
	}
}
