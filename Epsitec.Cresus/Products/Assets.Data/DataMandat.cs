//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Reports;

namespace Epsitec.Cresus.Assets.Data
{
	public class DataMandat
	{
		public DataMandat(string name, System.DateTime startDate)
		{
			this.Name      = name;
			this.StartDate = startDate;

			this.Guid = Guid.NewGuid ();

			this.undoManager = new UndoManager ();
			this.globalSettings = new GlobalSettings (this.undoManager);

			this.assets        = new GuidList<DataObject> (this.undoManager);
			this.categories    = new GuidList<DataObject> (this.undoManager);
			this.groups        = new GuidList<DataObject> (this.undoManager);
			this.persons       = new GuidList<DataObject> (this.undoManager);
			this.entries       = new GuidList<DataObject> (this.undoManager);
			this.rangeAccounts = new UndoableDictionary<DateRange, GuidList<DataObject>> (this.undoManager);
			this.reports       = new GuidList<AbstractReportParams> (this.undoManager);
		}

		public GlobalSettings					GlobalSettings
		{
			get
			{
				return this.globalSettings;
			}
		}

		public UndoManager						UndoManager
		{
			get
			{
				return this.undoManager;
			}
		}

		public GuidList<AbstractReportParams>	Reports
		{
			get
			{
				return this.reports;
			}
		}


		public readonly string					Name;
		public readonly System.DateTime			StartDate;
		public readonly Guid					Guid;


		public GuidList<DataObject> GetData(BaseType type)
		{
			switch (type.Kind)
			{
				case BaseTypeKind.Assets:
					return this.assets;

				case BaseTypeKind.Categories:
					return this.categories;

				case BaseTypeKind.Groups:
					return this.groups;

				case BaseTypeKind.Persons:
					return this.persons;

				case BaseTypeKind.Entries:
					return this.entries;

				case BaseTypeKind.Accounts:
					return this.GetAccounts (type.AccountsDateRange);

				default:
					// Il vaut mieux retourner une liste vide, plutôt que null.
					return new GuidList<DataObject> (this.undoManager);
			}
		}


		#region Accounts
		public IEnumerable<DateRange>			AccountsDateRanges
		{
			//	Retourne la liste des périodes de tous les plans comptables connus.
			get
			{
				return this.rangeAccounts.Select (x => x.Key);
			}
		}

		public BaseType GetAccountsBase(System.DateTime date)
		{
			//	Retourne la base correspondant à une date.
			//	Si plusieurs périodes se recouvrent, on prend la dernière définie.
			var range = this.GetBestDateRange (date);
			return new BaseType (BaseTypeKind.Accounts, range);
		}

		public GuidList<DataObject> GetAccounts(DateRange range)
		{
			//	Retourne le plan comptable correspondant à une période.
			GuidList<DataObject> accounts;
			if (!range.IsEmpty && this.rangeAccounts.TryGetValue (range, out accounts))
			{
				return accounts;
			}
			else
			{
				// Il vaut mieux retourner une liste vide, plutôt que null.
				return new GuidList<DataObject> (this.undoManager);
			}
		}

		public void AddAccounts(DateRange dateRange, GuidList<DataObject> accounts)
		{
			//	Prend connaissance d'un nouveau plan comptable, qui est ajouté ou
			//	qui remplace un existant, selon sa période.
			this.rangeAccounts[dateRange] = accounts;
		}

		public DateRange GetBestDateRange(System.DateTime date)
		{
			//	Retourne la période comptable correspondant à une date donnée.
			//	Si plusieurs périodes se recouvrent, on prend la dernière définie.
			return this.AccountsDateRanges
				.Reverse ()
				.Where (x => x.IsInside (date))
				.FirstOrDefault ();
		}
		#endregion


		private readonly GlobalSettings									globalSettings;
		private readonly UndoManager									undoManager;
		private readonly GuidList<DataObject>							assets;
		private readonly GuidList<DataObject>							categories;
		private readonly GuidList<DataObject>							groups;
		private readonly GuidList<DataObject>							persons;
		private readonly GuidList<DataObject>							entries;
		private readonly UndoableDictionary<DateRange, GuidList<DataObject>> rangeAccounts;
		private readonly GuidList<AbstractReportParams>					reports;
	}
}
