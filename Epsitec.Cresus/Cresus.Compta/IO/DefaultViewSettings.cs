//	Copyright � 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

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
	/// Cette classe s'occupe de cr�er les r�glages de pr�sentation par d�faut.
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
			this.CreateDefaultViewSettingsJournal          (Res.Commands.Pr�sentation.Journal);
			this.CreateDefaultViewSettingsBalance          (Res.Commands.Pr�sentation.Balance);
			this.CreateDefaultViewSettingsExtrait          (Res.Commands.Pr�sentation.Extrait);
			this.CreateDefaultViewSettingsBilan            (Res.Commands.Pr�sentation.Bilan);
			this.CreateDefaultViewSettingsPP               (Res.Commands.Pr�sentation.PP);
			this.CreateDefaultViewSettingsExploitation     (Res.Commands.Pr�sentation.Exploitation);
			this.CreateDefaultViewSettingsBudgets          (Res.Commands.Pr�sentation.Budgets);
			this.CreateDefaultViewSettingsR�sum�P�riodique (Res.Commands.Pr�sentation.R�sum�P�riodique);
			this.CreateDefaultViewSettingsTVA              (Res.Commands.Pr�sentation.TVA);
			this.CreateDefaultViewSettingsLogin            (Res.Commands.Pr�sentation.Login);
			this.CreateDefaultViewSettingsOpen             (Res.Commands.Pr�sentation.Open);
			this.CreateDefaultViewSettingsPrint            (Res.Commands.Pr�sentation.Print);
			this.CreateDefaultViewSettingsSave             (Res.Commands.Pr�sentation.Save);
			this.CreateDefaultViewSettingsSoldes           (Res.Commands.Pr�sentation.Soldes);
			this.CreateDefaultViewSettingsR�glages         (Res.Commands.Pr�sentation.R�glages);
		}


		private void CreateDefaultViewSettingsJournal(Command cmd)
		{
			var list = this.GetList (cmd);

			this.CreateViewSettingsData<JournalOptions> (list, ControllerType.Journal, "Journal des �critures", true, true,  true);

			this.CreateViewSettingsData (list, ControllerType.Libell�s, "Libell�s usuels",       true, false, false);
			this.CreateViewSettingsData (list, ControllerType.Mod�les,  "Ecritures mod�les",     true, false, false);
			this.CreateViewSettingsData (list, ControllerType.Journaux, "Journaux",              true, false, false);

			this.Select (list);
		}

		private void CreateDefaultViewSettingsR�glages(Command cmd)
		{
			var list = this.GetList (cmd);

			this.CreateViewSettingsData (list, ControllerType.PlanComptable,   "Plan comptable",        true,  true,  false);
			this.CreateViewSettingsData (list, ControllerType.Monnaies,        "Monnaies",              false, false, false);
			this.CreateViewSettingsData (list, ControllerType.P�riodes,        "P�riodes comptables",   true,  false, false);
			this.CreateViewSettingsData (list, ControllerType.Pi�cesGenerator, "G�n�rateurs n� pi�ces", false, false, false);
			this.CreateViewSettingsData (list, ControllerType.Utilisateurs,    "Utilisateurs",          false, false, false);
			this.CreateViewSettingsData (list, ControllerType.R�glages,        "R�glages avanc�s",      false, false, false);

			this.Select (list);
		}

		private void CreateDefaultViewSettingsBalance(Command cmd)
		{
			var list = this.GetList (cmd);

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var viewSettings = this.CreateViewSettingsData<BalanceOptions> (list, ControllerType.Balance, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.BaseFilter);
			}

			{
				var viewSettings = this.CreateViewSettingsData<BalanceOptions> (list, ControllerType.Balance, "Vue d'ensemble", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.BaseFilter);
				this.SearchAdd (viewSettings.BaseFilter);
				this.SearchAdaptProfondeur (viewSettings.BaseFilter, 1, 2);
			}

			{
				var viewSettings = this.CreateViewSettingsData<BalanceOptions> (list, ControllerType.Balance, "Histogramme des comptes importants", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.BaseFilter);
				this.SearchAdd (viewSettings.BaseFilter);
				this.SearchAdaptProfondeur (viewSettings.BaseFilter, 4, int.MaxValue);

				this.OptionsAdaptGraph (viewSettings.BaseOptions);
				viewSettings.BaseOptions.GraphOptions.Mode = GraphMode.SideBySide;
				viewSettings.BaseOptions.GraphOptions.PrimaryDimension = 0;
				viewSettings.BaseOptions.GraphOptions.SecondaryDimension = 1;
				viewSettings.BaseOptions.GraphOptions.HasThreshold0 = true;
				viewSettings.BaseOptions.GraphOptions.ThresholdValue0 = 0.01m;
				viewSettings.BaseOptions.GraphOptions.HasThreshold1 = true;
				viewSettings.BaseOptions.GraphOptions.ThresholdValue1 = 0.02m;
			}

			{
				var viewSettings = this.CreateViewSettingsData<BalanceOptions> (list, ControllerType.Balance, "Secteurs des comptes importants", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.BaseFilter);
				this.SearchAdd (viewSettings.BaseFilter);
				this.SearchAdaptProfondeur (viewSettings.BaseFilter, 4, int.MaxValue);

				this.OptionsAdaptGraph (viewSettings.BaseOptions);
				viewSettings.BaseOptions.GraphOptions.Mode = GraphMode.Pie;
				viewSettings.BaseOptions.GraphOptions.ExplodedPieFactor = 0;
				viewSettings.BaseOptions.GraphOptions.PiePercents = false;
				viewSettings.BaseOptions.GraphOptions.PieValues = true;
				viewSettings.BaseOptions.GraphOptions.PrimaryDimension = 0;
				viewSettings.BaseOptions.GraphOptions.SecondaryDimension = 1;
				viewSettings.BaseOptions.GraphOptions.HasThreshold0 = true;
				viewSettings.BaseOptions.GraphOptions.ThresholdValue0 = 0.02m;
				viewSettings.BaseOptions.GraphOptions.HasThreshold1 = true;
				viewSettings.BaseOptions.GraphOptions.ThresholdValue1 = 0.02m;
			}

			this.Select (list);
		}

		private void CreateDefaultViewSettingsExtrait(Command cmd)
		{
			var list = this.GetList (cmd);

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var viewSettings = this.CreateViewSettingsData<ExtraitDeCompteOptions> (list, ControllerType.Extrait, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
			}

			{
				var viewSettings = this.CreateViewSettingsData<ExtraitDeCompteOptions> (list, ControllerType.Extrait, "Graphique du solde", searchExist, filterExist, optionsExist);

				this.OptionsAdaptGraph (viewSettings.BaseOptions);
				viewSettings.BaseOptions.GraphOptions.Mode = GraphMode.Lines;
			}

			this.Select (list);
		}

		private void CreateDefaultViewSettingsBilan(Command cmd)
		{
			var list = this.GetList (cmd);

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var viewSettings = this.CreateViewSettingsData<BilanOptions> (list, ControllerType.Bilan, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.BaseFilter);
				this.OptionsAdaptDouble (viewSettings.BaseOptions, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<BilanOptions> (list, ControllerType.Bilan, "Vue d'ensemble du budget", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.BaseFilter);
				this.SearchAdd (viewSettings.BaseFilter);
				this.SearchAdaptProfondeur (viewSettings.BaseFilter, 1, 2);
				this.OptionsAdaptDouble (viewSettings.BaseOptions, ComparisonShowed.Budget, ComparisonDisplayMode.Montant);
			}

			this.Select (list);
		}

		private void CreateDefaultViewSettingsPP(Command cmd)
		{
			var list = this.GetList (cmd);

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var viewSettings = this.CreateViewSettingsData<PPOptions> (list, ControllerType.PP, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.BaseFilter);
				this.OptionsAdaptDouble (viewSettings.BaseOptions, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<PPOptions> (list, ControllerType.PP, "Vue d'ensemble du budget", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.BaseFilter);
				this.SearchAdd (viewSettings.BaseFilter);
				this.SearchAdaptProfondeur (viewSettings.BaseFilter, 1, 2);
				this.OptionsAdaptDouble (viewSettings.BaseOptions, ComparisonShowed.Budget, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<PPOptions> (list, ControllerType.PP, "Comparaison avec le budget", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.BaseFilter);
				this.SearchAdd (viewSettings.BaseFilter);
				this.SearchAdaptProfondeur (viewSettings.BaseFilter, 4, int.MaxValue);
				this.OptionsAdaptDouble (viewSettings.BaseOptions, ComparisonShowed.Budget, ComparisonDisplayMode.Montant);

				this.OptionsAdaptGraph (viewSettings.BaseOptions);
				viewSettings.BaseOptions.GraphOptions.Mode = GraphMode.SideBySide;
			}

			this.Select (list);
		}

		private void CreateDefaultViewSettingsExploitation(Command cmd)
		{
			var list = this.GetList (cmd);

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var viewSettings = this.CreateViewSettingsData<ExploitationOptions> (list, ControllerType.Exploitation, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.BaseFilter);
			}

			this.Select (list);
		}

		private void CreateDefaultViewSettingsBudgets(Command cmd)
		{
			var list = this.GetList (cmd);

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = false;

			{
				var viewSettings = this.CreateViewSettingsData (list, ControllerType.Budgets, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list);
		}

		private void CreateDefaultViewSettingsR�sum�P�riodique(Command cmd)
		{
			var list = this.GetList (cmd);

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var viewSettings = this.CreateViewSettingsData<R�sum�P�riodiqueOptions> (list, ControllerType.R�sum�P�riodique, "Charges et produits", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.BaseFilter);
				this.SearchAdd (viewSettings.BaseFilter);
				this.SearchAdaptCat�gorie (viewSettings.BaseFilter, Cat�gorieDeCompte.Charge | Cat�gorieDeCompte.Produit);
				this.SearchAdd (viewSettings.BaseFilter);
				this.SearchAdaptProfondeur (viewSettings.BaseFilter, 4, int.MaxValue);
			}

			{
				var viewSettings = this.CreateViewSettingsData<R�sum�P�riodiqueOptions> (list, ControllerType.R�sum�P�riodique, "Tout", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.BaseFilter);
			}

			{
				var viewSettings = this.CreateViewSettingsData<R�sum�P�riodiqueOptions> (list, ControllerType.R�sum�P�riodique, "Histogramme des soldes", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.BaseFilter);
				this.SearchAdd (viewSettings.BaseFilter);
				this.SearchAdaptCat�gorie (viewSettings.BaseFilter, Cat�gorieDeCompte.Charge | Cat�gorieDeCompte.Produit);
				this.SearchAdd (viewSettings.BaseFilter);
				this.SearchAdaptProfondeur (viewSettings.BaseFilter, 4, int.MaxValue);

				this.OptionsAdaptGraph (viewSettings.BaseOptions);
				viewSettings.BaseOptions.GraphOptions.Mode = GraphMode.Stacked;
				viewSettings.BaseOptions.GraphOptions.HasThreshold0 = true;
				viewSettings.BaseOptions.GraphOptions.ThresholdValue0 = 0.02m;
				viewSettings.BaseOptions.GraphOptions.HasThreshold1 = true;
				viewSettings.BaseOptions.GraphOptions.ThresholdValue1 = 0.02m;
			}

			{
				var viewSettings = this.CreateViewSettingsData<R�sum�P�riodiqueOptions> (list, ControllerType.R�sum�P�riodique, "Secteurs des soldes", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.BaseFilter);
				this.SearchAdd (viewSettings.BaseFilter);
				this.SearchAdaptCat�gorie (viewSettings.BaseFilter, Cat�gorieDeCompte.Charge | Cat�gorieDeCompte.Produit);
				this.SearchAdd (viewSettings.BaseFilter);
				this.SearchAdaptProfondeur (viewSettings.BaseFilter, 4, int.MaxValue);
				this.OptionsAdaptR�sum�P�riodique (viewSettings.BaseOptions, 1);

				this.OptionsAdaptGraph (viewSettings.BaseOptions);
				viewSettings.BaseOptions.GraphOptions.Mode = GraphMode.Pie;
				viewSettings.BaseOptions.GraphOptions.HasThreshold0 = true;
				viewSettings.BaseOptions.GraphOptions.ThresholdValue0 = 0.05m;
				viewSettings.BaseOptions.GraphOptions.HasThreshold1 = true;
				viewSettings.BaseOptions.GraphOptions.ThresholdValue1 = 0.02m;
				viewSettings.BaseOptions.GraphOptions.PrimaryDimension = 0;
				viewSettings.BaseOptions.GraphOptions.SecondaryDimension = 1;
			}

			this.Select (list);
		}


		private void CreateDefaultViewSettingsTVA(Command cmd)
		{
			var list = this.GetList (cmd);

			this.CreateViewSettingsData<R�sum�TVAOptions> (list, ControllerType.R�sum�TVA, "R�sum� TVA", false, false, false);
			this.CreateViewSettingsData (list, ControllerType.CodesTVA, "Codes TVA", false, false, false);
			this.CreateViewSettingsData (list, ControllerType.ListeTVA, "Listes de taux de TVA", false, false, false);

			this.Select (list);
		}

		private void CreateDefaultViewSettingsLogin(Command cmd)
		{
			var list = this.GetList (cmd);
			this.CreateViewSettingsData (list, ControllerType.Login, DefaultViewSettings.defaultName, false, false, false);
			this.Select (list);
		}

		private void CreateDefaultViewSettingsOpen(Command cmd)
		{
			var list = this.GetList (cmd);
			this.CreateViewSettingsData (list, ControllerType.Open, DefaultViewSettings.defaultName, false, false, false);
			this.Select (list);
		}

		private void CreateDefaultViewSettingsPrint(Command cmd)
		{
			var list = this.GetList (cmd);
			this.CreateViewSettingsData (list, ControllerType.Print, DefaultViewSettings.defaultName, false, false, false);
			this.Select (list);
		}

		private void CreateDefaultViewSettingsSave(Command cmd)
		{
			var list = this.GetList (cmd);
			this.CreateViewSettingsData (list, ControllerType.Save, DefaultViewSettings.defaultName, false, false, false);
			this.Select (list);
		}

		private void CreateDefaultViewSettingsSoldes(Command cmd)
		{
			var list = this.GetList (cmd);
			this.CreateViewSettingsData<SoldesOptions> (list, ControllerType.Soldes, DefaultViewSettings.defaultName, false, false, false);
			this.Select (list);
		}


		private ViewSettingsData CreateViewSettingsData<T>(ViewSettingsList list, ControllerType type, FormattedText name, bool searchExist, bool filterExist, bool optionsExist)
			where T : AbstractOptions, new ()
		{
			var viewSettings = this.CreateViewSettingsData (list, type, name, searchExist, filterExist, optionsExist);

			viewSettings.BaseOptions = new T ();
			viewSettings.BaseOptions.SetComptaEntity (this.compta);
			viewSettings.BaseOptions.Clear ();

			return viewSettings;
		}

		private ViewSettingsData CreateViewSettingsData(ViewSettingsList list, ControllerType type, FormattedText name, bool searchExist, bool filterExist, bool optionsExist)
		{
			var viewSettings = new ViewSettingsData ()
			{
				Name           = name,
				Readonly       = true,
				ControllerType = type,
				BaseFilter     = new SearchData (),
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

		private void SearchAdaptCat�gorie(SearchData data, Cat�gorieDeCompte cat�gorie)
		{
			var tab = data.NodesData[0].TabsData.Last ();

			tab.SearchText.FromText = Converters.Cat�goriesToString (cat�gorie);
			tab.SearchText.Mode     = SearchMode.Jokers;
			tab.Column              = ColumnType.Cat�gorie;

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


		private void OptionsAdaptR�sum�P�riodique(AbstractOptions options, int numberOfMonths)
		{
			var o = options as R�sum�P�riodiqueOptions;

			o.NumberOfMonths = numberOfMonths;
		}


		private void OptionsAdaptGraph(AbstractOptions options)
		{
			options.ViewGraph = true;
		}


		private ViewSettingsList GetList(Command cmd)
		{
			return this.mainWindowController.GetViewSettingsList (Pr�sentations.GetViewSettingsKey (cmd));
		}

		private void Select(ViewSettingsList list)
		{
			list.SelectedIndex = 0;  // s�lectionne "R�glages standards"
		}


		private readonly static FormattedText				defaultName = "Standard";

		private readonly MainWindowController				mainWindowController;
		private readonly ComptaEntity						compta;
	}
}
