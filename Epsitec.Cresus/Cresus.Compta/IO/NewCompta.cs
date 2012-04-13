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
	public static class NewCompta
	{
		public static void NewNull(ComptaEntity compta)
		{
			compta.Nom         = "vide";
			compta.Description = null;

			compta.PlanComptable.Clear ();
			compta.Périodes.Clear ();
			compta.Journaux.Clear ();
			compta.PiècesGenerator.Clear ();
			compta.Utilisateurs.Clear ();

			compta.PiècesGenerator.Add (NewCompta.CreatePiècesGenerator ());
			compta.Utilisateurs.Add (NewCompta.CreateAdminUser ());
			//?compta.Utilisateurs.Add (NewCompta.CreateFirstUser ());
		}

		public static void NewEmpty(ComptaEntity compta)
		{
			compta.Nom         = "vide";
			compta.Description = null;

			compta.PlanComptable.Clear ();
			compta.Périodes.Clear ();
			compta.Journaux.Clear ();
			compta.PiècesGenerator.Clear ();
			compta.Utilisateurs.Clear ();

			NewCompta.CreateTVA (compta);
			NewCompta.CreateMonnaies (compta);
			NewCompta.CreatePériodes (compta);
			compta.PiècesGenerator.Add (NewCompta.CreatePiècesGenerator ());
			compta.Journaux.Add (NewCompta.CreateJournal (compta));
			compta.Utilisateurs.Add (NewCompta.CreateAdminUser ());
			//?compta.Utilisateurs.Add (NewCompta.CreateFirstUser ());
		}

		public static void CreatePériodes(ComptaEntity compta, int pastCount = -1, int postCount = 10)
		{
			compta.Périodes.Clear ();

			var now = Date.Today;
			for (int year = now.Year+pastCount; year < now.Year+postCount; year++)
			{
				compta.Périodes.Add (NewCompta.CreatePériode (year));
			}
		}

		private static ComptaPériodeEntity CreatePériode(int year)
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

		private static ComptaJournalEntity CreateJournal(ComptaEntity compta)
		{
			//	Crée un journal principal.
			var journal = new ComptaJournalEntity ();

			journal.Id  = compta.GetJournalId ();
			journal.Nom = "Principal";

			return journal;
		}

		private static ComptaUtilisateurEntity CreateAdminUser()
		{
			//	Crée l'utilisteur administrateur. Il est préférable qu'il n'ait pas de mot de passe,
			//	pour permettre un login automatique à l'ouverture.
			var utilisateur = new ComptaUtilisateurEntity ();

			utilisateur.Utilisateur = "Admin";
			utilisateur.NomComplet  = "Administrateur";
			//?utilisateur.MotDePasse  = Strings.ComputeMd5Hash ("epsitec");
			utilisateur.Admin       = true;

			return utilisateur;
		}

		private static ComptaUtilisateurEntity CreateFirstUser()
		{
			//	Crée un premier utilisateur neutre, sans mot de passe.
			var utilisateur = new ComptaUtilisateurEntity ();

			utilisateur.Utilisateur = "Moi";
			utilisateur.NomComplet  = "Moi-même";

			string list = null;
			foreach (var cmd in Converters.PrésentationCommands)
			{
				if (cmd != Res.Commands.Présentation.Utilisateurs &&
					cmd != Res.Commands.Présentation.Réglages)
				{
					Converters.SetPrésentationCommand (ref list, cmd, true);
				}
			}
			utilisateur.Présentations = list;

			return utilisateur;
		}

		private static ComptaPiècesGeneratorEntity CreatePiècesGenerator()
		{
			//	Crée le générateur de numéros de pièces de base.
			var pièce = new ComptaPiècesGeneratorEntity ();

			pièce.Nom       = "Base";
			pièce.Format    = "1";
			pièce.Numéro    = 1;
			pièce.Incrément = 1;

			return pièce;
		}


		public static void CreateTVA(ComptaEntity compta)
		{
			//	Crée tout ce qui concerne la TVA pour toutes comptabilités.
			compta.ListesTVA.Clear ();

			{
				var liste = new ComptaListeTVAEntity
				{
					Nom = "Exclu",
				};

				liste.TauxParDéfaut = NewCompta.CreateTauxTVA (liste, 1995, 0.0m);

				compta.ListesTVA.Add (liste);
			}

			{
				var liste = new ComptaListeTVAEntity
				{
					Nom = "Réduit",
				};

				NewCompta.CreateTauxTVA (liste, 1995, 2.0m);
				NewCompta.CreateTauxTVA (liste, 1999, 2.3m);
				NewCompta.CreateTauxTVA (liste, 2001, 2.4m);
				liste.TauxParDéfaut = NewCompta.CreateTauxTVA (liste, 2011, 2.5m);

				compta.ListesTVA.Add (liste);
			}

			{
				var liste = new ComptaListeTVAEntity
				{
					Nom = "Hébergement",
				};

				NewCompta.CreateTauxTVA (liste, 1996, 3.0m);
				NewCompta.CreateTauxTVA (liste, 1999, 3.5m);
				NewCompta.CreateTauxTVA (liste, 2001, 3.6m);
				liste.TauxParDéfaut = NewCompta.CreateTauxTVA (liste, 2011, 3.8m);

				compta.ListesTVA.Add (liste);
			}

			{
				var liste = new ComptaListeTVAEntity
				{
					Nom = "Normal",
				};

				NewCompta.CreateTauxTVA (liste, 1995, 6.5m);
				NewCompta.CreateTauxTVA (liste, 1999, 7.5m);
				NewCompta.CreateTauxTVA (liste, 2001, 7.6m);
				liste.TauxParDéfaut = NewCompta.CreateTauxTVA (liste, 2011, 8.0m);

				compta.ListesTVA.Add (liste);
			}
		}

		private static ComptaTauxTVAEntity CreateTauxTVA(ComptaListeTVAEntity liste, int year, decimal taux)
		{
			var entity = new ComptaTauxTVAEntity ()
			{
				DateDébut = new Date (year, 1, 1),
				Taux      = taux/100,
			};

			liste.Taux.Add (entity);

			return entity;
		}


		public static void CreateMonnaies(ComptaEntity compta)
		{
			//	Crée tout ce qui concerne les taux de change pour toutes comptabilités.
			compta.Monnaies.Clear ();

			NewCompta.CreateMonnaie (compta, "CHR", "Franc",     1.0m);
			NewCompta.CreateMonnaie (compta, "EUR", "Euro",      1.2m);
			NewCompta.CreateMonnaie (compta, "USD", "Dollar US", 1.1m);
		}

		private static void CreateMonnaie(ComptaEntity compta, FormattedText code, FormattedText description, decimal cours)
		{
			var taux = new ComptaMonnaieEntity ()
			{
				CodeISO     = code,
				Description = description,
				Décimales   = 2,
				Arrondi     = 0.01m,
				Cours       = cours,
				Unité       = 1,
			};

			ComptaCompteEntity compte;

			compte = NewCompta.GetCompte (compta, "Gains de change");
			if (compte != null)
			{
				taux.CompteGain = compte;
			}

			compte = NewCompta.GetCompte (compta, "Pertes de change");
			if (compte != null)
			{
				taux.ComptePerte = compte;
			}

			compta.Monnaies.Add (taux);
		}

		private static ComptaCompteEntity GetCompte(ComptaEntity compta, FormattedText titre)
		{
			return compta.PlanComptable.Where (x => x.Titre == titre).FirstOrDefault ();
		}


		public static void NewModel(ComptaEntity compta)
		{
			// TODO...
			NewCompta.NewEmpty (compta);
		}
	}
}
