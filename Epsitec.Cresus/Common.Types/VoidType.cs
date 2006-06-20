//	Copyright � 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe VoidType d�crit les valeurs de type System.Void.
	/// </summary>
	public sealed class VoidType : NamedDependencyObject, INamedType
	{
		public VoidType() : base ("Void")
		{
		}

		#region INamedType Members

		public string DefaultController
		{
			get
			{
				return null;
			}
		}

		public string DefaultControllerParameter
		{
			get
			{
				return null;
			}
		}

		#endregion

		#region ISystemType Members

		public System.Type SystemType
		{
			get
			{
				return typeof (void);
			}
		}

		#endregion

		public static readonly VoidType Default = new VoidType ();
	}
}
