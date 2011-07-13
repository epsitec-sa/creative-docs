//	Copyright © 2008-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public sealed class CoreSession : CoreApp
	{
		public static BrickWall GetBrickWall(AbstractEntity entity, ViewControllerMode mode)
		{
			var controller = EntityViewControllerFactory.Create ("js", entity, mode, null, resolutionMode: Resolvers.ResolutionMode.InspectOnly);
			var brickWall  = controller.CreateBrickWallForInspection ();

			return brickWall;
		}

		public override string ApplicationIdentifier
		{
			get
			{
				return "CoreSession";
			}
		}

		public override string ShortWindowTitle
		{
			get
			{
				throw new System.NotImplementedException ();
			}
		}
	}
}
