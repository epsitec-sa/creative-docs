//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe StringType décrit des valeurs de type System.String.
	/// </summary>
	public class StringType : AbstractType, IStringType
	{
		public StringType()
			: base ("String", "String", null)
		{
		}

		public StringType(int minimumLength)
			: this ()
		{
			this.minimumLength = minimumLength;
		}

		public StringType(int minimumLength, int maximumLength)
			: this ()
		{
			this.minimumLength = minimumLength;
			this.maximumLength = maximumLength;
		}


		#region ISystemType Members
		public override System.Type				SystemType
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

		public override bool IsValidValue(object value)
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
