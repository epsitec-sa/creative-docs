//	Copyright � 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public static class Pr�sentations
	{
		public static string GetViewSettingsKey(ControllerType type)
		{
			//	Retourne la cl� d'acc�s pour les donn�es ViewSettingsList.
			var cmd = Pr�sentations.GetCommand (type);
			return Pr�sentations.GetViewSettingsKey (cmd);
		}

		public static string GetViewSettingsKey(Command cmd)
		{
			//	Retourne la cl� d'acc�s pour les donn�es ViewSettingsList.
			return string.Concat (cmd.Name + ".ViewSettings");
		}

		public static string GetSearchSettingsKey(ControllerType type)
		{
			//	Retourne la cl� d'acc�s pour les donn�es SearchData.
			return string.Concat ("Pr�sentation." + Pr�sentations.ControllerTypeToString (type) + ".Search");
		}

		public static string GetPermanentsSettingsKey(ControllerType type)
		{
			//	Retourne la cl� d'acc�s pour les donn�es AbstractPermanents.
			return string.Concat ("Pr�sentation." + Pr�sentations.ControllerTypeToString (type) + ".Permanents");
		}


		public static FormattedText GetGroupName(ControllerType type)
		{
			//	Retourne le nom du groupe d'une pr�snetation.
			//	Par exemple, ControllerType.Mod�les retourne "Journal", puisque la pr�sentation
			//	des �critures mod�les fait partir du groupe Res.Commands.Pr�sentation.Journal.
			var cmd = Pr�sentations.GetCommand (type);
			if (cmd != null)
			{
				type = Pr�sentations.GetControllerType (cmd);

				switch (type)
				{
					case ControllerType.Journal:
						return "Journal";

					case ControllerType.PlanComptable:
						return "R�glages";

					case ControllerType.R�sum�TVA:
						return "TVA";
				}
	
				return Pr�sentations.GetName (type);
			}

			return FormattedText.Empty;
		}

		public static FormattedText GetName(ControllerType type)
		{
			//	Retourne le nom d'une pr�snetation.
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

				case ControllerType.Mod�les:
					return "Ecritures mod�les";

				case ControllerType.Libell�s:
					return "Libell�s usuels";

				case ControllerType.P�riodes:
					return "P�riodes comptables";

				case ControllerType.Journaux:
					return "Journaux";

				case ControllerType.Journal:
					return "Journal des �critures";

				case ControllerType.PlanComptable:
					return "Plan comptable";

				case ControllerType.Balance:
					return "Balance de v�rification";

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

				case ControllerType.R�sum�P�riodique:
					return "R�sum� p�riodique";

				case ControllerType.Soldes:
					return "Soldes";

				case ControllerType.R�sum�TVA:
					return "R�sum� TVA";

				case ControllerType.D�compteTVA:
					return "D�compte TVA";

				case ControllerType.CodesTVA:
					return "Codes TVA";

				case ControllerType.ListeTVA:
					return "Listes de taux de TVA";

				case ControllerType.Diff�rencesChange:
					return "Diff�rences de change";

				case ControllerType.Pi�cesGenerator:
					return "G�n�rateur de pi�ces";

				case ControllerType.Utilisateurs:
					return "Utilisateurs";

				case ControllerType.R�glages:
					return "R�glages";

				default:
					return FormattedText.Empty;
			}
		}

		public static string GetTabIcon(ViewSettingsData data)
		{
			//	Retourne l'ic�ne � afficher dans l'onglet, � gauche de la description.
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
				type == ControllerType.R�sum�P�riodique ||
				type == ControllerType.Soldes ||
				type == ControllerType.R�sum�TVA)
			{
				return "Edit.Tab.System";
			}
			else
			{
				return "Edit.Tab.Settings";
			}
#else
			return Pr�sentations.GetIcon (data.ControllerType);
