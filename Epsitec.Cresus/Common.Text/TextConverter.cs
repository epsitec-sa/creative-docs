//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// Summary description for TextConverter.
	/// </summary>
	public sealed class TextConverter
	{
		private TextConverter()
		{
		}
		
		public static void ConvertFromString(string text, out uint[] result)
		{
			//	Convertit un texte 'string' codé sous une forme UTF-16 en un
			//	texte interne (21 bits + bits de contrôle).
			
			//	On part de l'hypothèse que le texte passé en entrée ne contient
			//	aucun caractère nécessitant un traitement particulier, ce qui
			//	veut dire que l'on va avoir une relation 1 <-> 1 entre 'text'
			//	et 'result'; on peut donc allouer le buffer avec la taille
			//	adéquate tout de suite :
			
			result = new uint[text.Length];
			
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				
				//	S'agit-il là d'un caractère potentiellement complexe ?
				
				if ((c >= TextConverter.UnicodeSurrogateMin) ||
					(c <= TextConverter.UnicodeSurrogateMax))
				{
					//	On vient de tomber sur une partie de 'surrogate pair'. Il
					//	faut donc traiter tout le texte comme un texte complexe :
					
					TextConverter.ConvertFromComplexString (text, i, ref result);
					break;
				}
				
				result[i] = c;
			}
		}
		
		
		private static void ConvertFromComplexString(string text, int index, ref uint[] result)
		{
			//	Conversion d'un texte contenant des surrogate pairs. L'appelant
			//	a alloué un buffer suffisamment grand pour stocker le résultat
			//	de la conversion, mais peut-être sera-t-il trop grand ?
			
			for (int i = index; i < text.Length; i++)
			{
				char c    = text[i];
				uint code = c;
				
				if ((c >= TextConverter.UnicodeSurrogateLowMin) &&
					(c <= TextConverter.UnicodeSurrogateLowMax))
				{
					if ((i >= text.Length-1) ||
						(text[i+1] < TextConverter.UnicodeSurrogateHighMin) ||
						(text[i+1] > TextConverter.UnicodeSurrogateHighMax))
					{
						//	Demi paire ou paire incorrecte; ceci implique une erreur de
						//	codage de la part de l'appelant.
						
						throw new IllegalUnicodeException ();
					}
					
					uint low  = (uint)text[i+0] - TextConverter.UnicodeSurrogateLowMin;
					uint high = (uint)text[i+1] - TextConverter.UnicodeSurrogateHighMin;
					
					Debug.Assert.IsInBounds (low, 0, 0x3FF);
					Debug.Assert.IsInBounds (high, 0, 0x3FF);
					
					code = low + (high << 10) + 0x10000;
					
					Debug.Assert.IsInBounds (code, 0x00010000, 0x0010FFFF);
					
					//	Saute le caractère suivant dans la source...
					
					i++;
				}
				else if ((c >= TextConverter.UnicodeSurrogateHighMin) &&
					/**/ (c <= TextConverter.UnicodeSurrogateHighMax))
				{
					throw new IllegalUnicodeException ();
				}
				
				result[index++] = code;
			}
			
			if (index < result.Length)
			{
				uint[] old_result = result;
				uint[] new_result = new uint[index];
				
				System.Buffer.BlockCopy (old_result, 0, new_result, 0, index*4);
				
				result = new_result;
			}
		}
		
		
		#region IllegalUnicodeException Class
		public class IllegalUnicodeException : System.ApplicationException
		{
			public IllegalUnicodeException()
			{
			}
			
			public IllegalUnicodeException(string message) : base (message)
			{
			}
		}
		#endregion
		
		public const char						UnicodeSurrogateLowMin	= (char) 0xD800;
		public const char						UnicodeSurrogateLowMax	= (char) 0xDBFF;
		public const char						UnicodeSurrogateHighMin = (char) 0xDC00;
		public const char						UnicodeSurrogateHighMax = (char) 0xDFFF;
		
		public const char						UnicodeSurrogateMin		= UnicodeSurrogateLowMin;
		public const char						UnicodeSurrogateMax		= UnicodeSurrogateHighMax;
	}
}
