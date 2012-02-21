//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.IO
{
	/// <summary>
	/// Cette classe s'occupe de créer une nouvelle comptabilité de toutes pièces.
	/// </summary>
	public class NewCompta
	{
		public void NewNull(ComptaEntity compta)
		{
			compta.Nom           = "vide";
			compta.Description   = null;

			compta.PlanComptable.Clear ();
			compta.Périodes.Clear ();
			compta.Journaux.Clear ();
		}

		public void NewEmpty(ComptaEntity compta)
		{
			compta.Nom         = "vide";
			compta.Description = null;

			compta.PlanComptable.Clear ();
			compta.Périodes.Clear ();
			compta.Journaux.Clear ();

			this.CreatePériodes (compta);
			compta.PiècesGenerator.Add (this.CreatePiècesGeneratorEntity ());
			compta.Journaux.Add (this.CreateJournal (compta));
			compta.Utilisateurs.Add (this.CreateUtilisateur ());
		}

		public void CreatePériodes(ComptaEntity compta, int pastCount = -1, int postCount = 10)
		{
			compta.Périodes.Clear ();

			var now = Date.Today;
			for (int year = now.Year+pastCount; year < now.Year+postCount; year++)
			{
				compta.Périodes.Add (this.CreatePériode (year));
			}
		}

		private ComptaPériodeEntity CreatePériode(int year)
		{
			//	Crée une période couvrant une année.
			var beginDate = new Date (year,  1,  1);  // du 1 janvier
			var endDate   = new Date (year, 12, 31);  // au 31 décembre

			var période = new ComptaPériodeEntity ();
			période.DateDébut    = beginDate;
			période.DateFin      = endDate;
			période.DernièreDate = beginDate;

			return période;
		}

		private ComptaJournalEntity CreateJournal(ComptaEntity compta)
		{
			//	Crée un journal principal.
			var journal = new ComptaJournalEntity ();

			journal.Id  = compta.GetJournalId ();
			journal.Nom = "Principal";

			return journal;
		}

		private ComptaUtilisateurEntity CreateUtilisateur()
		{
			//	Crée l'utilisteur administrateur.
			var utilisateur = new ComptaUtilisateurEntity ();

			utilisateur.Nom        = "Admin";
			utilisateur.MotDePasse = "epsitec";

			return utilisateur;
		}

		private ComptaPiècesGeneratorEntity CreatePiècesGeneratorEntity()
		{
			//	Crée le générateur de numéros de pièces principal.
			var pièce = new ComptaPiècesGeneratorEntity ();

			pièce.Nom       = "Base";
			pièce.Numéro    = 1;
			pièce.Incrément = 1;

			return pièce;
		}


		public void NewModel(ComptaEntity compta)
		{
			// TODO...
			this.NewEmpty (compta);
		}
	}
}
