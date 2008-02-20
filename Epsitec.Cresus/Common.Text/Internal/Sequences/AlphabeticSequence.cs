//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		
		public override Generator.SequenceType	WellKnownType
		{
			get
			{
				return Generator.SequenceType.Alphabetic;
			}
		}
		
		
		public override bool ParseText(string text, out int value)
		{
			if ((text == null) ||
				(text.Length == 0))
			{
				value = 0;
				return false;
			}
			
			value = this.alphabet.ToLower ().IndexOf (text.ToLower ()[0]) + 1;
			
			return (value > 0);
		}
		
		
		protected override string GetRawText(int rank, System.Globalization.CultureInfo culture)
		{
			if ((rank < 1) ||
				(rank > this.alphabet.Length))
			{
				return "?";
			}
			
			return string.Format (culture, "{0}", this.alphabet[rank-1]);
		}
		
		protected override string GetSetupArgument()
		{
			return this.alphabet;
		}
		
		protected override void Setup(string argument)
		{
			this.alphabet = argument;
		}

		
		private string							alphabet;
	}
}
