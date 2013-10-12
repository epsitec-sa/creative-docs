//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.WebCore.Server.Core;

using System.Collections.Generic;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// This class represents an entity column, which is what the javascript client will recieve
	/// when asking for a layout. It is the base class of all columns kinds.
	/// </summary>
	internal abstract class EntityColumn
	{
		public string							EntityId
		{
			get;
			set;
		}

		public string							ViewMode
		{
			get;
			set;
		}

		public string							ViewId
		{
			get;
			set;
		}


		public abstract string GetColumnType();

		public static Dictionary<string, object> GetEmptyDictionary()
		{
			return new Dictionary<string, object> ();
		}

		public Dictionary<string, object> ToDictionary(Caches caches)
		{
			var column = EntityColumn.GetEmptyDictionary ();

			this.FillDictionary (column, caches);

			return column;
		}

		protected virtual void FillDictionary(Dictionary<string, object> column, Caches caches)
		{
			column["type"]     = this.GetColumnType ();
			column["entityId"] = this.EntityId;
			column["viewMode"] = this.ViewMode;
			column["viewId"]   = this.ViewId;
		}
	}
}
