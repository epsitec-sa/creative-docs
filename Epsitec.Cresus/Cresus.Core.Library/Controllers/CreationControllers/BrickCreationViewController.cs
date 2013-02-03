using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Bricks;

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	// This class is some kind of a hack. It shouldn't really be an EntityViewController as the
	// entity we want to create with it does not exist yet. It is always associated with a
	// dummy entity, so don't bother calling this.Entity or using a lambda expression in the
	// bricks, as they will use this dummy entity.
	// The reason I chose to implement things this way instead of having this inheriting from
	// CoreViewController, is that this would require a lot more work. I would have to change
	// the way controllers are resolved, as now we can build only EntityViewControllers in
	// the EntityViewControllerResolver and in the EntityViewControllerFactory. I would also
	// have to change all the brick, because they all assume to have an entity associated. Also
	// the Carpenter and the UiBuilder assume that there is an entity. All this would be a lot
	// of work and I really don't have the time to do it right now.

	public abstract class BrickCreationViewController<T> : EntityViewController<T>, IFunctionExecutorProvider
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

		#region IFunctionExecutorProvider Members

		public abstract FunctionExecutor GetExecutor();

		#endregion
	}
}

