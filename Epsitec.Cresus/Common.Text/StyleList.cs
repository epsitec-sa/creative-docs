//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe StyleList g�re la liste des styles associ�s � un ou plusieurs
	/// textes.
	/// Note: "StyleList" se prononce comme "stylist" :-)
	/// </summary>
	public sealed class StyleList
	{
		public StyleList()
		{
			this.styles = new Internal.StyleTable ();
		}
		
		
		internal Internal.StyleTable			StyleTable
		{
			get
			{
				return this.styles;
			}
		}
		
		
		private Internal.StyleTable				styles;
	}
}
