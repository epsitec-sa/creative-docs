//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe StringType décrit des valeurs de type System.String.
	/// </summary>
	public class StringType : IStringType
	{
		public StringType()
		{
		}
		
		
		#region INamedType Members
		public System.Type						SystemType
		{
			get
			{
				return typeof (System.String);
			}
		}
		#endregion
		
		#region IStringType Members
		public int								Length
		{
			get
			{
				return this.length;
			}
		}
		#endregion

		#region INameCaption Members
		public string							Name
		{
			get
			{
				return "String";
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
		
		public static readonly StringType		Default = new StringType ();
		
		private int								length = 100000;
	}
}
