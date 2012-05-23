//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Settings.Data;
using Epsitec.Cresus.Compta.Settings.Controllers;
using Epsitec.Cresus.Compta.ViewSettings.Data;
using Epsitec.Cresus.Compta.Options.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Helpers
{
	public static class Présentations
	{
		public static string GetViewSettingsKey(ControllerType type)
		{
			//	Retourne la clé d'accès pour les données ViewSettingsList.
			var cmd = Présentations.GetCommand (type);
			return Présentations.GetViewSettingsKey (cmd);
		}

		public static string GetViewSettingsKey(Command cmd)
		{
			//	Retourne la clé d'accès pour les données ViewSettingsList.
			return string.Concat (cmd.Name + ".ViewSettings");
		}

		public static string GetSearchSettingsKey(ControllerType type)
		{
			//	Retourne la clé d'accès pour les données SearchData.
			return string.Concat ("Présentation." + Présentations.ControllerTypeToString (type) + ".Search");
		}

		public static string GetPermanentsSettingsKey(ControllerType type)
		{
			//	Retourne la clé d'accès pour les données AbstractPermanents.
			return string.Concat ("Présentation." + Présentations.ControllerTypeToString (type) + ".Permanents");
		}


		public static FormattedText GetGroupName(ControllerType type)
		{
			//	Retourne le nom du groupe d'une présnetation.
			//	Par exemple, ControllerType.Modèles retourne "Journal", puisque la présentation
			//	des écritures modèles fait partir du groupe Res.Commands.Présentation.Journal.
			var cmd = Présentations.GetCommand (type);
			if (cmd != null)
			{
				type = Présentations.GetControllerType (cmd);

				switch (type)
				{
					case ControllerType.Journal:
						return "Journal";

					case ControllerType.PlanComptable:
						return "Réglages";

					case ControllerType.RésuméTVA:
						return "TVA";
				}
	
				return Présentations.GetName (type);
			}

			return FormattedText.Empty;
		}

		public static FormattedText GetName(ControllerType type)
		{
			//	Retourne le nom d'une présnetation.
			switch (type)
			{
				case ControllerType.Open:
					return "Ouverture";

				case ControllerType.Save:
					return "Enregistrement";

				case ControllerType.Print:
					return "Impression";

				case ControllerType.Login:
					return "Indentification";

				case ControllerType.Modèles:
					return "Ecritures modèles";

				case ControllerType.Libellés:
					return "Libellés usuels";

				case ControllerType.Périodes:
					return "Périodes comptables";

				case ControllerType.Journaux:
					return "Journaux";

				case ControllerType.Journal:
					return "Journal des écritures";

				case ControllerType.PlanComptable:
					return "Plan comptable";

				case ControllerType.Balance:
					return "Balance de vérification";

				case ControllerType.Extrait:
					return "Extrait de compte";

				case ControllerType.Bilan:
					return "Bilan";

				case ControllerType.PP:
					return "Pertes et Profits";

				case ControllerType.Exploitation:
					return "Compte d'exploitation";

				case ControllerType.Budgets:
					return "Budgets";

				case ControllerType.Monnaies:
					return "Taux de change";

				case ControllerType.RésuméPériodique:
					return "Résumé périodique";

				case ControllerType.Soldes:
					return "Soldes";

				case ControllerType.RésuméTVA:
					return "Résumé TVA";

				case ControllerType.DécompteTVA:
					return "Décompte TVA";

				case ControllerType.CodesTVA:
					return "Codes TVA";

				case ControllerType.ListeTVA:
					return "Listes de taux de TVA";

				case ControllerType.DifférencesChange:
					return "Différences de change";

				case ControllerType.PiècesGenerator:
					return "Générateur de pièces";

				case ControllerType.Utilisateurs:
					return "Utilisateurs";

				case ControllerType.Réglages:
					return "Réglages";

				default:
					return FormattedText.Empty;
			}
		}

		public static string GetTabIcon(ViewSettingsData data)
		{
			//	Retourne l'icône à afficher dans l'onglet, à gauche de la description.
			if (data.Readonly == false)
			{
				return "Edit.Tab.User";
			}

#if false
			var type = data.ControllerType;

			if (type == ControllerType.Journal ||
				type == ControllerType.Extrait ||
				type == ControllerType.PlanComptable ||
				type == ControllerType.Balance ||
				type == ControllerType.Extrait ||
				type == ControllerType.Bilan ||
				type == ControllerType.PP ||
				type == ControllerType.Exploitation ||
				type == ControllerType.RésuméPériodique ||
				type == ControllerType.Soldes ||
				type == ControllerType.RésuméTVA)
			{
				return "Edit.Tab.System";
			}
			else
			{
				return "Edit.Tab.Settings";
			}
#else
			return Présentations.GetIcon (data.ControllerType);
#endif
		}

		public static string GetIcon(ControllerType type)
		{
			//	Retourne l'icône d'une présentation.
			return "Présentation." + type.ToString ();  // requiert des noms d'icônes parfaitement synchrones avec les noms des ControllerType !
		}


		#region Liste de présentations
		public static void SetPrésentationType(ref string list, ControllerType type, bool state)
		{
			//	Ajoute ou enlève une présentation dans une liste.
			var s = Présentations.ControllerTypeToString (type);

			if (string.IsNullOrEmpty (list))
			{
				if (state)
				{
					list = s;
				}
			}
			else
			{
				var words = list.Split (',').ToList ();
				int i = words.IndexOf (s);

				if (i == -1)
				{
					if (state)
					{
						list += "," + s;
					}
				}
				else
				{
					if (!state)
					{
						words.RemoveAt (i);
						list = string.Join (",", words);
					}
				}
			}
		}

		public static bool ContainsPrésentationType(string list, ControllerType type)
		{
			//	Indique si une liste contient une présentation.
			if (string.IsNullOrEmpty (list))
			{
				return false;
			}
			else
			{
				var words = list.Split (',');
				return words.Contains (Présentations.ControllerTypeToString (type));
			}
		}

		public static int PrésentationTypeCount(string list)
		{
			//	Retourne le nombre de présentations contenues dans une liste.
			if (string.IsNullOrEmpty (list))
			{
				return 0;
			}
			else
			{
				var words = list.Split (',');
				return words.Length;
			}
		}

		public static ControllerType StringToControllerType(string text)
		{
			foreach (var type in Présentations.ControllerTypes)
			{
				if (Présentations.ControllerTypeToString (type) == text)
				{
					return type;
				}
			}

			return ControllerType.Unknown;
		}

		public static string ControllerTypeToString(ControllerType type)
		{
			return type.ToString ();
		}
		#endregion


		public static ControllerType GetGroupControllerType(ControllerType type)
		{
			foreach (var cmd in Présentations.PrésentationCommands)
			{
				var mainType = ControllerType.Unknown;

				foreach (var t in Présentations.GetControllerTypes (cmd))
				{
					if (mainType == ControllerType.Unknown)
					{
						mainType = t;
					}

					if (t == type)
					{
						return mainType;
					}
				}
			}

			return ControllerType.Unknown;
		}

		private static Command GetCommand(ControllerType type)
		{
			//	Retourne la commande de base permettant d'accéder à une présentation.
			foreach (var cmd in Présentations.PrésentationCommands)
			{
				foreach (var t in Présentations.GetControllerTypes (cmd))
				{
					if (t == type)
					{
						return cmd;
					}
				}
			}

			return null;
		}


		public static IEnumerable<ControllerType> ControllerTypes
		{
			//	Liste de toutes les présentations existantes.
			//	L'ordre est utilisé lors du choix d'un utilisateur, pour afficher les boutons à cocher.
			get
			{
				foreach (var cmd in Présentations.PrésentationCommands)
				{
					foreach (var type in Présentations.GetControllerTypes (cmd))
					{
						yield return type;
					}
				}
			}
		}

		public static ControllerType GetControllerType(Command cmd)
		{
			//	Retourne la présentation principale associée à une commande.
			return Présentations.GetControllerTypes (cmd).FirstOrDefault ();
		}

		public static IEnumerable<ControllerType> GetControllerTypes(Command cmd)
		{
			//	Retourne la liste des présentations associées à une commande.
			//	La première est toujours la présentation priccipale.
			if (cmd == Res.Commands.Présentation.Open)
			{
				yield return ControllerType.Open;
			}

			if (cmd == Res.Commands.Présentation.Save)
			{
				yield return ControllerType.Save;
			}

			if (cmd == Res.Commands.Présentation.Login)
			{
				yield return ControllerType.Login;
			}

			if (cmd == Res.Commands.Présentation.Print)
			{
				yield return ControllerType.Print;
			}

			if (cmd == Res.Commands.Présentation.Journal)
			{
				yield return ControllerType.Journal;  // présentation principale
				yield return ControllerType.Libellés;
				yield return ControllerType.Modèles;
				yield return ControllerType.Journaux;
			}

			if (cmd == Res.Commands.Présentation.Balance)
			{
				yield return ControllerType.Balance;
			}

			if (cmd == Res.Commands.Présentation.Extrait)
			{
				yield return ControllerType.Extrait;
			}

			if (cmd == Res.Commands.Présentation.Bilan)
			{
				yield return ControllerType.Bilan;
			}

			if (cmd == Res.Commands.Présentation.PP)
			{
				yield return ControllerType.PP;
			}

			if (cmd == Res.Commands.Présentation.Exploitation)
			{
				yield return ControllerType.Exploitation;
			}

			if (cmd == Res.Commands.Présentation.Budgets)
			{
				yield return ControllerType.Budgets;
			}

			if (cmd == Res.Commands.Présentation.DifférencesChange)
			{
				yield return ControllerType.DifférencesChange;
			}

			if (cmd == Res.Commands.Présentation.RésuméPériodique)
			{
				yield return ControllerType.RésuméPériodique;
			}

			if (cmd == Res.Commands.Présentation.Soldes)
			{
				yield return ControllerType.Soldes;
			}

			if (cmd == Res.Commands.Présentation.TVA)
			{
				yield return ControllerType.RésuméTVA;  // présentation principale
				yield return ControllerType.DécompteTVA;
				yield return ControllerType.CodesTVA;
				yield return ControllerType.ListeTVA;
			}

			if (cmd == Res.Commands.Présentation.Réglages)
			{
				yield return ControllerType.PlanComptable;  // présentation principale
				yield return ControllerType.Monnaies;
				yield return ControllerType.Périodes;
				yield return ControllerType.PiècesGenerator;
				yield return ControllerType.Utilisateurs;
				yield return ControllerType.Réglages;
			}
		}

		public static IEnumerable<Command> PrésentationCommands
		{
			//	Liste de toutes les commandes de groupes de présentations.
			//	L'ordre n'a pas d'importance.
			get
			{
				yield return Res.Commands.Présentation.Open;
				yield return Res.Commands.Présentation.Save;
				yield return Res.Commands.Présentation.Login;
				yield return Res.Commands.Présentation.Print;
				yield return Res.Commands.Présentation.Journal;
				yield return Res.Commands.Présentation.Balance;
				yield return Res.Commands.Présentation.Extrait;
				yield return Res.Commands.Présentation.Bilan;
				yield return Res.Commands.Présentation.PP;
				yield return Res.Commands.Présentation.Exploitation;
				yield return Res.Commands.Présentation.Budgets;
				yield return Res.Commands.Présentation.DifférencesChange;
				yield return Res.Commands.Présentation.RésuméPériodique;
				yield return Res.Commands.Présentation.Soldes;
				yield return Res.Commands.Présentation.TVA;
				yield return Res.Commands.Présentation.Réglages;
			}
		}
	}
}
