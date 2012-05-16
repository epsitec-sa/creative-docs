//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.ViewSettings.Data;
using Epsitec.Cresus.Compta.Graph;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.IO
{
	/// <summary>
	/// Cette classe s'occupe de créer les réglages de présentation par défaut.
	/// </summary>
	public class DefaultViewSettings
	{
		public DefaultViewSettings(MainWindowController mainWindowController)
		{
			this.mainWindowController = mainWindowController;
			this.compta               = mainWindowController.Compta;
		}


		public void CreateDefaultViewSettings()
		{
			this.CreateDefaultViewSettingsJournal          ("Journal");
			this.CreateDefaultViewSettingsBalance          ("Balance");
			this.CreateDefaultViewSettingsExtraitDeCompte  ("ExtraitDeCompte");
			this.CreateDefaultViewSettingsBilan            ("Bilan");
			this.CreateDefaultViewSettingsPP               ("PP");
			this.CreateDefaultViewSettingsExploitation     ("Exploitation");
			this.CreateDefaultViewSettingsBudgets          ("Budgets");
			this.CreateDefaultViewSettingsRésuméPériodique ("RésuméPériodique");
			this.CreateDefaultViewSettingsTVA              ("TVA");
			this.CreateDefaultViewSettingsLogin            ("Login");
			this.CreateDefaultViewSettingsOpen             ("Open");
			this.CreateDefaultViewSettingsPrint            ("Print");
			this.CreateDefaultViewSettingsSave             ("Save");
			this.CreateDefaultViewSettingsSoldes           ("Soldes");
			this.CreateDefaultViewSettingsRéglages         ("Réglages");
		}


		private void CreateDefaultViewSettingsJournal(string nomPrésentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPrésentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var viewSettings = this.CreateViewSettingsData<JournalOptions> (list, ControllerType.Journal, "Journal des écritures", searchExist, filterExist, optionsExist);
			}

#if false
			{
				var viewSettings = this.CreateViewSettingsData<JournalOptions> (list, ControllerType.Journal, "1er trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (viewSettings.Filter, 1, 1, 31, 3);  // 1 janvier -> 31 mars
			}

			{
				var viewSettings = this.CreateViewSettingsData<JournalOptions> (list, ControllerType.Journal, "2ème trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (viewSettings.Filter, 1, 4, 30, 6);  // 1 avril -> 30 juin
			}

			{
				var viewSettings = this.CreateViewSettingsData<JournalOptions> (list, ControllerType.Journal, "3ème trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (viewSettings.Filter, 1, 7, 30, 9);  // 1 juillet  -> 30 septembre
			}

			{
				var viewSettings = this.CreateViewSettingsData<JournalOptions> (list, ControllerType.Journal, "4ème trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (viewSettings.Filter, 1, 10, 31, 12);  // 1 octobre -> 31 décembre
			}

			{
				var viewSettings = this.CreateViewSettingsData<JournalOptions> (list, ControllerType.Journal, "Rechercher...", searchExist, filterExist, optionsExist);
				viewSettings.ShowSearch = ShowPanelMode.ShowBeginner;
			}
#endif

			this.CreateViewSettingsData (list, ControllerType.Libellés, "Libellés usuels",   true, false, false);
			this.CreateViewSettingsData (list, ControllerType.Modèles,  "Ecritures modèles", true, false, false);
			this.CreateViewSettingsData (list, ControllerType.Journaux, "Journaux",          true, false, false);

			this.Select<JournalOptions> (list, nomPrésentation);
		}

		private void CreateDefaultViewSettingsRéglages(string nomPrésentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPrésentation, "ViewSettings"));

#if false
			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = false;

			{
				var viewSettings = this.CreateViewSettingsData (list, ControllerType.PlanComptable, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
			}

			{
				var viewSettings = this.CreateViewSettingsData (list, ControllerType.PlanComptable, "Actifs", searchExist, filterExist, optionsExist);
				this.SearchAdaptCatégorie (viewSettings.Filter, CatégorieDeCompte.Actif);
			}

			{
				var viewSettings = this.CreateViewSettingsData (list, ControllerType.PlanComptable, "Passifs", searchExist, filterExist, optionsExist);
				this.SearchAdaptCatégorie (viewSettings.Filter, CatégorieDeCompte.Passif);
			}

			{
				var viewSettings = this.CreateViewSettingsData (list, ControllerType.PlanComptable, "Charges", searchExist, filterExist, optionsExist);
				this.SearchAdaptCatégorie (viewSettings.Filter, CatégorieDeCompte.Charge);
			}

			{
				var viewSettings = this.CreateViewSettingsData (list, ControllerType.PlanComptable, "Produits", searchExist, filterExist, optionsExist);
				this.SearchAdaptCatégorie (viewSettings.Filter, CatégorieDeCompte.Produit);
			}

			{
				var viewSettings = this.CreateViewSettingsData (list, ControllerType.PlanComptable, "Exploitations", searchExist, filterExist, optionsExist);
				this.SearchAdaptCatégorie (viewSettings.Filter, CatégorieDeCompte.Exploitation);
			}
#endif

			this.CreateViewSettingsData (list, ControllerType.PlanComptable,   "Plan comptable",        true,  true,  false);
			this.CreateViewSettingsData (list, ControllerType.Monnaies,        "Monnaies",              false, false, false);
			this.CreateViewSettingsData (list, ControllerType.Périodes,        "Périodes comptables",   true,  false, false);
			this.CreateViewSettingsData (list, ControllerType.PiècesGenerator, "Générateurs n° pièces", false, false, false);
			this.CreateViewSettingsData (list, ControllerType.Utilisateurs,    "Utilisateurs",          false, false, false);
			this.CreateViewSettingsData (list, ControllerType.Réglages,        "Réglages avancés",      false, false, false);

			this.Select (list, nomPrésentation);
		}

		private void CreateDefaultViewSettingsBalance(string nomPrésentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPrésentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var viewSettings = this.CreateViewSettingsData<BalanceOptions> (list, ControllerType.Balance, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
			}

			{
				var viewSettings = this.CreateViewSettingsData<BalanceOptions> (list, ControllerType.Balance, "Vue d'ensemble", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptProfondeur (viewSettings.Filter, 1, 2);
			}

			{
				var viewSettings = this.CreateViewSettingsData<BalanceOptions> (list, ControllerType.Balance, "Histogramme des comptes importants", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptProfondeur (viewSettings.Filter, 4, int.MaxValue);

				this.OptionsAdaptGraph (viewSettings.Options);
				viewSettings.Options.GraphOptions.Mode = GraphMode.SideBySide;
				viewSettings.Options.GraphOptions.PrimaryDimension = 0;
				viewSettings.Options.GraphOptions.SecondaryDimension = 1;
				viewSettings.Options.GraphOptions.HasThreshold0 = true;
				viewSettings.Options.GraphOptions.ThresholdValue0 = 0.01m;
				viewSettings.Options.GraphOptions.HasThreshold1 = true;
				viewSettings.Options.GraphOptions.ThresholdValue1 = 0.02m;
			}

			{
				var viewSettings = this.CreateViewSettingsData<BalanceOptions> (list, ControllerType.Balance, "Secteurs des comptes importants", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptProfondeur (viewSettings.Filter, 4, int.MaxValue);

				this.OptionsAdaptGraph (viewSettings.Options);
				viewSettings.Options.GraphOptions.Mode = GraphMode.Pie;
				viewSettings.Options.GraphOptions.ExplodedPieFactor = 0;
				viewSettings.Options.GraphOptions.PiePercents = false;
				viewSettings.Options.GraphOptions.PieValues = true;
				viewSettings.Options.GraphOptions.PrimaryDimension = 0;
				viewSettings.Options.GraphOptions.SecondaryDimension = 1;
				viewSettings.Options.GraphOptions.HasThreshold0 = true;
				viewSettings.Options.GraphOptions.ThresholdValue0 = 0.02m;
				viewSettings.Options.GraphOptions.HasThreshold1 = true;
				viewSettings.Options.GraphOptions.ThresholdValue1 = 0.02m;
			}

			this.Select<BalanceOptions> (list, nomPrésentation);
		}

		private void CreateDefaultViewSettingsExtraitDeCompte(string nomPrésentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPrésentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var viewSettings = this.CreateViewSettingsData<ExtraitDeCompteOptions> (list, ControllerType.Extrait, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
			}

			{
				var viewSettings = this.CreateViewSettingsData<ExtraitDeCompteOptions> (list, ControllerType.Extrait, "Graphique du solde", searchExist, filterExist, optionsExist);

				this.OptionsAdaptGraph (viewSettings.Options);
				viewSettings.Options.GraphOptions.Mode = GraphMode.Lines;
			}

			this.Select<ExtraitDeCompteOptions> (list, nomPrésentation);
		}

		private void CreateDefaultViewSettingsBilan(string nomPrésentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPrésentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var viewSettings = this.CreateViewSettingsData<BilanOptions> (list, ControllerType.Bilan, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

#if false
			{
				var viewSettings = this.CreateViewSettingsData<BilanOptions> (list, ControllerType.Bilan, "1er trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptDate (viewSettings.Filter, 1, 1, 31, 3);  // 1 janvier -> 31 mars
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<BilanOptions> (list, ControllerType.Bilan, "2ème trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptDate (viewSettings.Filter, 1, 4, 30, 6);  // 1 avril -> 30 juin
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<BilanOptions> (list, ControllerType.Bilan, "3ème trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptDate (viewSettings.Filter, 1, 7, 30, 9);  // 1 juillet  -> 30 septembre
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<BilanOptions> (list, ControllerType.Bilan, "4ème trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptDate (viewSettings.Filter, 1, 10, 31, 12);  // 1 octobre -> 31 décembre
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}
#endif

			{
				var viewSettings = this.CreateViewSettingsData<BilanOptions> (list, ControllerType.Bilan, "Vue d'ensemble du budget", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptProfondeur (viewSettings.Filter, 1, 2);
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.Budget, ComparisonDisplayMode.Montant);
			}

			this.Select<BilanOptions> (list, nomPrésentation);
		}

		private void CreateDefaultViewSettingsPP(string nomPrésentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPrésentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var viewSettings = this.CreateViewSettingsData<PPOptions> (list, ControllerType.PP, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

#if false
			{
				var viewSettings = this.CreateViewSettingsData<PPOptions> (list, ControllerType.PP, "1er trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptDate (viewSettings.Filter, 1, 1, 31, 3);  // 1 janvier -> 31 mars
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<PPOptions> (list, ControllerType.PP, "2ème trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptDate (viewSettings.Filter, 1, 4, 30, 6);  // 1 avril -> 30 juin
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<PPOptions> (list, ControllerType.PP, "3ème trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptDate (viewSettings.Filter, 1, 7, 30, 9);  // 1 juillet  -> 30 septembre
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<PPOptions> (list, ControllerType.PP, "4ème trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptDate (viewSettings.Filter, 1, 10, 31, 12);  // 1 octobre -> 31 décembre
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}
#endif

			{
				var viewSettings = this.CreateViewSettingsData<PPOptions> (list, ControllerType.PP, "Vue d'ensemble du budget", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptProfondeur (viewSettings.Filter, 1, 2);
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.Budget, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<PPOptions> (list, ControllerType.PP, "Comparaison avec le budget", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptProfondeur (viewSettings.Filter, 4, int.MaxValue);
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.Budget, ComparisonDisplayMode.Montant);

				this.OptionsAdaptGraph (viewSettings.Options);
				viewSettings.Options.GraphOptions.Mode = GraphMode.SideBySide;
			}

			this.Select<PPOptions> (list, nomPrésentation);
		}

		private void CreateDefaultViewSettingsExploitation(string nomPrésentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPrésentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var viewSettings = this.CreateViewSettingsData<ExploitationOptions> (list, ControllerType.Exploitation, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
			}

			this.Select<ExploitationOptions> (list, nomPrésentation);
		}

		private void CreateDefaultViewSettingsBudgets(string nomPrésentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPrésentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = false;

			{
				var viewSettings = this.CreateViewSettingsData (list, ControllerType.Budgets, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPrésentation);
		}

		private void CreateDefaultViewSettingsRésuméPériodique(string nomPrésentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPrésentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var viewSettings = this.CreateViewSettingsData<RésuméPériodiqueOptions> (list, ControllerType.RésuméPériodique, "Charges et produits", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptCatégorie (viewSettings.Filter, CatégorieDeCompte.Charge | CatégorieDeCompte.Produit);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptProfondeur (viewSettings.Filter, 4, int.MaxValue);
			}

			{
				var viewSettings = this.CreateViewSettingsData<RésuméPériodiqueOptions> (list, ControllerType.RésuméPériodique, "Tout", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
			}

			{
				var viewSettings = this.CreateViewSettingsData<RésuméPériodiqueOptions> (list, ControllerType.RésuméPériodique, "Histogramme des soldes", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptCatégorie (viewSettings.Filter, CatégorieDeCompte.Charge | CatégorieDeCompte.Produit);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptProfondeur (viewSettings.Filter, 4, int.MaxValue);

				this.OptionsAdaptGraph (viewSettings.Options);
				viewSettings.Options.GraphOptions.Mode = GraphMode.Stacked;
				viewSettings.Options.GraphOptions.HasThreshold0 = true;
				viewSettings.Options.GraphOptions.ThresholdValue0 = 0.02m;
				viewSettings.Options.GraphOptions.HasThreshold1 = true;
				viewSettings.Options.GraphOptions.ThresholdValue1 = 0.02m;
			}

			{
				var viewSettings = this.CreateViewSettingsData<RésuméPériodiqueOptions> (list, ControllerType.RésuméPériodique, "Secteurs des soldes", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptCatégorie (viewSettings.Filter, CatégorieDeCompte.Charge | CatégorieDeCompte.Produit);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptProfondeur (viewSettings.Filter, 4, int.MaxValue);
				this.OptionsAdaptRésuméPériodique (viewSettings.Options, 1);

				this.OptionsAdaptGraph (viewSettings.Options);
				viewSettings.Options.GraphOptions.Mode = GraphMode.Pie;
				viewSettings.Options.GraphOptions.HasThreshold0 = true;
				viewSettings.Options.GraphOptions.ThresholdValue0 = 0.05m;
				viewSettings.Options.GraphOptions.HasThreshold1 = true;
				viewSettings.Options.GraphOptions.ThresholdValue1 = 0.02m;
				viewSettings.Options.GraphOptions.PrimaryDimension = 0;
				viewSettings.Options.GraphOptions.SecondaryDimension = 1;
			}

			this.Select<RésuméPériodiqueOptions> (list, nomPrésentation);
		}


		private void CreateDefaultViewSettingsTVA(string nomPrésentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPrésentation, "ViewSettings"));

			this.CreateViewSettingsData (list, ControllerType.RésuméTVA, "Résumé TVA", false, false, false);
			this.CreateViewSettingsData (list, ControllerType.CodesTVA, "Codes TVA", false, false, false);
			this.CreateViewSettingsData (list, ControllerType.ListeTVA, "Listes de taux de TVA", false, false, false);
			
			this.Select (list, nomPrésentation);
		}

		private void CreateDefaultViewSettingsLogin(string nomPrésentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPrésentation, "ViewSettings"));
			var viewSettings = this.CreateViewSettingsData (list, ControllerType.Login, DefaultViewSettings.defaultName, false, false, false);
			this.Select (list, nomPrésentation);
		}

		private void CreateDefaultViewSettingsOpen(string nomPrésentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPrésentation, "ViewSettings"));
			var viewSettings = this.CreateViewSettingsData (list, ControllerType.Open, DefaultViewSettings.defaultName, false, false, false);
			this.Select (list, nomPrésentation);
		}

		private void CreateDefaultViewSettingsPrint(string nomPrésentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPrésentation, "ViewSettings"));
			var viewSettings = this.CreateViewSettingsData (list, ControllerType.Print, DefaultViewSettings.defaultName, false, false, false);
			this.Select (list, nomPrésentation);
		}

		private void CreateDefaultViewSettingsSave(string nomPrésentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPrésentation, "ViewSettings"));
			var viewSettings = this.CreateViewSettingsData (list, ControllerType.Save, DefaultViewSettings.defaultName, false, false, false);
			this.Select (list, nomPrésentation);
		}

		private void CreateDefaultViewSettingsSoldes(string nomPrésentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPrésentation, "ViewSettings"));
			var viewSettings = this.CreateViewSettingsData (list, ControllerType.Soldes, DefaultViewSettings.defaultName, false, false, false);
			this.Select (list, nomPrésentation);
		}


		private ViewSettingsData CreateViewSettingsData<T>(ViewSettingsList list, ControllerType type, FormattedText name, bool searchExist, bool filterExist, bool optionsExist)
			where T : AbstractOptions, new ()
		{
			var viewSettings = this.CreateViewSettingsData (list, type, name, searchExist, filterExist, optionsExist);

			viewSettings.Options = new T ();
			viewSettings.Options.SetComptaEntity (this.compta);
			viewSettings.Options.Clear ();

			return viewSettings;
		}

		private ViewSettingsData CreateViewSettingsData(ViewSettingsList list, ControllerType type, FormattedText name, bool searchExist, bool filterExist, bool optionsExist)
		{
			var viewSettings = new ViewSettingsData ()
			{
				Name           = name,
				Readonly       = true,
				Permanent      = true,
				ControllerType = type,
				Search         = new SearchData (),
				Filter         = new SearchData (),
				ShowSearch     = searchExist  ? ShowPanelMode.Nop : ShowPanelMode.DoesNotExist,
				ShowFilter     = filterExist  ? ShowPanelMode.Nop : ShowPanelMode.DoesNotExist,
				ShowOptions    = optionsExist ? ShowPanelMode.Nop : ShowPanelMode.DoesNotExist,
			};

			list.List.Add (viewSettings);

			return viewSettings;
		}


		private void SearchAdd(SearchData data)
		{
			data.NodesData[0].TabsData.Add (new SearchTabData ());
		}

		private void SearchAdaptForNonZero(SearchData data)
		{
			var tab = data.NodesData[0].TabsData.Last ();

			tab.SearchText.FromText = Converters.MontantToString (0, null);
			tab.SearchText.Mode     = SearchMode.WholeContent;
			tab.SearchText.Invert   = true;
			tab.Column              = ColumnType.Solde;

			this.SearchAdaptOrMode (data);
		}

		private void SearchAdaptProfondeur(SearchData data, int minProfondeur, int maxProfondeur)
		{
			var tab = data.NodesData[0].TabsData.Last ();

			tab.SearchText.FromText = (minProfondeur == int.MaxValue) ? null : Converters.IntToString (minProfondeur);
			tab.SearchText.ToText   = (maxProfondeur == int.MaxValue) ? null : Converters.IntToString (maxProfondeur);
			tab.SearchText.Mode     = SearchMode.Interval;
			tab.Column              = ColumnType.Profondeur;

			this.SearchAdaptOrMode (data);
		}

		private void SearchAdaptDate(SearchData data, int startDay, int startMonth, int endDay, int endMonth)
		{
			var tab = data.NodesData[0].TabsData.Last ();

			int year = Date.Today.Year;

			tab.SearchText.FromText = Converters.DateToString (new Date (year, startMonth, startDay));
			tab.SearchText.ToText   = Converters.DateToString (new Date (year,   endMonth,   endDay));
			tab.SearchText.Mode     = SearchMode.Interval;
			tab.Column              = ColumnType.Date;

			this.SearchAdaptOrMode (data);
		}

		private void SearchAdaptCatégorie(SearchData data, CatégorieDeCompte catégorie)
		{
			var tab = data.NodesData[0].TabsData.Last ();

			tab.SearchText.FromText = Converters.CatégoriesToString (catégorie);
			tab.SearchText.Mode     = SearchMode.Jokers;
			tab.Column              = ColumnType.Catégorie;

			this.SearchAdaptOrMode (data);
		}

		private void SearchAdaptOrMode(SearchData data)
		{
			data.NodesData[0].OrMode = (data.NodesData[0].TabsData.Count == 1);
		}


		private void OptionsAdaptDouble(AbstractOptions options, ComparisonShowed showed, ComparisonDisplayMode mode)
		{
			var o = options as DoubleOptions;

			o.ZeroDisplayedInWhite  = true;
			o.HasGraphics           = (showed != ComparisonShowed.None);
			o.ComparisonEnable      = (showed != ComparisonShowed.None);
			o.ComparisonShowed      = showed;
			o.ComparisonDisplayMode = mode;
		}


		private void OptionsAdaptRésuméPériodique(AbstractOptions options, int numberOfMonths)
		{
			var o = options as RésuméPériodiqueOptions;

			o.NumberOfMonths = numberOfMonths;
		}


		private void OptionsAdaptGraph(AbstractOptions options)
		{
			options.ViewGraph = true;
		}


		private void Select<T>(ViewSettingsList list, string nomPrésentation)
			where T : AbstractOptions, new ()
		{
			this.Select (list, nomPrésentation);

			if (list.Selected.Options != null)
			{
				list.Selected.Options.CopyTo (this.mainWindowController.GetSettingsOptions<T> (this.GetKey (nomPrésentation, "Options"), this.compta));
			}
		}

		private void Select(ViewSettingsList list, string nomPrésentation)
		{
			list.SelectedIndex = 0;  // sélectionne "Réglages standards"

			if (list.Selected.Search != null)
			{
				list.Selected.Search.CopyTo (this.mainWindowController.GetSettingsSearchData (this.GetKey (nomPrésentation, "Search")));
			}

			if (list.Selected.Filter != null)
			{
				list.Selected.Filter.CopyTo (this.mainWindowController.GetSettingsSearchData (this.GetKey (nomPrésentation, "Filter")));
			}
		}

		private string GetKey(string nomPrésentation, string typeName)
		{
			//	Retourne par exemple "Présentation.Journal.Search".
			return "Présentation." + nomPrésentation + "." + typeName;
		}


		private readonly static FormattedText				defaultName = "Standard";

		private readonly MainWindowController				mainWindowController;
		private readonly ComptaEntity						compta;
	}
}
