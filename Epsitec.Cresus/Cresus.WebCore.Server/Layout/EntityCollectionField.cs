//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// This class represents an edition field used to edit entity collections values
	/// </summary>
	internal sealed class EntityCollectionField : AbstractField<EntityCollectionField>
	{
		public string							DatabaseName
		{
			get;
			set;
		}

		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["databaseName"] = this.DatabaseName;

			return brick;
		}
	}
}
