using Epsitec.Cresus.WebCore.Server.Core;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	/// <summary>
	/// This class is used to represent columns that display action tiles, but for actions that are
	/// not related to an entity but to an entity type.
	/// </summary>
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
