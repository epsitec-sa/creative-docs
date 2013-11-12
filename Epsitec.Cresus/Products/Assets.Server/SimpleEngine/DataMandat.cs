//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class DataMandat
	{
		public DataMandat(System.DateTime startDate, System.DateTime endDate)
		{
			this.StartDate = startDate;
			this.EndDate   = endDate;

			this.Guid = Guid.NewGuid ();

			this.objects    = new GuidList<DataObject> ();
			this.categories = new GuidList<DataObject> ();
			this.groups     = new GuidList<DataObject> ();
		}

		public readonly System.DateTime			StartDate;
		public readonly System.DateTime			EndDate;
		public readonly Guid					Guid;


		public GuidList<DataObject> GetData(BaseType type)
		{
			switch (type)
			{
				case BaseType.Objects:
					return this.objects;

				case BaseType.Categories:
					return this.categories;

				case BaseType.Groups:
					return this.groups;

				default:
					return null;
			}
		}


		private readonly GuidList<DataObject>	objects;
		private readonly GuidList<DataObject>	categories;
		private readonly GuidList<DataObject>	groups;
	}
}
