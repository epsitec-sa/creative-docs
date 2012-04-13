//	Copyright � 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	/// Cette classe s'occupe de cr�er une nouvelle comptabilit� de toutes pi�ces.
	/// </summary>
	public static class NewCompta
	{
		public static void NewNull(ComptaEntity compta)
		{
			compta.Nom         = "vide";
			compta.Description = null;

			compta.PlanComptable.Clear ();
			compta.P�riodes.Clear ();
			compta.Journaux.Clear ();
			compta.Pi�cesGenerator.Clear ();
			compta.Utilisateurs.Clear ();

			compta.Pi�cesGenerator.Add (NewCompta.CreatePi�cesGenerator ());
			compta.Utilisateurs.Add (NewCompta.CreateAdminUser ());
			//?compta.Utilisateurs.Add (NewCompta.CreateFirstUser ());
		}

		public static void NewEmpty(ComptaEntity compta)
		{
			compta.Nom         = "vide";
			compta.Description = null;

			compta.PlanComptable.Clear ();
			compta.P�riodes.Clear ();
			compta.Journaux.Clear ();
			compta.Pi�cesGenerator.Clear ();
			compta.Utilisateurs.Clear ();

			NewCompta.CreateTVA (compta);
			NewCompta.CreateMonnaies (compta);
			NewCompta.CreateP�riodes (compta);
			compta.Pi�cesGenerator.Add (NewCompta.CreatePi�cesGenerator ());
			compta.Journaux.Add (NewCompta.CreateJournal (compta));
			compta.Utilisateurs.Add (NewCompta.CreateAdminUser ());
			//?compta.Utilisateurs.Add (NewCompta.CreateFirstUser ());
		}

		public static void CreateP�riodes(ComptaEntity compta, int pastCount = -1, int postCount = 10)
		{
			compta.P�riodes.Clear ();

			var now = Date.Today;
			for (int year = now.Year+pastCount; year < now.Year+postCount; year++)
			{
				compta.P�riodes.Add (NewCompta.CreateP�riode (year));
			}
		}

		private static ComptaP�riodeEntity CreateP�riode(int year)
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

		private static ComptaJournalEntity CreateJournal(ComptaEntity compta)
		{
			//	Cr�e un journal principal.
			var journal = new ComptaJournalEntity ();

			journal.Id  = compta.GetJournalId ();
			journal.Nom = "Principal";

			return journal;
		}

		private static ComptaUtilisateurEntity CreateAdminUser()
		{
			//	Cr�e l'utilisteur administrateur. Il est pr�f�rable qu'il n'ait pas de mot de passe,
			//	pour permettre un login automatique � l'ouverture.
			var utilisateur = new ComptaUtilisateurEntity ();

			utilisateur.Utilisateur = "Admin";
			utilisateur.NomComplet  = "Administrateur";
			//?utilisateur.MotDePasse  = Strings.ComputeMd5Hash ("epsitec");
			utilisateur.Admin       = true;

			return utilisateur;
		}

		private static ComptaUtilisateurEntity CreateFirstUser()
		{
			//	Cr�e un premier utilisateur neutre, sans mot de passe.
			var utilisateur = new ComptaUtilisateurEntity ();

			utilisateur.Utilisateur = "Moi";
			utilisateur.NomComplet  = "Moi-m�me";

			string list = null;
			foreach (var cmd in Converters.Pr�sentationCommands)
			{
				if (cmd != Res.Commands.Pr�sentation.Utilisateurs &&
					cmd != Res.Commands.Pr�sentation.R�glages)
				{
					Converters.SetPr�sentationCommand (ref list, cmd, true);
				}
			}
			utilisateur.Pr�sentations = list;

			return utilisateur;
		}

		private static ComptaPi�cesGeneratorEntity CreatePi�cesGenerator()
		{
			//	Cr�e le g�n�rateur de num�ros de pi�ces de base.
			var pi�ce = new ComptaPi�cesGeneratorEntity ();

			pi�ce.Nom       = "Base";
			pi�ce.Format    = "1";
			pi�ce.Num�ro    = 1;
			pi�ce.Incr�ment = 1;

			return pi�ce;
		}


		public static void CreateTVA(ComptaEntity compta)
		{
			//	Cr�e tout ce qui concerne la TVA pour toutes comptabilit�s.
			compta.ListesTVA.Clear ();

			{
				var liste = new ComptaListeTVAEntity
				{
					Nom = "Exclu",
				};

				liste.TauxParD�faut = NewCompta.CreateTauxTVA (liste, 1995, 0.0m);

				compta.ListesTVA.Add (liste);
			}

			{
				var liste = new ComptaListeTVAEntity
				{
					Nom = "R�duit",
				};

				NewCompta.CreateTauxTVA (liste, 1995, 2.0m);
				NewCompta.CreateTauxTVA (liste, 1999, 2.3m);
				NewCompta.CreateTauxTVA (liste, 2001, 2.4m);
				liste.TauxParD�faut = NewCompta.CreateTauxTVA (liste, 2011, 2.5m);

				compta.ListesTVA.Add (liste);
			}

			{
				var liste = new ComptaListeTVAEntity
				{
					Nom = "H�bergement",
				};

				NewCompta.CreateTauxTVA (liste, 1996, 3.0m);
				NewCompta.CreateTauxTVA (liste, 1999, 3.5m);
				NewCompta.CreateTauxTVA (liste, 2001, 3.6m);
				liste.TauxParD�faut = NewCompta.CreateTauxTVA (liste, 2011, 3.8m);

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
				liste.TauxParD�faut = NewCompta.CreateTauxTVA (liste, 2011, 8.0m);

				compta.ListesTVA.Add (liste);
			}
		}

		private static ComptaTauxTVAEntity CreateTauxTVA(ComptaListeTVAEntity liste, int year, decimal taux)
		{
			var entity = new ComptaTauxTVAEntity ()
			{
				DateD�but = new Date (year, 1, 1),
				Taux      = taux/100,
			};

			liste.Taux.Add (entity);

			return entity;
		}


		public static void CreateMonnaies(ComptaEntity compta)
		{
			//	Cr�e tout ce qui concerne les taux de change pour toutes comptabilit�s.
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
				D�cimales   = 2,
				Arrondi     = 0.01m,
				Cours       = cours,
				Unit�       = 1,
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
