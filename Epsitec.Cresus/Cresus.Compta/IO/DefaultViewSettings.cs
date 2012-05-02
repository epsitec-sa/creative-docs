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
			this.CreateDefaultViewSettingsPlanComptable    ("PlanComptable");
			this.CreateDefaultViewSettingsBalance          ("Balance");
			this.CreateDefaultViewSettingsExtraitDeCompte  ("ExtraitDeCompte");
			this.CreateDefaultViewSettingsBilan            ("Bilan");
			this.CreateDefaultViewSettingsPP               ("PP");
			this.CreateDefaultViewSettingsExploitation     ("Exploitation");
			this.CreateDefaultViewSettingsBudgets          ("Budgets");
			this.CreateDefaultViewSettingsJournaux         ("Journaux");
			this.CreateDefaultViewSettingsLibell�s         ("Libell�s");
			this.CreateDefaultViewSettingsMod�les          ("Mod�les");
			this.CreateDefaultViewSettingsP�riodes         ("P�riodes");
			this.CreateDefaultViewSettingsR�sum�P�riodique ("R�sum�P�riodique");
		}


		private void CreateDefaultViewSettingsJournal(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var viewSettings = this.CreateViewSettingsData<JournalOptions> (list, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
			}

			{
				var viewSettings = this.CreateViewSettingsData<JournalOptions> (list, "Premier trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (viewSettings.Filter, 1, 1, 31, 3);  // 1 janvier -> 31 mars
			}

			{
				var viewSettings = this.CreateViewSettingsData<JournalOptions> (list, "Deuxi�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (viewSettings.Filter, 1, 4, 30, 6);  // 1 avril -> 30 juin
			}

			{
				var viewSettings = this.CreateViewSettingsData<JournalOptions> (list, "Troisi�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (viewSettings.Filter, 1, 7, 30, 9);  // 1 juillet  -> 30 septembre
			}

			{
				var viewSettings = this.CreateViewSettingsData<JournalOptions> (list, "Quatri�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (viewSettings.Filter, 1, 10, 31, 12);  // 1 octobre -> 31 d�cembre
			}

			{
				var viewSettings = this.CreateViewSettingsData<JournalOptions> (list, "Rechercher...", searchExist, filterExist, optionsExist);
				viewSettings.ShowSearch = ShowPanelMode.ShowBeginner;
			}

			this.Select<JournalOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsPlanComptable(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = false;

			{
				var viewSettings = this.CreateViewSettingsData (list, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
			}

			{
				var viewSettings = this.CreateViewSettingsData (list, "Actifs", searchExist, filterExist, optionsExist);
				this.SearchAdaptCat�gorie (viewSettings.Filter, Cat�gorieDeCompte.Actif);
			}

			{
				var viewSettings = this.CreateViewSettingsData (list, "Passifs", searchExist, filterExist, optionsExist);
				this.SearchAdaptCat�gorie (viewSettings.Filter, Cat�gorieDeCompte.Passif);
			}

			{
				var viewSettings = this.CreateViewSettingsData (list, "Charges", searchExist, filterExist, optionsExist);
				this.SearchAdaptCat�gorie (viewSettings.Filter, Cat�gorieDeCompte.Charge);
			}

			{
				var viewSettings = this.CreateViewSettingsData (list, "Produits", searchExist, filterExist, optionsExist);
				this.SearchAdaptCat�gorie (viewSettings.Filter, Cat�gorieDeCompte.Produit);
			}

			{
				var viewSettings = this.CreateViewSettingsData (list, "Exploitations", searchExist, filterExist, optionsExist);
				this.SearchAdaptCat�gorie (viewSettings.Filter, Cat�gorieDeCompte.Exploitation);
			}

			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsBalance(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var viewSettings = this.CreateViewSettingsData<BalanceOptions> (list, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
			}

			{
				var viewSettings = this.CreateViewSettingsData<BalanceOptions> (list, "Vue d'ensemble", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptProfondeur (viewSettings.Filter, 1, 2);
			}

			{
				var viewSettings = this.CreateViewSettingsData<BalanceOptions> (list, "Histogramme des comptes importants", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptProfondeur (viewSettings.Filter, 4, int.MaxValue);

				this.OptionsAdaptGraph (viewSettings.Options);
				viewSettings.Options.GraphOptions.Mode = GraphMode.SideBySide;
				viewSettings.Options.GraphOptions.PrimaryDimension = 0;
				viewSettings.Options.GraphOptions.SecondaryDimension = 1;
				viewSettings.Options.GraphOptions.HasThreshold = true;
				viewSettings.Options.GraphOptions.ThresholdValue = 0.01m;
			}

			{
				var viewSettings = this.CreateViewSettingsData<BalanceOptions> (list, "Secteurs des comptes importants", searchExist, filterExist, optionsExist);
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
				viewSettings.Options.GraphOptions.HasThreshold = true;
				viewSettings.Options.GraphOptions.ThresholdValue = 0.02m;
			}

			this.Select<BalanceOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsExtraitDeCompte(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var viewSettings = this.CreateViewSettingsData<ExtraitDeCompteOptions> (list, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
			}

			{
				var viewSettings = this.CreateViewSettingsData<ExtraitDeCompteOptions> (list, "Graphique du solde", searchExist, filterExist, optionsExist);

				this.OptionsAdaptGraph (viewSettings.Options);
				viewSettings.Options.GraphOptions.Mode = GraphMode.Lines;
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
				var viewSettings = this.CreateViewSettingsData<BilanOptions> (list, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<BilanOptions> (list, "Premier trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptDate (viewSettings.Filter, 1, 1, 31, 3);  // 1 janvier -> 31 mars
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<BilanOptions> (list, "Deuxi�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptDate (viewSettings.Filter, 1, 4, 30, 6);  // 1 avril -> 30 juin
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<BilanOptions> (list, "Troisi�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptDate (viewSettings.Filter, 1, 7, 30, 9);  // 1 juillet  -> 30 septembre
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<BilanOptions> (list, "Quatri�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptDate (viewSettings.Filter, 1, 10, 31, 12);  // 1 octobre -> 31 d�cembre
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<BilanOptions> (list, "Vue d'ensemble du budget", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptProfondeur (viewSettings.Filter, 1, 2);
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.Budget, ComparisonDisplayMode.Montant);
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
				var viewSettings = this.CreateViewSettingsData<PPOptions> (list, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<PPOptions> (list, "Premier trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptDate (viewSettings.Filter, 1, 1, 31, 3);  // 1 janvier -> 31 mars
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<PPOptions> (list, "Deuxi�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptDate (viewSettings.Filter, 1, 4, 30, 6);  // 1 avril -> 30 juin
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<PPOptions> (list, "Troisi�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptDate (viewSettings.Filter, 1, 7, 30, 9);  // 1 juillet  -> 30 septembre
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<PPOptions> (list, "Quatri�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptDate (viewSettings.Filter, 1, 10, 31, 12);  // 1 octobre -> 31 d�cembre
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<PPOptions> (list, "Vue d'ensemble du budget", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptProfondeur (viewSettings.Filter, 1, 2);
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.Budget, ComparisonDisplayMode.Montant);
			}

			{
				var viewSettings = this.CreateViewSettingsData<PPOptions> (list, "Comparaison avec le budget", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptProfondeur (viewSettings.Filter, 4, int.MaxValue);
				this.OptionsAdaptDouble (viewSettings.Options, ComparisonShowed.Budget, ComparisonDisplayMode.Montant);

				this.OptionsAdaptGraph (viewSettings.Options);
				viewSettings.Options.GraphOptions.Mode = GraphMode.SideBySide;
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
				var viewSettings = this.CreateViewSettingsData<ExploitationOptions> (list, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
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
				var viewSettings = this.CreateViewSettingsData (list, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsJournaux(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var viewSettings = this.CreateViewSettingsData (list, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsLibell�s(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var viewSettings = this.CreateViewSettingsData (list, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsMod�les(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var viewSettings = this.CreateViewSettingsData (list, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultViewSettingsP�riodes(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetViewSettingsList (this.GetKey (nomPr�sentation, "ViewSettings"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var viewSettings = this.CreateViewSettingsData (list, DefaultViewSettings.defaultName, searchExist, filterExist, optionsExist);
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
				var viewSettings = this.CreateViewSettingsData<R�sum�P�riodiqueOptions> (list, "Charges et produits", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptCat�gorie (viewSettings.Filter, Cat�gorieDeCompte.Charge | Cat�gorieDeCompte.Produit);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptProfondeur (viewSettings.Filter, 4, int.MaxValue);
			}

			{
				var viewSettings = this.CreateViewSettingsData<R�sum�P�riodiqueOptions> (list, "Tout", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
			}

			{
				var viewSettings = this.CreateViewSettingsData<R�sum�P�riodiqueOptions> (list, "Histogramme des soldes", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptCat�gorie (viewSettings.Filter, Cat�gorieDeCompte.Charge | Cat�gorieDeCompte.Produit);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptProfondeur (viewSettings.Filter, 4, int.MaxValue);

				this.OptionsAdaptGraph (viewSettings.Options);
				viewSettings.Options.GraphOptions.Mode = GraphMode.Stacked;
			}

			{
				var viewSettings = this.CreateViewSettingsData<R�sum�P�riodiqueOptions> (list, "Secteurs des soldes", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (viewSettings.Filter);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptCat�gorie (viewSettings.Filter, Cat�gorieDeCompte.Charge | Cat�gorieDeCompte.Produit);
				this.SearchAdd (viewSettings.Filter);
				this.SearchAdaptProfondeur (viewSettings.Filter, 4, int.MaxValue);
				this.OptionsAdaptR�sum�P�riodique (viewSettings.Options, 1);

				this.OptionsAdaptGraph (viewSettings.Options);
				viewSettings.Options.GraphOptions.Mode = GraphMode.Pie;
				viewSettings.Options.GraphOptions.HasThreshold = true;
				viewSettings.Options.GraphOptions.PrimaryDimension = 0;
				viewSettings.Options.GraphOptions.SecondaryDimension = 1;
			}

			this.Select<R�sum�P�riodiqueOptions> (list, nomPr�sentation);
		}


		private ViewSettingsData CreateViewSettingsData<T>(ViewSettingsList list, FormattedText name, bool searchExist, bool filterExist, bool optionsExist)
			where T : AbstractOptions, new ()
		{
			var viewSettings = this.CreateViewSettingsData (list, name, searchExist, filterExist, optionsExist);

			viewSettings.Options = new T ();
			viewSettings.Options.SetComptaEntity (this.compta);
			viewSettings.Options.Clear ();

			return viewSettings;
		}

		private ViewSettingsData CreateViewSettingsData(ViewSettingsList list, FormattedText name, bool searchExist, bool filterExist, bool optionsExist)
		{
			var viewSettings = new ViewSettingsData ()
			{
				Name        = name,
				Readonly    = true,
				Permanent   = true,
				Search      = new SearchData (),
				Filter      = new SearchData (),
				ShowSearch  = searchExist  ? ShowPanelMode.Nop : ShowPanelMode.DoesNotExist,
				ShowFilter  = filterExist  ? ShowPanelMode.Nop : ShowPanelMode.DoesNotExist,
				ShowOptions = optionsExist ? ShowPanelMode.Nop : ShowPanelMode.DoesNotExist,
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

			if (list.Selected.Options != null)
			{
				list.Selected.Options.CopyTo (this.mainWindowController.GetSettingsOptions<T> (this.GetKey (nomPr�sentation, "Options"), this.compta));
			}
		}

		private void Select(ViewSettingsList list, string nomPr�sentation)
		{
			list.SelectedIndex = 0;  // s�lectionne "R�glages standards"

			if (list.Selected.Search != null)
			{
				list.Selected.Search.CopyTo (this.mainWindowController.GetSettingsSearchData (this.GetKey (nomPr�sentation, "Search")));
			}

			if (list.Selected.Filter != null)
			{
				list.Selected.Filter.CopyTo (this.mainWindowController.GetSettingsSearchData (this.GetKey (nomPr�sentation, "Filter")));
			}
		}

		private string GetKey(string nomPr�sentation, string typeName)
		{
			//	Retourne par exemple "Pr�sentation.Journal.Search".
			return "Pr�sentation." + nomPr�sentation + "." + typeName;
		}


		private readonly static FormattedText				defaultName = "R�glages standards";

		private readonly MainWindowController				mainWindowController;
		private readonly ComptaEntity						compta;
	}
}
