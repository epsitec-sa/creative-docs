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

			this.globalSettings = new GlobalSettings ();

			this.assets     = new GuidList<DataObject> ();
			this.categories = new GuidList<DataObject> ();
			this.groups     = new GuidList<DataObject> ();
			this.persons    = new GuidList<DataObject> ();
			this.accounts   = new GuidList<DataObject> ();
			this.entries    = new GuidList<DataObject> ();
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
					return this.accounts;

				case BaseType.Entries:
					return this.entries;

				default:
					return null;
			}
		}


		private readonly GlobalSettings			globalSettings;
		private readonly GuidList<DataObject>	assets;
		private readonly GuidList<DataObject>	categories;
		private readonly GuidList<DataObject>	groups;
		private readonly GuidList<DataObject>	persons;
		private readonly GuidList<DataObject>	accounts;
		private readonly GuidList<DataObject>	entries;
	}
}
