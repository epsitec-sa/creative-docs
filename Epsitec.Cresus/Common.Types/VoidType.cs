//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		
		public static readonly VoidType Default = new VoidType ();
	}
}
