//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// 
	/// </summary>
	public static class DruidType
	{
		public static OtherType Default
		{
			get
			{
				TypeRosetta.InitializeKnownTypes ();

				if (DruidType.defaultValue == null)
				{
					DruidType.defaultValue = (OtherType) TypeRosetta.CreateTypeObject (Support.Druid.Parse ("[100K]"));
				}

				return DruidType.defaultValue;
			}
		}

		private static OtherType defaultValue;
	}
}
