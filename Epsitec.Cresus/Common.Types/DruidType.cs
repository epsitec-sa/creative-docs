//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.DruidType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe DruidType décrit les valeurs de type Support.Druid.
	/// </summary>
	public sealed class DruidType : AbstractType
	{
		public DruidType()
			: base ("Druid")
		{
		}

		public DruidType(Caption caption)
			: base (caption)
		{
		}

		#region ISystemType Members

		public override System.Type SystemType
		{
			get
			{
				return typeof (Support.Druid);
			}
		}

		#endregion

		public override bool IsValidValue(object value)
		{
			return false;
		}

		public static DruidType Default
		{
			get
			{
				TypeRosetta.InitializeKnownTypes ();

				if (DruidType.defaultValue == null)
				{
					DruidType.defaultValue = (DruidType) TypeRosetta.CreateTypeObject (Support.Druid.Parse ("[100K]"));
				}

				return DruidType.defaultValue;
			}
		}

		private static DruidType defaultValue;
	}
}
