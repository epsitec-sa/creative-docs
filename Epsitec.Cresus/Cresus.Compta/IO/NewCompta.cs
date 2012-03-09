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
			NewCompta.CreateTauxTVA (compta);
			NewCompta.CreateListesTVA (compta);
		}

		private static void CreateTauxTVA(ComptaEntity compta)
		{
			//	Cr�e les taux de TVA n�cessaires pour toutes comptabilit�s.
			compta.TauxTVA.Clear ();

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom     = "Exclu",
					Taux    = 0.0m,
				};
				compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom     = "R�duit 1",
					DateFin = new Date (2010, 12, 31),
					Taux    = 0.024m,
				};
				compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom       = "R�duit 2",
					DateD�but = new Date (2011, 1, 1),
					Taux      = 0.025m,
					ParD�faut = true,
				};
				compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom     = "H�bergement 1",
					DateFin = new Date (2010, 12, 31),
					Taux    = 0.036m,
				};
				compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom       = "H�bergement 2",
					DateD�but = new Date (2011, 1, 1),
					Taux      = 0.038m,
					ParD�faut = true,
				};
				compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom     = "Normal 1",
					DateFin = new Date (2010, 12, 31),
					Taux    = 0.076m,
				};
				compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom       = "Normal 2",
					DateD�but = new Date (2011, 1, 1),
					Taux      = 0.08m,
					ParD�faut = true,
				};
				compta.TauxTVA.Add (taux);
			}
		}

		private static void CreateListesTVA(ComptaEntity compta)
		{
			//	Cr�e les listes de taux de TVA n�cessaires pour toutes comptabilit�s.
			compta.ListesTVA.Clear ();

			{
				var liste = new ComptaListeTVAEntity
				{
					Nom = "Exclu",
				};

				liste.Taux.Add (compta.TauxTVA.Where (x => x.Nom == "Exclu").FirstOrDefault ());

				compta.ListesTVA.Add (liste);
			}

			{
				var liste = new ComptaListeTVAEntity
				{
					Nom = "R�duit",
				};

				liste.Taux.Add (compta.TauxTVA.Where (x => x.Nom == "R�duit 1").FirstOrDefault ());
				liste.Taux.Add (compta.TauxTVA.Where (x => x.Nom == "R�duit 2").FirstOrDefault ());

				compta.ListesTVA.Add (liste);
			}

			{
				var liste = new ComptaListeTVAEntity
				{
					Nom = "H�bergement",
				};

				liste.Taux.Add (compta.TauxTVA.Where (x => x.Nom == "H�bergement 1").FirstOrDefault ());
				liste.Taux.Add (compta.TauxTVA.Where (x => x.Nom == "H�bergement 2").FirstOrDefault ());

				compta.ListesTVA.Add (liste);
			}

			{
				var liste = new ComptaListeTVAEntity
				{
					Nom = "Normal",
				};

				liste.Taux.Add (compta.TauxTVA.Where (x => x.Nom == "Normal 1").FirstOrDefault ());
				liste.Taux.Add (compta.TauxTVA.Where (x => x.Nom == "Normal 2").FirstOrDefault ());

				compta.ListesTVA.Add (liste);
			}
		}


		public static void NewModel(ComptaEntity compta)
		{
			// TODO...
			NewCompta.NewEmpty (compta);
		}
	}
}
