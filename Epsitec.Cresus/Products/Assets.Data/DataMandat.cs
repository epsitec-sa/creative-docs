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
					// Il vaut mieux retourner une liste vide, plut�t que null.
					return new GuidList<DataObject> ();
			}
		}


		#region Accounts
		public IEnumerable<DateRange>			AccountsDateRanges
		{
			//	Retourne la liste des p�riodes de tous les plans comptables connus.
			get
			{
				return this.rangeAccounts.Select (x => x.Key);
			}
		}

		public BaseType GetAccountsBase(System.DateTime date)
		{
			//	Retourne la base correspondant � une date.
			//	Si plusieurs p�riodes se recouvrent, on prend la derni�re d�finie.
			var range = this.GetBestDateRange (date);
			return new BaseType (BaseTypeKind.Accounts, range);
		}

		public GuidList<DataObject> GetAccounts(DateRange range)
		{
			//	Retourne le plan comptable correspondant � une p�riode.
			GuidList<DataObject> accounts;
			if (!range.IsEmpty && this.rangeAccounts.TryGetValue (range, out accounts))
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

		public DateRange GetBestDateRange(System.DateTime date)
		{
			//	Retourne la p�riode comptable correspondant � une date donn�e.
			//	Si plusieurs p�riodes se recouvrent, on prend la derni�re d�finie.
			var range = this.AccountsDateRanges
				.Reverse ()
				.Where (x => x.IsInside (date))
				.FirstOrDefault ();

			if (range.IncludeFrom == System.DateTime.MinValue)
			{
				//	Attention, si FirstOrDefault ne trouve rien, il ne rend pas un
				//	DateRange.Empty, mais un DateRange avec les deux dates � "z�ro",
				//	c'est-�-dire �gales � System.DateTime.MinValue !
				return DateRange.Empty;
			}
			else
			{
				return range;
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
