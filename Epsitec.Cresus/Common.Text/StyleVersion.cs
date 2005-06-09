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
		
		
		public static long						Current
		{
			get
			{
				return StyleVersion.current;
			}
		}
		
		public static System.DateTime			LastModificationTime
		{
			get
			{
				return StyleVersion.time;
			}
		}
		
		
		public static void Change()
		{
			System.Threading.Interlocked.Increment (ref StyleVersion.current);
			StyleVersion.time = System.DateTime.Now;
		}
		
		
		private static long						current = 1;
		private static System.DateTime			time = System.DateTime.Now;
	}
}
