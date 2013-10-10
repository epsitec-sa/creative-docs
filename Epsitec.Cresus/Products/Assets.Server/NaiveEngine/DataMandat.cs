//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.NaiveEngine
{
	public class DataMandat
	{
		public DataMandat()
		{
			this.Guid = Guid.NewGuid ();

			this.objects    = new List<DataObject> ();
			this.categories = new List<DataObject> ();
			this.groups     = new List<DataObject> ();
		}

		public readonly Guid					Guid;


		public List<DataObject>					Objects
		{
			get
			{
				return this.objects;
			}
		}

		public List<DataObject>					Categories
		{
			get
			{
				return this.categories;
			}
		}

		public List<DataObject>					Groups
		{
			get
			{
				return this.groups;
			}
		}


		public DataObject GetObject(Guid guid)
		{
			return this.objects.Where (x => x.Guid == guid).FirstOrDefault ();
		}

		public DataObject GetCategory(Guid guid)
		{
			return this.categories.Where (x => x.Guid == guid).FirstOrDefault ();
		}

		public DataObject GetGroup(Guid guid)
		{
			return this.groups.Where (x => x.Guid == guid).FirstOrDefault ();
		}


		private readonly List<DataObject> objects;
		private readonly List<DataObject> categories;
		private readonly List<DataObject> groups;
	}
}
