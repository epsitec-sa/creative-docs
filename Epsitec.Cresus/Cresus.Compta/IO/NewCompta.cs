//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

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
			compta.Nom         = "vide";
			compta.Description = null;

			compta.PlanComptable.Clear ();
			compta.Périodes.Clear ();
			compta.Journaux.Clear ();
			compta.PiècesGenerator.Clear ();
			compta.Utilisateurs.Clear ();

			compta.PiècesGenerator.Add (this.CreatePiècesGenerator ());
			compta.Utilisateurs.Add (this.CreateUtilisateur ());
		}

		public void NewEmpty(ComptaEntity compta)
		{
			compta.Nom         = "vide";
			compta.Description = null;

			compta.PlanComptable.Clear ();
			compta.Périodes.Clear ();
			compta.Journaux.Clear ();
			compta.PiècesGenerator.Clear ();
			compta.Utilisateurs.Clear ();

			this.CreatePériodes (compta);
			compta.PiècesGenerator.Add (this.CreatePiècesGenerator ());
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

			utilisateur.Utilisateur = "Admin";
			utilisateur.MotDePasse  = Strings.ComputeMd5Hash ("epsitec");
			utilisateur.UserAccess  = UserAccess.Full;

			return utilisateur;
		}

		private ComptaPiècesGeneratorEntity CreatePiècesGenerator()
		{
			//	Crée le générateur de numéros de pièces de base.
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
