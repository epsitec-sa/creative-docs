//	Copyright � 2005-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			//	Convertit un texte 'string' cod� sous une forme UTF-16 en un
			//	texte interne (21 bits + bits de contr�le).
			
			//	On part de l'hypoth�se que le texte pass� en entr�e ne contient
			//	aucun caract�re n�cessitant un traitement particulier, ce qui
			//	veut dire que l'on va avoir une relation 1 <-> 1 entre 'text'
			//	et 'result'; on peut donc allouer le buffer avec la taille
			//	ad�quate tout de suite :
			
			result = new uint[text.Length];
			
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				
				//	S'agit-il l� d'un caract�re potentiellement complexe ?
				
				if ((c >= Unicode.SurrogateMin) &&
					(c <= Unicode.SurrogateMax))
				{
					//	On vient de tomber sur une partie de 'surrogate pair'. Il
					//	faut donc traiter tout le texte comme un texte complexe :
					
					TextConverter.ConvertFromComplexString (text, i, ref result);
					break;
				}
				
				if (c == '\r')
				{
					//	On vient de tomber sur un CR qui doit �tre filtr� du texte
					//	car il n'est pas consid�r� comme valide en aval. Il faut
					//	donc traiter tout le texte comme un texte complexe :
					
					TextConverter.ConvertFromComplexString (text, i, ref result);
					break;
				}
				else if (c == '\n')
				{
					c = (char) Unicode.Code.ParagraphSeparator;
				}
				
				result[i] = c;
			}
		}
		
		public static void ConvertFromString(string text, out ulong[] result)
		{
			result = new ulong[text.Length];
			
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				
				//	S'agit-il l� d'un caract�re potentiellement complexe ?
				
				if ((c >= Unicode.SurrogateMin) &&
					(c <= Unicode.SurrogateMax))
				{
					//	On vient de tomber sur une partie de 'surrogate pair'. Il
					//	faut donc traiter tout le texte comme un texte complexe :
					
					TextConverter.ConvertFromComplexString (text, i, ref result);
					break;
				}
				
				if (c == '\r')
				{
					//	On vient de tomber sur un CR qui doit �tre filtr� du texte
					//	car il n'est pas consid�r� comme valide en aval. Il faut
					//	donc traiter tout le texte comme un texte complexe :
					
					TextConverter.ConvertFromComplexString (text, i, ref result);
					break;
				}
				else if (c == '\n')
				{
					c = (char) Unicode.Code.ParagraphSeparator;
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
						throw new Unicode.IllegalCodeException ();
					}
					
					//	Il faut coder ce caract�re comme un surrogate pair.
					
					code -= 0x010000;
					
					uint low  = (code >> 10) & 0x03FF;
					uint high = (code >>  0) & 0x03FF;
					
					buffer.Append ((char) (low  + Unicode.SurrogateLowMin));
					buffer.Append ((char) (high + Unicode.SurrogateHighMin));
				}
				else
				{
					if ((code >= Unicode.SurrogateMin) &&
						(code <= Unicode.SurrogateMax))
					{
						//	Un surrogate pair n'a pas le droit d'appara�tre dans le texte
						//	source, car UTF-32 consid�re ces codes comme non valides.
						
						throw new Unicode.IllegalCodeException ();
					}
					
					buffer.Append ((char) code);
				}
			}
		}
		
		public static void ConvertToString(ulong[] text, out string result)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			TextConverter.ConvertToString (text, buffer);
			
			result = buffer.ToString ();
		}
		
		public static void ConvertToString(ulong[] text, int length, out string result)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			TextConverter.ConvertToString (text, length, buffer);
			
			result = buffer.ToString ();
		}
		
		public static void ConvertToString(ulong[] text, System.Text.StringBuilder buffer)
		{
			TextConverter.ConvertToString(text, text.Length, buffer);
		}
		
		public static void ConvertToString(ulong[] text, int length, System.Text.StringBuilder buffer)
		{
			for (int i = 0; i < length; i++)
			{
				uint code = (uint) Unicode.Bits.GetCode (text[i]);
				
				if (code > 0x00FFFF)
				{
					if (code > 0x10FFFF)
					{
						throw new Unicode.IllegalCodeException ();
					}
					
					//	Il faut coder ce caract�re comme un surrogate pair.
					
					code -= 0x010000;
					
					uint low  = (code >> 10) & 0x03FF;
					uint high = (code >>  0) & 0x03FF;
					
					buffer.Append ((char) (low  + Unicode.SurrogateLowMin));
					buffer.Append ((char) (high + Unicode.SurrogateHighMin));
				}
				else
				{
					if ((code >= Unicode.SurrogateMin) &&
						(code <= Unicode.SurrogateMax))
					{
						//	Un surrogate pair n'a pas le droit d'appara�tre dans le texte
						//	source, car UTF-32 consid�re ces codes comme non valides.
						
						throw new Unicode.IllegalCodeException ();
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
			//	a allou� un buffer suffisamment grand pour stocker le r�sultat
			//	de la conversion, mais peut-�tre sera-t-il trop grand ?
			
			for (int i = index; i < text.Length; i++)
			{
				char c    = text[i];
				uint code = c;
				
				if ((c >= Unicode.SurrogateLowMin) &&
					(c <= Unicode.SurrogateLowMax))
				{
					//	Traitement des 'surrogate pairs'.
					//
					//	http://www.i18nguy.com/surrogates.html
					//	http://www.i18nguy.com/unicode/surrogatetable.html
					
					if ((i >= text.Length-1) ||
						(text[i+1] < Unicode.SurrogateHighMin) ||
						(text[i+1] > Unicode.SurrogateHighMax))
					{
						//	Demi paire ou paire incorrecte; ceci implique une erreur de
						//	codage de la part de l'appelant.
						
						throw new Unicode.IllegalCodeException ();
					}
					
					uint low  = (uint)text[i+0] - Unicode.SurrogateLowMin;
					uint high = (uint)text[i+1] - Unicode.SurrogateHighMin;
					
					Debug.Assert.IsInBounds (low, 0, 0x3FF);
					Debug.Assert.IsInBounds (high, 0, 0x3FF);
					
					code = (low << 10) + (high) + 0x10000;
					
					Debug.Assert.IsInBounds (code, 0x010000, 0x10FFFF);
					
					//	Saute le caract�re suivant dans la source...
					
					i++;
				}
				else if ((c >= Unicode.SurrogateHighMin) &&
					/**/ (c <= Unicode.SurrogateHighMax))
				{
					throw new Unicode.IllegalCodeException ();
				}
				else if (c == '\r')
				{
					//	Saut le caract�re dans la source...
					
					continue;
				}
				else if (c == '\n')
				{
					code = (char) Unicode.Code.ParagraphSeparator;
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
		
		private static void ConvertFromComplexString(string text, int index, ref ulong[] result)
		{
			//	Conversion d'un texte contenant des surrogate pairs. L'appelant
			//	a allou� un buffer suffisamment grand pour stocker le r�sultat
			//	de la conversion, mais peut-�tre sera-t-il trop grand ?
			
			for (int i = index; i < text.Length; i++)
			{
				char  c    = text[i];
				ulong code = c;
				
				if ((c >= Unicode.SurrogateLowMin) &&
					(c <= Unicode.SurrogateLowMax))
				{
					//	Traitement des 'surrogate pairs'.
					//
					//	http://www.i18nguy.com/surrogates.html
					//	http://www.i18nguy.com/unicode/surrogatetable.html
					
					if ((i >= text.Length-1) ||
						(text[i+1] < Unicode.SurrogateHighMin) ||
						(text[i+1] > Unicode.SurrogateHighMax))
					{
						//	Demi paire ou paire incorrecte; ceci implique une erreur de
						//	codage de la part de l'appelant.
						
						throw new Unicode.IllegalCodeException ();
					}
					
					ulong low  = (ulong)text[i+0] - Unicode.SurrogateLowMin;
					ulong high = (ulong)text[i+1] - Unicode.SurrogateHighMin;
					
					Debug.Assert.IsInBounds (low, 0, 0x3FF);
					Debug.Assert.IsInBounds (high, 0, 0x3FF);
					
					code = (low << 10) + (high) + 0x10000;
					
					Debug.Assert.IsInBounds (code, 0x010000, 0x10FFFF);
					
					//	Saute le caract�re suivant dans la source...
					
					i++;
				}
				else if ((c >= Unicode.SurrogateHighMin) &&
					/**/ (c <= Unicode.SurrogateHighMax))
				{
					throw new Unicode.IllegalCodeException ();
				}
				else if (c == '\r')
				{
					//	Saut le caract�re dans la source...
					
					continue;
				}
				else if (c == '\n')
				{
					code = (char) Unicode.Code.ParagraphSeparator;
				}
				
				result[index++] = code;
			}
			
			if (index < result.Length)
			{
				ulong[] old_result = result;
				ulong[] new_result = new ulong[index];
				
				System.Buffer.BlockCopy (old_result, 0, new_result, 0, index*8);
				
				result = new_result;
			}
		}
	}
}
