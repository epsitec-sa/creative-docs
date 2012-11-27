using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using System;

namespace Epsitec.Cresus.Core.Controllers.ActionControllers
{
	public abstract class ActionViewController<T> : EntityViewController<T>, IActionViewController
		where T : AbstractEntity, new ()
	{
		protected sealed override void CreateBricks(BrickWall<T> wall)
		{
			var action = wall
				.AddBrick ()
				.DefineAction ();

			this.GetForm (action);
		}

		protected abstract void GetForm(ActionBrick<T, SimpleBrick<T>> action);

		#region IActionViewController Members

		public abstract FormattedText GetTitle();

		public abstract ActionExecutor GetExecutor();

		#endregion
	}
}