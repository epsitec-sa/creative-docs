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


		public string EntityId
		{
			get;
			set;
		}


		public string ViewMode
		{
			get;
			set;
		}


		public string ViewId
		{
			get;
			set;
		}


		public abstract string GetColumnType();


		public virtual Dictionary<string, object> ToDictionary(Caches caches)
		{
			var column = new Dictionary<string, object> ();

			column["type"] = this.GetColumnType ();
			column["entityId"] = this.EntityId;
			column["viewMode"] = this.ViewMode;
			column["viewId"] = this.ViewId;

			return column;
		}


	}


}
