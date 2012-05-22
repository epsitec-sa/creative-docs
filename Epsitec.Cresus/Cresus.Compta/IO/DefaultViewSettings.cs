//	Copyright � 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			this.CreateDefaultViewSettingsJournal          ("Journal");
			this.CreateDefaultViewSettingsBalance          ("Balance");
			this.CreateDefaultViewSettingsExtrait          ("Extrait");
			this.CreateDefaultViewSettingsBilan            ("Bilan");
			this.CreateDefaultViewSettingsPP               ("PP");
			this.CreateDefaultViewSettingsExploitation     ("Exploitation");
			this.CreateDefaultViewSettingsBudgets          ("Budgets");
			this.CreateDefaultViewSettingsR�sum�P�riodique ("R�sum�P�riodique");
			this.CreateDefaultViewSettingsTVA              ("TVA");
			this.CreateDefaultViewSettingsLogin            ("Login");
			this.CreateDefaultViewSettingsOpen             ("Open");
			this.CreateDefaultViewSettingsPrint            ("Print");
			this.CreateDefaultViewSettingsSave             ("Save");
			this.CreateDefaultViewSettingsSoldes           ("Soldes");
			this.CreateDefaultViewSettingsR�glages         ("R�glages");
		}


		private void CreateDefaultViewSettingsJournal(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

			this.CreateViewSettingsData<JournalOptions> (list, ControllerType.Journal, "Journal des �critures", true, true,  true);

			this.CreateViewSettingsData (list, ControllerType.Libell�s, "Libell�s usuels",       true, false, false);
			this.CreateViewSettingsData (list, ControllerType.Mod�les,  "Ecritures mod�les",     true, false, false);
			this.CreateViewSettingsData (list, ControllerType.Journaux, "Journaux",              true, false, false);

			this.Select<JournalOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsR�glages(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

			this.CreateViewSettingsData (list, ControllerType.PlanComptable,   "Plan comptable",        true,  true,  false);
			this.CreateViewSettingsData (list, ControllerType.Monnaies,        "Monnaies",              false, false, false);
			this.CreateViewSettingsData (list, ControllerType.P�riodes,        "P�riodes comptables",   true,  false, false);
			this.CreateViewSettingsData (list, ControllerType.Pi�cesGenerator, "G�n�rateurs n� pi�ces", false, false, false);
			this.CreateViewSettingsData (list, ControllerType.Utilisateurs,    "Utilisateurs",          false, false, false);
			this.CreateViewSettingsData (list, ControllerType.R�glages,        "R�glages avanc�s",      false, false, false);

			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsBalance(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

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

			this.Select<BalanceOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsExtrait(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

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

			this.Select<ExtraitDeCompteOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsBilan(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

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

			this.Select<BilanOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsPP(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

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

			this.Select<PPOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsExploitation(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var viewSettings = this.CreateViewSettingsData<ExploitationOptions> (list, ControllerType.Exploitation, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.BaseFilter);
			}

			this.Select<ExploitationOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsBudgets(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = false;

			{
				var viewSettings = this.CreateViewSettingsData (list, ControllerType.Budgets, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsR�sum�P�riodique(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

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

			this.Select<R�sum�P�riodiqueOptions> (list, nomPr�sentation);
		}


		private void CreateDefaultViewSettingsTVA(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

			this.CreateViewSettingsData<R�sum�TVAOptions> (list, ControllerType.R�sum�TVA, "R�sum� TVA", false, false, false);
			this.CreateViewSettingsData (list, ControllerType.CodesTVA, "Codes TVA", false, false, false);
			this.CreateViewSettingsData (list, ControllerType.ListeTVA, "Listes de taux de TVA", false, false, false);
			
			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsLogin(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));
			this.CreateViewSettingsData (list, ControllerType.Login, DefaultViewSettings.defaultName, false, false, false);
			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsOpen(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));
			this.CreateViewSettingsData (list, ControllerType.Open, DefaultViewSettings.defaultName, false, false, false);
			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsPrint(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));
			this.CreateViewSettingsData (list, ControllerType.Print, DefaultViewSettings.defaultName, false, false, false);
			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsSave(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));
			this.CreateViewSettingsData (list, ControllerType.Save, DefaultViewSettings.defaultName, false, false, false);
			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsSoldes(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));
			this.CreateViewSettingsData<SoldesOptions> (list, ControllerType.Soldes, DefaultViewSettings.defaultName, false, false, false);
			this.Select (list, nomPr�sentation);
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


		private void Select<T>(ViewSettingsList list, string nomPr�sentation)
			where T : AbstractOptions, new ()
		{
			this.Select (list, nomPr�sentation);

			if (list.Selected.BaseOptions != null)
			{
				//?list.Selected.BaseOptions.CopyTo (this.mainWindowController.GetSettingsOptions<T> (this.GetKey (nomPr�sentation, "Options"), this.compta));
			}
		}

		private void Select(ViewSettingsList list, string nomPr�sentation)
		{
			list.SelectedIndex = 0;  // s�lectionne "R�glages standards"

			if (list.Selected.BaseFilter != null)
			{
				//?list.Selected.BaseFilter.CopyTo (this.mainWindowController.GetSettingsSearchData (this.GetKey (nomPr�sentation, "Filter")));
			}
		}

		private string GetKey(string nomPr�sentation, string typeName)
		{
			//	Retourne par exemple "Pr�sentation.Journal.Search".
			return "Pr�sentation." + nomPr�sentation + "." + typeName;
		}


		private readonly static FormattedText				defaultName = "Standard";

		private readonly MainWindowController				mainWindowController;
		private readonly ComptaEntity						compta;
	}
}
