//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe HideAttribute d�finit un attribut [Hide] qui peut �tre
	/// utilis� pour cacher des valeurs d'une �num�ration.
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
