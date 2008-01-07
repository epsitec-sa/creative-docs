//	Copyright © 2004-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// 
	/// </summary>
	public static class VoidType
	{
		public static OtherType Default
		{
			get
			{
				TypeRosetta.InitializeKnownTypes ();

				if (VoidType.defaultValue == null)
				{
					VoidType.defaultValue = (OtherType) TypeRosetta.CreateTypeObject (Support.Druid.Parse ("[1009]"));
				}

				return VoidType.defaultValue;
			}
		}

		private static OtherType defaultValue;
	}
}
