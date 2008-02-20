//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
