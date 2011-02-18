//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe StyleVersion abrite un compteur qui permet d'associer des
	/// numéros de version aux styles, tabulateurs, etc. afin de détecter les
	/// modifications.
	/// </summary>
	public sealed class StyleVersion
	{
		public StyleVersion()
		{
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
		
		
		public long ChangeVersion()
		{
			//	Il faut appeler cette méthode chaque fois qu'un style est
			//	modifié.
			
			long value = System.Threading.Interlocked.Increment (ref this.current);
			this.time = System.DateTime.Now;
			return value;
		}
		
		
		private long							current	= 1;
		private System.DateTime					time	= System.DateTime.Now;
	}
}
