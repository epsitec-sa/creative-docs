using Epsitec.Cresus.WebCore.Server.Core;

using System.Collections.Generic;

using System.Linq;



namespace Epsitec.Cresus.WebCore.Server.Layout
{


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
