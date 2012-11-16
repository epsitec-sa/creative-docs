using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Cresus.Bricks
{
	public abstract class ChildBrick<T, TParent> : Brick<T>
		where T : AbstractEntity, new ()
		where TParent : Brick
	{
		public ChildBrick(TParent parent)
			: base (parent.BrickWall, null)
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
