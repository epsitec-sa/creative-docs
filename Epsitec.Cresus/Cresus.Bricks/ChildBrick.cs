namespace Epsitec.Cresus.Bricks
{
	public abstract class ChildBrick<T, TParent> : Brick<T>
		where TParent : Brick
	{
		public ChildBrick(TParent parent, BrickPropertyKey brickPropertyKey)
		{
			parent.AddProperty (new BrickProperty (brickPropertyKey, this));

			this.DefineBrickWall (parent.BrickWall);
			this.parent = parent;
		}

		public TParent End()
		{
			return this.parent;
		}

		readonly TParent parent;
	}
}
