//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// Summary description for BaseEngine.
	/// </summary>
	public abstract class BaseEngine
	{
		public BaseEngine()
		{
		}
		
		
		public virtual Layout.Status Fit(Layout.Context context, out Layout.BreakCollection result)
		{
			result = null;
			return Layout.Status.Ok;
		}
		
		public int GetRunLength(ulong[] text, int start, int length)
		{
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
			for (int i = fragment_length; i < length; i++)
			{
				Unicode.BreakInfo info = Unicode.Bits.GetBreakInfo (text[start+i]);
				
				if ((info == Unicode.BreakInfo.HyphenatePoorChoice) ||
					(info == Unicode.BreakInfo.HyphenateGoodChoice))
				{
					return i+1;
				}
			}
			
			return length;
		}
	}
}