#endif
		}

		public static string GetIcon(ControllerType type)
		{
			//	Retourne l'ic�ne d'une pr�sentation.
			return "Pr�sentation." + type.ToString ();  // requiert des noms d'ic�nes parfaitement synchrones avec les noms des ControllerType !
		}


		#region Liste de pr�sentations
		public static void SetPr�sentationType(ref string list, ControllerType type, bool state)
		{
			//	Ajoute ou enl�ve une pr�sentation dans une liste.
			var s = Pr�sentations.ControllerTypeToString (type);

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

		public static bool ContainsPr�sentationType(string list, ControllerType type)
		{
			//	Indique si une liste contient une pr�sentation.
			if (string.IsNullOrEmpty (list))
			{
				return false;
			}
			else
			{
				var words = list.Split (',');
				return words.Contains (Pr�sentations.ControllerTypeToString (type));
			}
		}

		public static int Pr�sentationTypeCount(string list)
		{
			//	Retourne le nombre de pr�sentations contenues dans une liste.
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
			foreach (var type in Pr�sentations.ControllerTypes)
			{
				if (Pr�sentations.ControllerTypeToString (type) == text)
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
			foreach (var cmd in Pr�sentations.Pr�sentationCommands)
			{
				var mainType = ControllerType.Unknown;

				foreach (var t in Pr�sentations.GetControllerTypes (cmd))
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
			//	Retourne la commande de base permettant d'acc�der � une pr�sentation.
			foreach (var cmd in Pr�sentations.Pr�sentationCommands)
			{
				foreach (var t in Pr�sentations.GetControllerTypes (cmd))
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
			//	Liste de toutes les pr�sentations existantes.
			//	L'ordre est utilis� lors du choix d'un utilisateur, pour afficher les boutons � cocher.
			get
			{
				foreach (var cmd in Pr�sentations.Pr�sentationCommands)
				{
					foreach (var type in Pr�sentations.GetControllerTypes (cmd))
					{
						yield return type;
					}
				}
			}
		}

		public static ControllerType GetControllerType(Command cmd)
		{
			//	Retourne la pr�sentation principale associ�e � une commande.
			return Pr�sentations.GetControllerTypes (cmd).FirstOrDefault ();
		}

		public static IEnumerable<ControllerType> GetControllerTypes(Command cmd)
		{
			//	Retourne la liste des pr�sentations associ�es � une commande.
			//	La premi�re est toujours la pr�sentation priccipale.
			if (cmd == Res.Commands.Pr�sentation.Open)
			{
				yield return ControllerType.Open;
			}

			if (cmd == Res.Commands.Pr�sentation.Save)
			{
				yield return ControllerType.Save;
			}

			if (cmd == Res.Commands.Pr�sentation.Login)
			{
				yield return ControllerType.Login;
			}

			if (cmd == Res.Commands.Pr�sentation.Print)
			{
				yield return ControllerType.Print;
			}

			if (cmd == Res.Commands.Pr�sentation.Journal)
			{
				yield return ControllerType.Journal;  // pr�sentation principale
				yield return ControllerType.Libell�s;
				yield return ControllerType.Mod�les;
				yield return ControllerType.Journaux;
			}

			if (cmd == Res.Commands.Pr�sentation.Balance)
			{
				yield return ControllerType.Balance;
			}

			if (cmd == Res.Commands.Pr�sentation.Extrait)
			{
				yield return ControllerType.Extrait;
			}

			if (cmd == Res.Commands.Pr�sentation.Bilan)
			{
				yield return ControllerType.Bilan;
			}

			if (cmd == Res.Commands.Pr�sentation.PP)
			{
				yield return ControllerType.PP;
			}

			if (cmd == Res.Commands.Pr�sentation.Exploitation)
			{
				yield return ControllerType.Exploitation;
			}

			if (cmd == Res.Commands.Pr�sentation.Budgets)
			{
				yield return ControllerType.Budgets;
			}

			if (cmd == Res.Commands.Pr�sentation.Diff�rencesChange)
			{
				yield return ControllerType.Diff�rencesChange;
			}

			if (cmd == Res.Commands.Pr�sentation.R�sum�P�riodique)
			{
				yield return ControllerType.R�sum�P�riodique;
			}

			if (cmd == Res.Commands.Pr�sentation.Soldes)
			{
				yield return ControllerType.Soldes;
			}

			if (cmd == Res.Commands.Pr�sentation.TVA)
			{
				yield return ControllerType.R�sum�TVA;  // pr�sentation principale
				yield return ControllerType.D�compteTVA;
				yield return ControllerType.CodesTVA;
				yield return ControllerType.ListeTVA;
			}

			if (cmd == Res.Commands.Pr�sentation.R�glages)
			{
				yield return ControllerType.PlanComptable;  // pr�sentation principale
				yield return ControllerType.Monnaies;
				yield return ControllerType.P�riodes;
				yield return ControllerType.Pi�cesGenerator;
				yield return ControllerType.Utilisateurs;
				yield return ControllerType.R�glages;
			}
		}

		public static IEnumerable<Command> Pr�sentationCommands
		{
			//	Liste de toutes les commandes de groupes de pr�sentations.
			//	L'ordre n'a pas d'importance.
			get
			{
				yield return Res.Commands.Pr�sentation.Open;
				yield return Res.Commands.Pr�sentation.Save;
				yield return Res.Commands.Pr�sentation.Login;
				yield return Res.Commands.Pr�sentation.Print;
				yield return Res.Commands.Pr�sentation.Journal;
				yield return Res.Commands.Pr�sentation.Balance;
				yield return Res.Commands.Pr�sentation.Extrait;
				yield return Res.Commands.Pr�sentation.Bilan;
				yield return Res.Commands.Pr�sentation.PP;
				yield return Res.Commands.Pr�sentation.Exploitation;
				yield return Res.Commands.Pr�sentation.Budgets;
				yield return Res.Commands.Pr�sentation.Diff�rencesChange;
				yield return Res.Commands.Pr�sentation.R�sum�P�riodique;
				yield return Res.Commands.Pr�sentation.Soldes;
				yield return Res.Commands.Pr�sentation.TVA;
				yield return Res.Commands.Pr�sentation.R�glages;
			}
		}
	}
}
