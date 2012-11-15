//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks.Helpers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	/// <summary>
	/// The <c>SummaryDummyListEntityViewController</c> generic class is used to represent a
	/// list (or sublist) of items which is not stored in a parent entity, but which comes directly
	/// from the database.
	/// </summary>
	/// <typeparam name="T">The type of the items.</typeparam>
	public class SummaryDummyListEntityViewController<T> : SummaryViewController<DummyListEntity<T>>
			where T : AbstractEntity, new()
	{
		protected override void CreateBricks(BrickWall<DummyListEntity<T>> wall)
		{
			wall.AddBrick (x => x.Items)
				.Template ()
				  .Text (x => x.GetSummary ())
				  .TextCompact (x => x.GetCompactSummary ())
				.End ();
		}
	}
}
