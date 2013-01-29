using Epsitec.Cresus.WebCore.Server.Core;

using System.Collections.Generic;

using System.Linq;



namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal sealed class ActionColumn : TileColumn
	{


		public string AdditionalEntityId
		{
			get;
			set;
		}


		public override string GetColumnType()
		{
			return "action";
		}


		public override Dictionary<string, object> ToDictionary(Caches caches)
		{
			var column = base.ToDictionary (caches);

			column["additionalEntityId"] = this.AdditionalEntityId;

			return column;
		}


	}


}
