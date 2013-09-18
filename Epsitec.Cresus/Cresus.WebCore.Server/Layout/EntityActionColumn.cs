//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.WebCore.Server.Core;

using System.Collections.Generic;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// This class represents the entity column that is used to display action tiles that are
	/// related to an entity (and optionally also an additional entity).
	/// </summary>
	internal sealed class EntityActionColumn : TileColumn
	{
		public string							AdditionalEntityId
		{
			get;
			set;
		}

		public override string GetColumnType()
		{
			return "entityaction";
		}

		
		protected override void FillDictionary(Dictionary<string, object> column, Caches caches)
		{
			base.FillDictionary (column, caches);

			column["additionalEntityId"] = this.AdditionalEntityId;
		}
	}
}
