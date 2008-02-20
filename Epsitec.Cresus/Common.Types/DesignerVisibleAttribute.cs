//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>DesignerVisibleAttribute</c> attribute can be used to mark an <c>enum</c>
	/// as visible in the designer.
	/// </summary>

	[System.Serializable]
	[System.AttributeUsage (System.AttributeTargets.Enum, AllowMultiple = false)]

	public sealed class DesignerVisibleAttribute : System.Attribute
	{
		public DesignerVisibleAttribute()
		{
		}
	}
}
