//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La classe CharMarker g�re l'attribution des marqueurs temporaires
	/// associ�s au texte; ces marqueurs sont stock�s comme des fanions aux
	/// bits 24..31 de chaque caract�re.
	/// </summary>
	internal sealed class CharMarker : System.Collections.IEnumerable
	{
		public CharMarker()
		{
			this.keys = new object[8];
			this.bits = new ulong[8];
			
			for (int i = 0; i < 8; i++)
			{
				this.keys[i] = null;
				this.bits[i] = 0x01000000ul << i;
			}
		}
		
		
		/*
		 *	Structure d'un caract�re en m�moire :
		 *
		 *	[a][bbb bbbb]:[cccc ccc][d:dddd dddd:dddd dddd]:[mmmm mmmm]:[xxx][y yyyy:yyyy yyyy:yyyy yyyy]
		 *
		 *	- bit 63 : a,  1-bit, "rich style flag"
		 *	- 62..56 : b,  7-bit, "extra index"			1..100
		 *	- 55..49 : c,  7-bit, "local index"			1..100
		 *	- 48..32 : d, 17-bit, "style index"			1..100'000
		 *	- 31..24 : m,  8-bit, "markers"
		 *  - 23..21 : x,  3-bit, "unicode flags"
		 *	- 20...0 : y, 21-bit, "unicode code"
		 *
		 *	- Le "rich style flag" indique si le "style index" pointe vers une
		 *	  description de style simple (false => Styles.SimpleStyle) ou vers
		 *	  une description de style riche (true => Styles.RichStyle).
		 *
		 *	- Le "extra index" pointe (au sein du style) sur un descripteur qui
		 *	  d�finit des propri�t�s non typographiques (couleur, langue, etc.)
		 *
		 *	- Le "local index" pointe (au sein du style) sur un descripteur qui
		 *	  d�finit des ajustement locaux (cr�nage, variante de glyphe, etc.)
		 *
		 *	- Le "style index" pointe soit sur un descripteur qui d�finit toues
		 *	  les propri�t�s principales (fonte, style, paragraphe, etc.)
		 *
		 *	- Les "markers" sont des bits de marquage temporaire (g�r�s par la
		 *	  class CharMarker).
		 *
		 *	- Les "unicode flags" et "unicode code" forment le caract�re Unicode
		 *	  � proprement dit.
		 */
		
		public ulong							this[object key]
		{
			get
			{
				if (key != null)
				{
					for (int i = 0; i < this.keys.Length; i++)
					{
						if (this.keys[i] == key)
						{
							return this.bits[i];
						}
					}
				}
				
				throw new System.ArgumentException ("Invalid key.");
			}
		}
		
		
		public bool Add(object key)
		{
			if (key != null)
			{
				int free = -1;
				
				for (int i = 0; i < this.keys.Length; i++)
				{
					if (this.keys[i] == key)
					{
						throw new System.ArgumentException ("Same key specified twice.");
					}
					if ((this.keys[i] == null) &&
						(free < 0))
					{
						free = i;
					}
				}
				
				if (free != -1)
				{
					this.keys[free] = key;
					return true;
				}
			}
			
			return false;
		}
		
		public void Remove(object key)
		{
			for (int i = 0; i < this.keys.Length; i++)
			{
				if (this.keys[i] == key)
				{
					this.keys[i] = null;
					return;
				}
			}
			
			throw new System.ArgumentException ("Invalid key.");
		}
		
		
		public static bool SetMarkers(ulong marker, ulong[] text, int offset, int length)
		{
			//	Place le marqueur sp�cifi� sur le fragment de texte et retourne
			//	'true' si des modifications ont �t� apport�es au texte.
			
			Debug.Assert.IsTrue (marker != 0);
			Debug.Assert.IsTrue ((marker & 0xFFFFFFFF00FFFFFFul) == 0);
			
			ulong acc = marker;
			int   end = offset + length;
			
			for (int i = offset; i < end; i++)
			{
				acc     &= text[i];
				text[i] |= marker;
			}
			
			return ((acc & marker) == 0) ? true : false;
		}
		
		public static bool ClearMarkers(ulong marker, ulong[] text, int offset, int length)
		{
			//	Efface le marqueur sp�cifi� sur le fragment de texte et retourne
			//	'true' si des modifications ont �t� apport�es au texte.
			
			Debug.Assert.IsTrue (marker != 0);
			Debug.Assert.IsTrue ((marker & 0xFFFFFFFF00FFFFFFul) == 0);
			
			ulong mask = ~marker;
			ulong acc  = 0;
			int   end  = offset + length;
			
			for (int i = offset; i < end; i++)
			{
				acc     |= text[i];
				text[i] &= mask;
			}
			
			return ((acc & marker) != 0) ? true : false;
		}
		
		
		public static ulong Accumulate(ulong[] text, int offset, int length)
		{
			//	Accumule (avec un "ou" logique) les marqueurs du texte pass�
			//	en entr�e.
			
			int   end = offset + length;
			ulong acc = 0;
			
			for (int i = offset; i < end; i++)
			{
				acc |= text[i] & 0x00000000FF000000UL;
			}
			
			return acc;
		}
		
		
		public static int GetStyleIndex(ulong code)
		{
			return (int)((code >> 32) & 0x000000000001FFFFul);
		}
		
		public static int GetLocalIndex(ulong code)
		{
			return (int)((code >> 49) & 0x000000000000003Ful);
		}
		
		public static int GetExtraIndex(ulong code)
		{
			return (int)((code >> 56) & 0x000000000000003Ful);
		}
		
		
		public static void SetStyleIndex(ref ulong code, int value)
		{
			code &= 0xFFFE0000FFFFFFFFul;
			code |= (uint) (value & 0x0001FFFF) << 32;
		}
		
		public static void SetLocalIndex(ref ulong code, int value)
		{
			code &= 0xFF01FFFFFFFFFFFFul;
			code |= (uint) (value & 0x0000007F) << 49;
		}
		
		public static void SetExtraIndex(ref ulong code, int value)
		{
			code &= 0x80FFFFFFFFFFFFFFul;
			code |= (uint) (value & 0x0000007F) << 56;
		}
		
		
		public static bool HasStyleOrSettings(ulong code)
		{
			return ((code & 0xFFFFFFFF00000000ul) == 0) ? false : true;
		}
		
		public static bool HasSettings(ulong code)
		{
			return ((code & 0x7FFE000000000000ul) == 0) ? false : true;
		}
		
		
		public static bool HasRichStyleFlag(ulong code)
		{
			return ((code & 0x8000000000000000ul) == 0) ? false : true;
		}
		
		public static void SetRichStyleFlag(ref ulong code, bool flag)
		{
			if (flag)
			{
				code |= 0x8000000000000000ul;
			}
			else
			{
				code &= 0x7FFFFFFFFFFFFFFFul;
			}
		}
		
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			for (int i = 0; i < this.keys.Length; i++)
			{
				if (this.keys[i] != null)
				{
					list.Add (this.keys[i]);
				}
			}
			
			return list.GetEnumerator ();
		}
		#endregion
		
		private object[]						keys;
		private ulong[]							bits;
	}
}
