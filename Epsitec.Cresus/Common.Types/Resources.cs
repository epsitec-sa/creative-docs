
namespace Epsitec.Common.Types
{
	/// <summary>
	/// Summary description for Resources.
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
			
			return string.Concat ("[res:", id1, ".", id2, "]");
		}
	}
}
