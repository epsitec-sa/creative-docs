//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.WebCore.Server.Core;

using System.Collections.Generic;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	using Database = Core.Databases.Database;

	/// <summary>
	/// The set column is a column that displays the content of a SetViewController.
	/// </summary>
	internal sealed class SetColumn : EntityColumn
	{
		public string							Icon
		{
			get;
			set;
		}

		public string							Title
		{
			get;
			set;
		}

		public Database							DisplayDatabase
		{
			get;
			set;
		}

		public Database							PickDatabase
		{
			get;
			set;
		}


		public override string GetColumnType()
		{
			return "set";
		}


		protected override void FillDictionary(Dictionary<string, object> column, Caches caches)
		{
			base.FillDictionary (column, caches);

			column["icon"]            = this.Icon;
			column["title"]           = this.Title;
			column["displayDatabase"] = this.DisplayDatabase.GetDataDictionary (caches);
			column["pickDatabase"]    = this.PickDatabase.GetDataDictionary (caches);
		}
	}
}
