//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

				case BaseType.Accounts:
					return this.CurrentAccounts;

				case BaseType.Entries:
					return this.entries;

				default:
					return null;
			}
		}


		#region Accounts
		public DateRange						CurrentAccountsDateRange;

		public IEnumerable<DateRange>			AccountsDateRanges
		{
			//	Retourne la liste des p�riodes de tous les plans comptables connus.
			get
			{
				return this.rangeAccounts.Select (x => x.Key);
			}
		}

		public GuidList<DataObject> GetAccounts(DateRange range)
		{
			//	Retourne le plan comptable correspondant � une p�riode.
			GuidList<DataObject> accounts;
			if (this.rangeAccounts.TryGetValue (range, out accounts))
			{
				return accounts;
			}
			else
			{
				// Il vaut mieux retourner une liste vide, plut�t que null.
				return new GuidList<DataObject> ();
			}
		}

		public void AddAccounts(DateRange dateRange, GuidList<DataObject> accounts)
		{
			//	Prend connaissance d'un nouveau plan comptable, qui est ajout� ou
			//	qui remplace un existant, selon sa p�riode.
			this.rangeAccounts[dateRange] = accounts;
		}

		private GuidList<DataObject> CurrentAccounts
		{
			//	Retourne le plan comptable correspondant � la p�riode courante.
			get
			{
				return this.GetAccounts (this.CurrentAccountsDateRange);
			}
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
