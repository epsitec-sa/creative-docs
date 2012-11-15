using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Cresus.Bricks
{
	public abstract class ChildBrick<T, TParent> : Brick<T>
		where T : AbstractEntity, new ()
		where TParent : Brick
	{
		public ChildBrick(BrickWall brickWall, TParent parent, BrickPropertyKey brickPropertyKey)
			: base (brickWall, null)
		{
			this.parent = parent;
			this.parent.AddProperty (new BrickProperty (brickPropertyKey, this));
		}

		public TParent End()
		{
			return this.parent;
		}

		readonly TParent parent;
	}
}
