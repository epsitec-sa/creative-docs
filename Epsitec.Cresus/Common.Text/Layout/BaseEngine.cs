//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// La classe BaseEngine sert de classe de base pour tous les moteurs de
	/// layout (LineEngine, etc.)
	/// </summary>
	public abstract class BaseEngine
	{
		public BaseEngine()
		{
		}
		
		
		public virtual Layout.Status Fit(Layout.Context context, ref Layout.BreakCollection result)
		{
			return Layout.Status.Ok;
		}
		
		
		public int GetRunLength(ulong[] text, int start, int length)
		{
			//	Détermine combien de caractères utilisent exactement les mêmes
			//	propriétés de style & réglages dans le texte passé en entrée.
			
			ulong code = Internal.CharMarker.ExtractStyleAndSettings (text[start]);
			
			for (int i = 1; i < length; i++)
			{
				if (Internal.CharMarker.ExtractStyleAndSettings (text[start+i]) != code)
				{
					return i;
				}
			}
			
			return length;
		}
		
		public int GetNextFragmentLength(ulong[] text, int start, int length, int fragment_length)
		{
			//	Détermine la taille d'un fragment de texte (prochaine césure) à
			//	partir d'une longueur de départ.
			
			for (int i = fragment_length; i < length; i++)
			{
				Unicode.BreakInfo info = Unicode.Bits.GetBreakInfo (text[start+i]);
				
				if ((info == Unicode.BreakInfo.HyphenatePoorChoice) ||
					(info == Unicode.BreakInfo.HyphenateGoodChoice))
				{
					return i+1;
				}
				
				Debug.Assert.IsTrue ((info == Unicode.BreakInfo.No) || (info == Unicode.BreakInfo.NoAlpha) || (i+1 == length));
			}
			
			return length;
		}
	}
}
