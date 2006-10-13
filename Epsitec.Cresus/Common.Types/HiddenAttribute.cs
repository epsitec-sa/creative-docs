//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>HiddenAttribute</c> attribute can be used to hide a specific item
	/// from the user-interface (for instance a value in an <c>enum</c> or a full
	/// <c>enum</c> when applied at the <c>enum</c> level).
	/// </summary>

	[System.Serializable]
	[System.AttributeUsage (System.AttributeTargets.Field | System.AttributeTargets.Enum, AllowMultiple = false)]

	public sealed class HiddenAttribute : System.Attribute
	{
		public HiddenAttribute()
		{
		}
	}
}
