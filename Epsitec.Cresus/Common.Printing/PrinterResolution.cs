//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 21/03/2004

namespace Epsitec.Common.Printing
{
	/// <summary>
	/// La classe PrinterResolution d�crit une r�solution d'imprimante.
	/// </summary>
	public class PrinterResolution
	{
		internal PrinterResolution(System.Drawing.Printing.PrinterResolution pr)
		{
			this.pr = pr;
		}
		
		
		public int								DpiX
		{
			get
			{
				return this.pr.X;
			}
		}
		
		public int								DpiY
		{
			get
			{
				return this.pr.Y;
			}
		}
		
		
		
		System.Drawing.Printing.PrinterResolution pr;
	}
}
