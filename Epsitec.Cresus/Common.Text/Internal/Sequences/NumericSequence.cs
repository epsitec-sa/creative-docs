//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal.Sequences
{
	/// <summary>
	/// La classe Numeric produit des numéros 1, 2, 3 ...
	/// </summary>
	public class Numeric : Generator.Sequence
	{
		public Numeric()
		{
		}
		
		
		protected override string GetRawText(int rank, System.Globalization.CultureInfo culture)
		{
			return string.Format (culture, "{0}", rank + 1);
		}
	}
}
