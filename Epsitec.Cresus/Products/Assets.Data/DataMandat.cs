//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public class DataMandat
	{
		public DataMandat(string name, System.DateTime startDate)
		{
			this.Name      = name;
			this.StartDate = startDate;

			this.Guid = Guid.NewGuid ();

			this.CurrentAccountsDateRange = DateRange.Empty;

			this.globalSettings = new GlobalSettings ();

			this.assets        = new GuidList<DataObject> ();
			this.categories    = new GuidList<DataObject> ();
			this.groups        = new GuidList<DataObject> ();
			this.persons       = new GuidList<DataObject> ();
			this.entries       = new GuidList<DataObject> ();
			this.rangeAccounts = new Dictionary<DateRange, GuidList<DataObject>> ();
		}

		public GlobalSettings					GlobalSettings
		{
			get
			{
				return this.globalSettings;
			}
		}


		public readonly string					Name;
		public readonly System.DateTime			StartDate;
		public readonly Guid					Guid;


		public GuidList<DataObject> GetData(BaseType type)
		{
			if (type >= BaseType.Accounts+0 &&
				type <= BaseType.Accounts+100)
			{
				return this.GetAccounts (type);
			}

			switch (type)
			{
				case BaseType.Assets:
					return this.assets;

				case BaseType.Categories:
					return this.categories;

				case BaseType.Groups:
					return this.groups;

				case BaseType.Persons:
					return this.persons;

				case BaseType.Entries:
					return this.entries;

				default:
					// Il vaut mieux retourner une liste vide, plutôt que null.
					return new GuidList<DataObject> ();
			}
		}


		#region Accounts
		public DateRange						CurrentAccountsDateRange;

		public IEnumerable<DateRange>			AccountsDateRanges
		{
			//	Retourne la liste des périodes de tous les plans comptables connus.
			get
			{
				return this.rangeAccounts.Select (x => x.Key);
			}
		}

		public DateRange GetAccountsDateRange(BaseType baseType)
		{
			int rank = baseType - BaseType.Accounts;  // 0..n
			var ranges = this.AccountsDateRanges.ToArray ();

			if (rank >= 0 && rank < ranges.Length)
			{
				return ranges[rank];
			}
			else
			{
				return DateRange.Empty;
			}
		}

		public BaseType GetAccountsBase(System.DateTime date)
		{
			//	Retourne la base correspondant à une date.
			//	Si plusieurs périodes se recouvrent, on prend la dernière définie.
			var ranges = this.AccountsDateRanges.ToArray ();

			for (int i=ranges.Length-1; i>=0; i--)
			{
				if (ranges[i].IsInside (date))
				{
					return BaseType.Accounts + i;
				}
			}

			return BaseType.Unknown;
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
				return new GuidList<DataObject> ();
			}
		}

		public void AddAccounts(DateRange dateRange, GuidList<DataObject> accounts)
		{
			//	Prend connaissance d'un nouveau plan comptable, qui est ajouté ou
			//	qui remplace un existant, selon sa période.
			this.rangeAccounts[dateRange] = accounts;
		}

		private GuidList<DataObject> GetAccounts(BaseType baseType)
		{
			//	Retourne le plan comptable.
			return this.GetAccounts (this.GetAccountsDateRange (baseType));
		}
		#endregion


		private readonly GlobalSettings									globalSettings;
		private readonly GuidList<DataObject>							assets;
		private readonly GuidList<DataObject>							categories;
		private readonly GuidList<DataObject>							groups;
		private readonly GuidList<DataObject>							persons;
		private readonly GuidList<DataObject>							entries;
		private readonly Dictionary<DateRange, GuidList<DataObject>>	rangeAccounts;
	}
}
