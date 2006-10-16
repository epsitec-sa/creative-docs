//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>DesignerAttribute</c> attribute can be used to mark an <c>enum</c>
	/// as visible in the designer.
	/// </summary>

	[System.Serializable]
	[System.AttributeUsage (System.AttributeTargets.Enum, AllowMultiple = false)]

	public sealed class DesignerAttribute : System.Attribute
	{
		public DesignerAttribute()
		{
		}
	}
}
