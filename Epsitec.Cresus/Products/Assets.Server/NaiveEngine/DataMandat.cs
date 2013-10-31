//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.NaiveEngine
{
	public class DataMandat
	{
		public DataMandat(System.DateTime startDate)
		{
			this.StartDate = startDate;
			this.Guid = Guid.NewGuid ();

			this.objects    = new GuidList<DataObject> ();
			this.categories = new GuidList<DataObject> ();
			this.groups     = new GuidList<DataObject> ();
		}

		public readonly System.DateTime			StartDate;
		public readonly Guid					Guid;


		public GuidList<DataObject> Objects
		{
			get
			{
				return this.objects;
			}
		}

		public GuidList<DataObject> Categories
		{
			get
			{
				return this.categories;
			}
		}

		public GuidList<DataObject> Groups
		{
			get
			{
				return this.groups;
			}
		}


		private readonly GuidList<DataObject> objects;
		private readonly GuidList<DataObject> categories;
		private readonly GuidList<DataObject> groups;
	}
}
