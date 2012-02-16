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
using Epsitec.Cresus.Compta.Memory.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.IO
{
	/// <summary>
	/// Cette classe s'occupe de créer les styles par défaut.
	/// </summary>
	public class DefaultMemory
	{
		public DefaultMemory(MainWindowController mainWindowController)
		{
			this.mainWindowController = mainWindowController;
			this.compta               = mainWindowController.Compta;
		}


		public void CreateDefaultMemory()
		{
			this.CreateDefaultMemoryJournal         ("Journal");
			this.CreateDefaultMemoryPlanComptable   ("PlanComptable");
			this.CreateDefaultMemoryBalance         ("Balance");
			this.CreateDefaultMemoryExtraitDeCompte ("ExtraitDeCompte");
			this.CreateDefaultMemoryBilan           ("Bilan");
			this.CreateDefaultMemoryPP              ("PP");
			this.CreateDefaultMemoryExploitation    ("Exploitation");
			this.CreateDefaultMemoryBudgets         ("Budgets");
			this.CreateDefaultMemoryJournaux        ("Journaux");
			this.CreateDefaultMemoryLibellés        ("Libellés");
			this.CreateDefaultMemoryModèles         ("Modèles");
			this.CreateDefaultMemoryPériodes        ("Périodes");
		}


		private void CreateDefaultMemoryJournal(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var memory = this.CreateMemoryData<JournalOptions> (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			{
				var memory = this.CreateMemoryData<JournalOptions> (list, "Premier trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (memory.Filter, 1, 1, 31, 3);  // 1 janvier -> 31 mars
			}

			{
				var memory = this.CreateMemoryData<JournalOptions> (list, "Deuxième trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (memory.Filter, 1, 4, 30, 6);  // 1 avril -> 30 juin
			}

			{
				var memory = this.CreateMemoryData<JournalOptions> (list, "Troisième trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (memory.Filter, 1, 7, 30, 9);  // 1 juillet  -> 30 septembre
			}

			{
				var memory = this.CreateMemoryData<JournalOptions> (list, "Quatrième trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (memory.Filter, 1, 10, 31, 12);  // 1 octobre -> 31 décembre
			}

			{
				var memory = this.CreateMemoryData<JournalOptions> (list, "Rechercher...", searchExist, filterExist, optionsExist);
				memory.ShowSearch = ShowPanelMode.ShowBeginner;
			}

			this.Select<JournalOptions> (list, nomPrésentation);
		}

		private void CreateDefaultMemoryPlanComptable(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			{
				var memory = this.CreateMemoryData (list, "Actifs", searchExist, filterExist, optionsExist);
				this.SearchAdaptCatégorie (memory.Filter, CatégorieDeCompte.Actif);
			}

			{
				var memory = this.CreateMemoryData (list, "Passifs", searchExist, filterExist, optionsExist);
				this.SearchAdaptCatégorie (memory.Filter, CatégorieDeCompte.Passif);
			}

			{
				var memory = this.CreateMemoryData (list, "Charges", searchExist, filterExist, optionsExist);
				this.SearchAdaptCatégorie (memory.Filter, CatégorieDeCompte.Charge);
			}

			{
				var memory = this.CreateMemoryData (list, "Produits", searchExist, filterExist, optionsExist);
				this.SearchAdaptCatégorie (memory.Filter, CatégorieDeCompte.Produit);
			}

			{
				var memory = this.CreateMemoryData (list, "Exploitations", searchExist, filterExist, optionsExist);
				this.SearchAdaptCatégorie (memory.Filter, CatégorieDeCompte.Exploitation);
			}

			this.Select (list, nomPrésentation);
		}

		private void CreateDefaultMemoryBalance(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var memory = this.CreateMemoryData<BalanceOptions> (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
			}

			{
				var memory = this.CreateMemoryData<BalanceOptions> (list, "Vue d'ensemble", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAdd (memory.Filter);
				this.SearchAdaptProfondeur (memory.Filter, 1, 2);
			}

			this.Select<BalanceOptions> (list, nomPrésentation);
		}

		private void CreateDefaultMemoryExtraitDeCompte(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var memory = this.CreateMemoryData<ExtraitDeCompteOptions> (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select<ExtraitDeCompteOptions> (list, nomPrésentation);
		}

		private void CreateDefaultMemoryBilan(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var memory = this.CreateMemoryData<BilanOptions> (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var memory = this.CreateMemoryData<BilanOptions> (list, "Premier trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAdd (memory.Filter);
				this.SearchAdaptDate (memory.Filter, 1, 1, 31, 3);  // 1 janvier -> 31 mars
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var memory = this.CreateMemoryData<BilanOptions> (list, "Deuxième trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAdd (memory.Filter);
				this.SearchAdaptDate (memory.Filter, 1, 4, 30, 6);  // 1 avril -> 30 juin
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var memory = this.CreateMemoryData<BilanOptions> (list, "Troisième trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAdd (memory.Filter);
				this.SearchAdaptDate (memory.Filter, 1, 7, 30, 9);  // 1 juillet  -> 30 septembre
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var memory = this.CreateMemoryData<BilanOptions> (list, "Quatrième trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAdd (memory.Filter);
				this.SearchAdaptDate (memory.Filter, 1, 10, 31, 12);  // 1 octobre -> 31 décembre
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var memory = this.CreateMemoryData<BilanOptions> (list, "Vue d'ensemble graphique", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAdd (memory.Filter);
				this.SearchAdaptProfondeur (memory.Filter, 1, 2);
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.Budget, ComparisonDisplayMode.Graphique);
			}

			{
				var memory = this.CreateMemoryData<BilanOptions> (list, "Précédent graphique", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.Budget, ComparisonDisplayMode.Graphique);
			}

			this.Select<BilanOptions> (list, nomPrésentation);
		}

		private void CreateDefaultMemoryPP(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var memory = this.CreateMemoryData<PPOptions> (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var memory = this.CreateMemoryData<PPOptions> (list, "Premier trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAdd (memory.Filter);
				this.SearchAdaptDate (memory.Filter, 1, 1, 31, 3);  // 1 janvier -> 31 mars
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var memory = this.CreateMemoryData<PPOptions> (list, "Deuxième trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAdd (memory.Filter);
				this.SearchAdaptDate (memory.Filter, 1, 4, 30, 6);  // 1 avril -> 30 juin
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var memory = this.CreateMemoryData<PPOptions> (list, "Troisième trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAdd (memory.Filter);
				this.SearchAdaptDate (memory.Filter, 1, 7, 30, 9);  // 1 juillet  -> 30 septembre
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var memory = this.CreateMemoryData<PPOptions> (list, "Quatrième trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAdd (memory.Filter);
				this.SearchAdaptDate (memory.Filter, 1, 10, 31, 12);  // 1 octobre -> 31 décembre
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var memory = this.CreateMemoryData<PPOptions> (list, "Vue d'ensemble graphique", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAdd (memory.Filter);
				this.SearchAdaptProfondeur (memory.Filter, 1, 2);
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.Budget, ComparisonDisplayMode.Graphique);
			}

			{
				var memory = this.CreateMemoryData<PPOptions> (list, "Budget graphique", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.Budget, ComparisonDisplayMode.Graphique);
			}

			this.Select<PPOptions> (list, nomPrésentation);
		}

		private void CreateDefaultMemoryExploitation(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var memory = this.CreateMemoryData<ExploitationOptions> (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
			}

			this.Select<ExploitationOptions> (list, nomPrésentation);
		}

		private void CreateDefaultMemoryBudgets(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPrésentation);
		}

		private void CreateDefaultMemoryJournaux(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPrésentation);
		}

		private void CreateDefaultMemoryLibellés(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPrésentation);
		}

		private void CreateDefaultMemoryModèles(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPrésentation);
		}

		private void CreateDefaultMemoryPériodes(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPrésentation);
		}


		private MemoryData CreateMemoryData<T>(MemoryList list, FormattedText name, bool searchExist, bool filterExist, bool optionsExist)
			where T : AbstractOptions, new ()
		{
			var memory = this.CreateMemoryData (list, name, searchExist, filterExist, optionsExist);

			memory.Options = new T ();
			memory.Options.SetComptaEntity (this.compta);
			memory.Options.Clear ();

			return memory;
		}

		private MemoryData CreateMemoryData(MemoryList list, FormattedText name, bool searchExist, bool filterExist, bool optionsExist)
		{
			var memory = new MemoryData ()
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

			list.List.Add (memory);

			return memory;
		}


		private void SearchAdd(SearchData data)
		{
			data.TabsData.Add (new SearchTabData ());
		}

		private void SearchAdaptForNonZero(SearchData data)
		{
			var tab = data.TabsData.Last ();

			tab.SearchText.FromText = Converters.MontantToString (0);
			tab.SearchText.Mode     = SearchMode.WholeContent;
			tab.SearchText.Invert   = true;
			tab.Column              = ColumnType.Solde;

			this.SearchAdaptOrMode (data);
		}

		private void SearchAdaptProfondeur(SearchData data, int minProfondeur, int maxProfondeur)
		{
			var tab = data.TabsData.Last ();

			tab.SearchText.FromText = Converters.IntToString (minProfondeur);
			tab.SearchText.ToText   = Converters.IntToString (maxProfondeur);
			tab.SearchText.Mode     = SearchMode.Interval;
			tab.Column              = ColumnType.Profondeur;

			this.SearchAdaptOrMode (data);
		}

		private void SearchAdaptDate(SearchData data, int startDay, int startMonth, int endDay, int endMonth)
		{
			var tab = data.TabsData.Last ();

			int year = Date.Today.Year;

			tab.SearchText.FromText = Converters.DateToString (new Date (year, startMonth, startDay));
			tab.SearchText.ToText   = Converters.DateToString (new Date (year,   endMonth,   endDay));
			tab.SearchText.Mode     = SearchMode.Interval;
			tab.Column              = ColumnType.Date;

			this.SearchAdaptOrMode (data);
		}

		private void SearchAdaptCatégorie(SearchData data, CatégorieDeCompte catégorie)
		{
			var tab = data.TabsData.Last ();

			tab.SearchText.FromText = Converters.CatégoriesToString (catégorie);
			tab.SearchText.Mode     = SearchMode.Fragment;
			tab.Column              = ColumnType.Catégorie;

			this.SearchAdaptOrMode (data);
		}

		private void SearchAdaptOrMode(SearchData data)
		{
			data.OrMode = (data.TabsData.Count == 1);
		}


		private void OptionsAdaptDouble(AbstractOptions options, ComparisonShowed showed, ComparisonDisplayMode mode)
		{
			var o = options as DoubleOptions;

			o.HideZero              = true;
			o.HasGraphics           = (showed != ComparisonShowed.None);
			o.ComparisonEnable      = (showed != ComparisonShowed.None);
			o.ComparisonShowed      = showed;
			o.ComparisonDisplayMode = mode;
		}


		private void Select<T>(MemoryList list, string nomPrésentation)
			where T : AbstractOptions, new ()
		{
			this.Select (list, nomPrésentation);

			if (list.Selected.Options != null)
			{
				list.Selected.Options.CopyTo (this.mainWindowController.GetSettingsOptions<T> (this.GetKey (nomPrésentation, "Options"), this.compta));
			}
		}

		private void Select(MemoryList list, string nomPrésentation)
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


		private readonly static FormattedText				defaultName = "Réglages standards";

		private readonly MainWindowController				mainWindowController;
		private readonly ComptaEntity						compta;
	}
}
