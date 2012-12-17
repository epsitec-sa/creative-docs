using Epsitec.Cresus.WebCore.Server.Core;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal sealed class SetColumn : EntityColumn
	{


		public string Icon
		{
			get;
			set;
		}


		public string Title
		{
			get;
			set;
		}


		public Core.Databases.Database DisplayDatabase
		{
			get;
			set;
		}


		public Core.Databases.Database PickDatabase
		{
			get;
			set;
		}


		public override string GetColumnType()
		{
			return "set";		
		}


		public override Dictionary<string, object> ToDictionary(Caches caches)
		{
			var column = base.ToDictionary (caches);

			column["icon"] = this.Icon;
			column["title"] = this.Title;
			column["displayDatabase"] = this.DisplayDatabase.GetDataDictionary (caches);
			column["pickDatabase"] = this.PickDatabase.GetDataDictionary (caches);

			return column;
		}


	}


}

