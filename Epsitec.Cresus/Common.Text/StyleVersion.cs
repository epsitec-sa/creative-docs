//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe StyleVersion abrite un compteur qui permet d'associer des
	/// numéros de version aux styles et aux propriétés afin de détecter les
	/// modifications.
	/// </summary>
	public sealed class StyleVersion
	{
		private StyleVersion()
		{
		}
		
		
		public static StyleVersion				Default
		{
			get
			{
				return StyleVersion.default_instance;
			}
		}
		
		
		public long								Current
		{
			get
			{
				return this.current;
			}
		}
		
		public System.DateTime					LastModificationTime
		{
			get
			{
				return this.time;
			}
		}
		
		
		public long Change()
		{
			//	Il faut appeler cette méthode chaque fois que la valeur d'une
			//	propriété est modifiée (c'est fait par Property.Invalidate).
			
			System.Threading.Interlocked.Increment (ref this.current);
			this.time = System.DateTime.Now;
			return this.current;
		}
		
		private static StyleVersion				default_instance = new StyleVersion ();
		
		private long							current	= 1;
		private System.DateTime					time	= System.DateTime.Now;
	}
}
