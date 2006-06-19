//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe HiddenAttribute définit un attribut [Hidden] qui peut être
	/// utilisé pour cacher des valeurs d'une énumération.
	/// </summary>
	
	[System.Serializable]
	[System.AttributeUsage (System.AttributeTargets.Field)]
	
	public sealed class HiddenAttribute : System.Attribute
	{
		public HiddenAttribute()
		{
		}
	}
}
