//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe HideAttribute définit un attribut [Hide] qui peut être
	/// utilisé pour cacher des valeurs d'une énumération.
	/// </summary>
	
	[System.Serializable]
	[System.AttributeUsage (System.AttributeTargets.Field)]
	
	public class HideAttribute : System.Attribute
	{
		public HideAttribute()
		{
		}
	}
}
