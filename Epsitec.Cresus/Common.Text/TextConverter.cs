//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// Summary description for TextConverter.
	/// </summary>
	public sealed class TextConverter
	{
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
				
				if ((c >= TextConverter.UnicodeSurrogateMin) &&
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
		
		public static void ConvertToString(uint[] text, out string result)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			TextConverter.ConvertToString (text, buffer);
			
			result = buffer.ToString ();
		}
		
		public static void ConvertToString(uint[] text, System.Text.StringBuilder buffer)
		{
			for (int i = 0; i < text.Length; i++)
			{
				uint code = text[i];
				
				if (code > 0x00FFFF)
				{
					if (code > 0x10FFFF)
					{
						throw new IllegalUnicodeException ();
					}
					
					//	Il faut coder ce caractère comme un surrogate pair.
					
					code -= 0x010000;
					
					uint low  = (code >> 10) & 0x03FF;
					uint high = (code >>  0) & 0x03FF;
					
					buffer.Append ((char) (low  + TextConverter.UnicodeSurrogateLowMin));
					buffer.Append ((char) (high + TextConverter.UnicodeSurrogateHighMin));
				}
				else
				{
					if ((code >= TextConverter.UnicodeSurrogateMin) &&
						(code <= TextConverter.UnicodeSurrogateMax))
					{
						//	Un surrogate pair n'a pas le droit d'apparaître dans le texte
						//	source, car UTF-32 considère ces codes comme non valides.
						
						throw new IllegalUnicodeException ();
					}
					
					buffer.Append ((char) code);
				}
			}
		}
		
		
		private TextConverter()
		{
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
					//	Traitement des 'surrogate pairs'.
					//
					//	http://www.i18nguy.com/surrogates.html
					//	http://www.i18nguy.com/unicode/surrogatetable.html
					
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
					
					code = (low << 10) + (high) + 0x10000;
					
					Debug.Assert.IsInBounds (code, 0x010000, 0x10FFFF);
					
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
