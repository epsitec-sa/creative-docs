//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>HiddenAttribute</c> attribute can be used to hide a specific item
	/// from the user-interface (for instance a value in an <c>enum</c>).
	/// </summary>

	[System.Serializable]
	[System.AttributeUsage (System.AttributeTargets.Field, AllowMultiple = false)]

	public sealed class HiddenAttribute : System.Attribute
	{
		public HiddenAttribute()
		{
		}
	}
}
