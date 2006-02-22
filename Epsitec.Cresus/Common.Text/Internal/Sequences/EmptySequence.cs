//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal.Sequences
{
	/// <summary>
	/// La classe Empty produit des séquences vides.
	/// </summary>
	public class Empty : Generator.Sequence
	{
		public Empty()
		{
		}
		
		
		public override Generator.SequenceType	WellKnownType
		{
			get
			{
				return Generator.SequenceType.Empty;
			}
		}
		
		
		protected override string GetRawText(int rank, System.Globalization.CultureInfo culture)
		{
			return "";
		}
	}
}
