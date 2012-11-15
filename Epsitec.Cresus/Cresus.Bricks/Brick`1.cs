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
	}
}
