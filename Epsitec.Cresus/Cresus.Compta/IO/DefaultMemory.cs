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
using Epsitec.Cresus.Compta.Memory.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.IO
{
	/// <summary>
	/// Cette classe s'occupe de cr�er les styles par d�faut.
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
			this.CreateDefaultMemoryLibell�s        ("Libell�s");
			this.CreateDefaultMemoryMod�les         ("Mod�les");
			this.CreateDefaultMemoryP�riodes        ("P�riodes");
		}


		private void CreateDefaultMemoryJournal(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

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
				var memory = this.CreateMemoryData<JournalOptions> (list, "Deuxi�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (memory.Filter, 1, 4, 30, 6);  // 1 avril -> 30 juin
			}

			{
				var memory = this.CreateMemoryData<JournalOptions> (list, "Troisi�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (memory.Filter, 1, 7, 30, 9);  // 1 juillet  -> 30 septembre
			}

			{
				var memory = this.CreateMemoryData<JournalOptions> (list, "Quatri�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (memory.Filter, 1, 10, 31, 12);  // 1 octobre -> 31 d�cembre
			}

			{
				var memory = this.CreateMemoryData<JournalOptions> (list, "Rechercher...", searchExist, filterExist, optionsExist);
				memory.ShowSearch = ShowPanelMode.ShowBeginner;
			}

			this.Select<JournalOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryPlanComptable(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			{
				var memory = this.CreateMemoryData (list, "Actifs", searchExist, filterExist, optionsExist);
				this.SearchAdaptCat�gorie (memory.Filter, Cat�gorieDeCompte.Actif);
			}

			{
				var memory = this.CreateMemoryData (list, "Passifs", searchExist, filterExist, optionsExist);
				this.SearchAdaptCat�gorie (memory.Filter, Cat�gorieDeCompte.Passif);
			}

			{
				var memory = this.CreateMemoryData (list, "Charges", searchExist, filterExist, optionsExist);
				this.SearchAdaptCat�gorie (memory.Filter, Cat�gorieDeCompte.Charge);
			}

			{
				var memory = this.CreateMemoryData (list, "Produits", searchExist, filterExist, optionsExist);
				this.SearchAdaptCat�gorie (memory.Filter, Cat�gorieDeCompte.Produit);
			}

			{
				var memory = this.CreateMemoryData (list, "Exploitations", searchExist, filterExist, optionsExist);
				this.SearchAdaptCat�gorie (memory.Filter, Cat�gorieDeCompte.Exploitation);
			}

			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryBalance(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

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

			this.Select<BalanceOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryExtraitDeCompte(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var memory = this.CreateMemoryData<ExtraitDeCompteOptions> (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select<ExtraitDeCompteOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryBilan(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

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
				var memory = this.CreateMemoryData<BilanOptions> (list, "Deuxi�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAdd (memory.Filter);
				this.SearchAdaptDate (memory.Filter, 1, 4, 30, 6);  // 1 avril -> 30 juin
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var memory = this.CreateMemoryData<BilanOptions> (list, "Troisi�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAdd (memory.Filter);
				this.SearchAdaptDate (memory.Filter, 1, 7, 30, 9);  // 1 juillet  -> 30 septembre
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var memory = this.CreateMemoryData<BilanOptions> (list, "Quatri�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAdd (memory.Filter);
				this.SearchAdaptDate (memory.Filter, 1, 10, 31, 12);  // 1 octobre -> 31 d�cembre
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
				var memory = this.CreateMemoryData<BilanOptions> (list, "Pr�c�dent graphique", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.Budget, ComparisonDisplayMode.Graphique);
			}

			this.Select<BilanOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryPP(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

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
				var memory = this.CreateMemoryData<PPOptions> (list, "Deuxi�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAdd (memory.Filter);
				this.SearchAdaptDate (memory.Filter, 1, 4, 30, 6);  // 1 avril -> 30 juin
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var memory = this.CreateMemoryData<PPOptions> (list, "Troisi�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAdd (memory.Filter);
				this.SearchAdaptDate (memory.Filter, 1, 7, 30, 9);  // 1 juillet  -> 30 septembre
				this.OptionsAdaptDouble (memory.Options, ComparisonShowed.None, ComparisonDisplayMode.Montant);
			}

			{
				var memory = this.CreateMemoryData<PPOptions> (list, "Quatri�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAdd (memory.Filter);
				this.SearchAdaptDate (memory.Filter, 1, 10, 31, 12);  // 1 octobre -> 31 d�cembre
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

			this.Select<PPOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryExploitation(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var memory = this.CreateMemoryData<ExploitationOptions> (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
			}

			this.Select<ExploitationOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryBudgets(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryJournaux(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryLibell�s(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryMod�les(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryP�riodes(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPr�sentation);
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

		private void SearchAdaptCat�gorie(SearchData data, Cat�gorieDeCompte cat�gorie)
		{
			var tab = data.TabsData.Last ();

			tab.SearchText.FromText = Converters.Cat�goriesToString (cat�gorie);
			tab.SearchText.Mode     = SearchMode.Fragment;
			tab.Column              = ColumnType.Cat�gorie;

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


		private void Select<T>(MemoryList list, string nomPr�sentation)
			where T : AbstractOptions, new ()
		{
			this.Select (list, nomPr�sentation);

			if (list.Selected.Options != null)
			{
				list.Selected.Options.CopyTo (this.mainWindowController.GetSettingsOptions<T> (this.GetKey (nomPr�sentation, "Options"), this.compta));
			}
		}

		private void Select(MemoryList list, string nomPr�sentation)
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
