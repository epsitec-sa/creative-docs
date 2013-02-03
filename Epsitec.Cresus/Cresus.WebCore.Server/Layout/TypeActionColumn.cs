using Epsitec.Cresus.WebCore.Server.Core;

using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal sealed class TypeActionColumn : TileColumn
	{


		public string EntityTypeId
		{
			get;
			set;
		}


		public override string GetColumnType()
		{
			return "typeaction";
		}


		public override Dictionary<string, object> ToDictionary(Caches caches)
		{
			var column = base.ToDictionary (caches);

			column["entityTypeId"] = this.EntityTypeId;

			column.Remove ("entityId");

			return column;
		}


	}


}