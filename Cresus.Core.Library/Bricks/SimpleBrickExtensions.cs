//	Copyright © 2013-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;

using System.Linq;

namespace Epsitec.Cresus.Core.Bricks
{
	public static class SimpleBrickExtensions
	{
		public static SimpleBrick<T> WithSpecialController<T>(this SimpleBrick<T> brick, System.Type controllerType, string arg = default)
			where T : AbstractEntity, new ()
		{
			var entityType = typeof (T);

			if (!EntityViewController.AreCompatible (entityType, controllerType))
			{
				var message = 
                    "The controller type " + controllerType.FullName + " is not " +
                    "compatible with the entity type " + entityType.FullName + ".";

				throw new System.ArgumentException (message);
			}

			var ids = controllerType
                .GetCustomAttributes<BrickControllerSubTypeAttribute> (false)
                .Select (x => x.Id);

			if (ids.Any ())
			{
				var index = ids.Single ();
				var mode  = BrickMode.SpecialController0 + index;

                if (string.IsNullOrEmpty (arg))
                {
                    brick.Attribute (mode);
                }
                else
                {
                    brick.Attribute (mode, arg);
                }

				return brick;
			}

			throw new System.ArgumentException ("The type " + controllerType.FullName + " does not support ControllerSubTypeAttribute.");
		}
	}
}
