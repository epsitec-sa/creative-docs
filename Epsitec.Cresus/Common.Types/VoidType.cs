//	Copyright � 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe VoidType d�crit les valeurs de type System.Void.
	/// </summary>
	public sealed class VoidType : INamedType
	{
		public VoidType()
		{
		}
		
		
		#region INamedType Members
		public System.Type						SystemType
		{
			get
			{
				return typeof (void);			//	System.Void
			}
		}
		#endregion
		
		#region INameCaption Members
		public string							Name
		{
			get
			{
				return "Void";
			}
		}

		public string							Caption
		{
			get
			{
				return null;
			}
		}

		public string							Description
		{
			get
			{
				return null;
			}
		}
		#endregion
		
		public static readonly VoidType			Default = new VoidType ();
	}
}
