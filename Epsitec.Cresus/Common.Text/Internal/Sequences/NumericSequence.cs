//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal.Sequences
{
	/// <summary>
	/// La classe Numeric produit des num�ros 1, 2, 3 ...
	/// </summary>
	public class Numeric : Generator.Sequence
	{
		public Numeric()
		{
		}
		
		
		public override Generator.SequenceType	WellKnownType
		{
			get
			{
				return Generator.SequenceType.Numeric;
			}
		}
		
		
		protected override string GetRawText(int rank, System.Globalization.CultureInfo culture)
		{
			return string.Format (culture, "{0}", rank);
		}
	}
}
