namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ArrowButton dessine un bouton flèche.
	/// </summary>
	public class ArrowButton : Button
	{
		public ArrowButton()
		{
			this.direction = Direction.None;
		}
		
		public Direction				Direction
		{
			get { return this.direction; }
			set { this.direction = value; }
		}
		
		public override double			DefaultWidth
		{
			get { return 17; }
		}
		
		public override double			DefaultHeight
		{
			get { return 17; }
		}
		
		
		protected Direction				direction;
	}
}
