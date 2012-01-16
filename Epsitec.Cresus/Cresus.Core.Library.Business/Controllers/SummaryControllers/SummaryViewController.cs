//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Bricks;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public abstract class SummaryViewController<T> : EntityViewController<T>
		where T : AbstractEntity, new ()
	{
		public void SerializeData()
		{
			//	TODO: ...
		}

		public void DeserializeData()
		{
			//	TODO: ...
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				//	...
			}

			base.Dispose (disposing);
		}

		protected override void CreateBricks(BrickWall<T> wall)
		{
			wall.AddBrick (x => x);
		}
	}
}
