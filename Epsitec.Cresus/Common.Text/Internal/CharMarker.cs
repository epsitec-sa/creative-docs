//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La classe CharMarker gère l'attribution des marqueurs temporaires
	/// associés au texte; ces marqueurs sont stockés comme des fanions aux
	/// bits 24..31 de chaque caractère.
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
		 *	Structure d'un caractère en mémoire :
		 *
		 *	[bbbb bbb][c:cccc cc][dd:dddd dddd:dddd dddd]:[mmmm mmmm]:[xxx][y yyyy:yyyy yyyy:yyyy yyyy]
		 *
		 *	- 63..57 : b,  7-bit, "extra index"			1..100
		 *	- 56..50 : c,  7-bit, "local index"			1..100
		 *	- 49..32 : d, 18-bit, "style index"			1..250'000
		 *	- 31..24 : m,  8-bit, "markers"
		 *  - 23..21 : x,  3-bit, "unicode flags"
		 *	- 20...0 : y, 21-bit, "unicode code"
		 *
		 *	- Le "extra index" pointe (au sein du style) sur un descripteur qui
		 *	  définit des propriétés non typographiques (couleur, langue, etc.)
		 *
		 *	- Le "local index" pointe (au sein du style) sur un descripteur qui
		 *	  définit des ajustement locaux (crénage, variante de glyphe, etc.)
		 *
		 *	- Le "style index" pointe soit sur un descripteur qui définit toues
		 *	  les propriétés principales (fonte, style, paragraphe, etc.)
		 *
		 *	- Les "markers" sont des bits de marquage temporaire (gérés par la
		 *	  class CharMarker).
		 *
		 *	- Les "unicode flags" et "unicode code" forment le caractère Unicode
		 *	  à proprement dit.
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
			//	Place le marqueur spécifié sur le fragment de texte et retourne
			//	'true' si des modifications ont été apportées au texte.
			
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
			//	Efface le marqueur spécifié sur le fragment de texte et retourne
			//	'true' si des modifications ont été apportées au texte.
			
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
			//	Accumule (avec un "ou" logique) les marqueurs du texte passé
			//	en entrée.
			
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
			return (int)((code >> 32) & 0x000000000003FFFFul);
		}
		
		public static int GetLocalIndex(ulong code)
		{
			return (int)((code >> 50) & 0x000000000000003Ful);
		}
		
		public static int GetExtraIndex(ulong code)
		{
			return (int)((code >> 57) & 0x000000000000003Ful);
		}
		
		
		public static void SetStyleIndex(ref ulong code, int value)
		{
			code &= 0xFFFC0000FFFFFFFFul;
			code |= (uint) (value & 0x0003FFFF) << 32;
		}
		
		public static void SetLocalIndex(ref ulong code, int value)
		{
			code &= 0xFE03FFFFFFFFFFFFul;
			code |= (uint) (value & 0x0000007F) << 50;
		}
		
		public static void SetExtraIndex(ref ulong code, int value)
		{
			code &= 0x01FFFFFFFFFFFFFFul;
			code |= (uint) (value & 0x0000007F) << 57;
		}
		
		
		public static bool HasStyleOrSettings(ulong code)
		{
			return ((code & 0xFFFFFFFF00000000ul) == 0) ? false : true;
		}
		
		public static bool HasSettings(ulong code)
		{
			return ((code & 0xFFFC000000000000ul) == 0) ? false : true;
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
