//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 26/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe StringType décrit le type 'string' natif.
	/// </summary>
	public class StringType : IString
	{
		public StringType()
		{
			this.length = 100000;
		}
		
		
		#region INamedType Members
		public System.Type						SystemType
		{
			get
			{
				return typeof (string);
			}
		}
		#endregion
		
		#region IString Members
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
		
		
		private int								length;
	}
}
