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
			NewCompta.CreateTauxTVA (compta);
			NewCompta.CreateListesTVA (compta);
		}

		private static void CreateTauxTVA(ComptaEntity compta)
		{
			//	Crée les taux de TVA nécessaires pour toutes comptabilités.
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
					Nom     = "Réduit 1",
					DateFin = new Date (2010, 12, 31),
					Taux    = 0.024m,
				};
				compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom       = "Réduit 2",
					DateDébut = new Date (2011, 1, 1),
					Taux      = 0.025m,
					ParDéfaut = true,
				};
				compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom     = "Hébergement 1",
					DateFin = new Date (2010, 12, 31),
					Taux    = 0.036m,
				};
				compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom       = "Hébergement 2",
					DateDébut = new Date (2011, 1, 1),
					Taux      = 0.038m,
					ParDéfaut = true,
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
					DateDébut = new Date (2011, 1, 1),
					Taux      = 0.08m,
					ParDéfaut = true,
				};
				compta.TauxTVA.Add (taux);
			}
		}

		private static void CreateListesTVA(ComptaEntity compta)
		{
			//	Crée les listes de taux de TVA nécessaires pour toutes comptabilités.
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
					Nom = "Réduit",
				};

				liste.Taux.Add (compta.TauxTVA.Where (x => x.Nom == "Réduit 1").FirstOrDefault ());
				liste.Taux.Add (compta.TauxTVA.Where (x => x.Nom == "Réduit 2").FirstOrDefault ());

				compta.ListesTVA.Add (liste);
			}

			{
				var liste = new ComptaListeTVAEntity
				{
					Nom = "Hébergement",
				};

				liste.Taux.Add (compta.TauxTVA.Where (x => x.Nom == "Hébergement 1").FirstOrDefault ());
				liste.Taux.Add (compta.TauxTVA.Where (x => x.Nom == "Hébergement 2").FirstOrDefault ());

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
