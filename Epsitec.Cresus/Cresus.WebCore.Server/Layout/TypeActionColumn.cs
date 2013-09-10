//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

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
		public string							EntityTypeId
		{
			get;
			set;
		}


		public override string GetColumnType()
		{
			return "typeaction";
		}

		protected override void FillDictionary(Dictionary<string, object> column, Caches caches)
		{
			base.FillDictionary (column, caches);

			column["entityTypeId"] = this.EntityTypeId;

			column.Remove ("entityId");
		}
	}
}
