using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Bricks;

namespace Epsitec.Cresus.Core.Controllers.DeletionControllers
{
	public abstract class BrickDeletionViewController<T> : EntityViewController<T>, IActionExecutorProvider
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

		#region IActionExecutorProvider Members

		public abstract ActionExecutor GetExecutor();

		#endregion
	}
}