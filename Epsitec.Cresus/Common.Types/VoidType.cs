//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.VoidType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe VoidType décrit les valeurs de type System.Void.
	/// </summary>
	public sealed class VoidType : AbstractType
	{
		public VoidType()
			: base ("Void")
		{
		}

		public VoidType(Caption caption)
			: base (caption)
		{
		}

		#region ISystemType Members

		public override System.Type SystemType
		{
			get
			{
				return typeof (void);
			}
		}

		#endregion

		public override bool IsValidValue(object value)
		{
			return false;
		}

		public static VoidType Default
		{
			get
			{
				if (VoidType.defaultValue == null)
				{
					VoidType.defaultValue = (VoidType) TypeRosetta.CreateTypeObject (Support.Druid.Parse ("[1009]"));
				}

				return VoidType.defaultValue;
			}
		}

		private static VoidType defaultValue;
	}
}
