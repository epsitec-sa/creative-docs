namespace Epsitec.Cresus.Bricks
{
	public abstract class Brick<T> : Brick
	{
		public override System.Type GetBrickType()
		{
			return typeof (T);
		}
	}
}
