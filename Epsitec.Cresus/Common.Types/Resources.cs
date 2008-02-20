//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe Resources exporte des fonctions qui ne sont pas accessibles
	/// dans Support (problème de références circulaires).
	/// </summary>
	internal sealed class Resources
	{
		private Resources()
		{
		}

		
		public static string MakeTextRef(string id)
		{
			System.Diagnostics.Debug.Assert (id != null);
			System.Diagnostics.Debug.Assert (id.Length > 0);
			
			return string.Concat ("[res:", id, "]");
		}
		
		public static string MakeTextRef(string id1, string id2)
		{
			System.Diagnostics.Debug.Assert (id1 != null);
			System.Diagnostics.Debug.Assert (id2 != null);
			System.Diagnostics.Debug.Assert (id1.Length > 0);
			System.Diagnostics.Debug.Assert (id2.Length > 0);
			
			return string.Concat ("[res:", id1, ".", id2, "]");
		}
		
		public static string MakeTextRef(string id1, string id2, string id3)
		{
			System.Diagnostics.Debug.Assert (id1 != null);
			System.Diagnostics.Debug.Assert (id2 != null);
			System.Diagnostics.Debug.Assert (id3 != null);
			System.Diagnostics.Debug.Assert (id1.Length > 0);
			System.Diagnostics.Debug.Assert (id2.Length > 0);
			System.Diagnostics.Debug.Assert (id3.Length > 0);
			
			return string.Concat ("[res:", id1, ".", id2, ".", id3, "]");
		}
	}
}
