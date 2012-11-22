using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Cresus.Bricks
{
	public abstract class ChildBrick<T, TParent> : Brick<T>
		where T : AbstractEntity, new ()
		where TParent : Brick
	{
		public ChildBrick(TParent parent, bool includeResolver = false)
			: base (parent.BrickWall, includeResolver ? parent.Resolver : null)
		{
			this.parent = parent;
		}

		public TParent End()
		{
			return this.parent;
		}

		readonly TParent parent;
	}
}
