using Epsitec.Common.Support.EntityEngine;

using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public abstract class Brick<T> : Brick
		where T : AbstractEntity, new ()
	{
		public Brick(BrickWall brickWall, Expression resolver)
			: base (brickWall, resolver)
		{
		}

		public override System.Type GetBrickType()
		{
			return typeof (T);
		}

		protected TChild AddChild<TChild>(TChild child, BrickPropertyKey brickPropertyKey)
			where TChild : Brick
		{
			this.AddProperty (new BrickProperty (brickPropertyKey, child));
			return child;
		}
	}
}
