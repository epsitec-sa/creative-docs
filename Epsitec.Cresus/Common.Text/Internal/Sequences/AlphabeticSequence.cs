//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal.Sequences
{
	/// <summary>
	/// La classe Alphabetic produit des lettres a, b, c ...
	/// </summary>
	public class Alphabetic : Generator.Sequence
	{
		public Alphabetic() : this ("abcdefghijklmnopqrstuvwxyz")
		{
		}
		
		public Alphabetic(string alphabet)
		{
			this.alphabet = alphabet;
		}
		
		
		protected override string GetRawText(int rank, System.Globalization.CultureInfo culture)
		{
			if ((rank <= 0) ||
				(rank >= this.alphabet.Length))
			{
				return "?";
			}
			
			return string.Format (culture, "{0}", this.alphabet[rank]);
		}
		
		
		private string							alphabet;
	}
}
