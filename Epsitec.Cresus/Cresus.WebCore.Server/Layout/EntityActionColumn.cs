using Epsitec.Cresus.WebCore.Server.Core;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	/// <summary>
	/// This class represents the entity column that is used to display action tiles that are
	/// related to an entity (and optionnaly also an additional entity).
	/// </summary>
	internal sealed class EntityActionColumn : TileColumn
	{


		public string AdditionalEntityId
		{
			get;
			set;
		}


		public override string GetColumnType()
		{
			return "entityaction";
		}


		public override Dictionary<string, object> ToDictionary(Caches caches)
		{
			var column = base.ToDictionary (caches);

			column["additionalEntityId"] = this.AdditionalEntityId;

			return column;
		}


	}


}
