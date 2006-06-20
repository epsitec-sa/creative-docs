//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe StringType décrit des valeurs de type System.String.
	/// </summary>
	public class StringType : NamedDependencyObject, IStringType, IDataConstraint
	{
		public StringType() : base ("String")
		{
		}

		public StringType(int minimumLength)
		{
			this.minimumLength = minimumLength;
		}

		public StringType(int minimumLength, int maximumLength)
		{
			this.minimumLength = minimumLength;
			this.maximumLength = maximumLength;
		}


		#region INamedType Members

		public string DefaultController
		{
			get
			{
				return "String";
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
		public System.Type						SystemType
		{
			get
			{
				return typeof (System.String);
			}
		}
		#endregion
		
		#region IStringType Members

		public int								MinimumLength
		{
			get
			{
				return this.minimumLength;
			}
		}

		public int								MaximumLength
		{
			get
			{
				return this.maximumLength;
			}
		}
		
		#endregion

		#region IDataConstraint Members

		public bool IsValidValue(object value)
		{
			string text = value as string;

			if (text != null)
			{
				int length = text.Length;

				if ((length >= this.MinimumLength) &&
					(length <= this.MaximumLength))
				{
					return true;
				}
			}
			
			return false;
		}

		#endregion
		
		public static readonly StringType		Default = new StringType ();
		
		private int								minimumLength = 0;
		private int								maximumLength = 100*1000;
	}
}
