//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en développement DD, 17/04/2004

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTypeByteArray définit les paramètres d'un tableau binaire
	/// (DbSimpleType.ByteArray).
	/// </summary>
	public class DbTypeByteArray : DbType
	{
		internal DbTypeByteArray() : base (DbSimpleType.ByteArray)
		{
		}
		
		
		public DbTypeByteArray(int length) : this (length, true)
		{
		}
		
		public DbTypeByteArray(int length, bool is_fixed_length) : base (DbSimpleType.ByteArray)
		{
			this.length = length;
			this.is_fixed_length = is_fixed_length;
		}
		
		public DbTypeByteArray(int length, params string[] attributes) : this (length, true, attributes)
		{
		}
		
		public DbTypeByteArray(int length, bool is_fixed_length, params string[] attributes) : base (DbSimpleType.ByteArray, attributes)
		{
			this.length = length;
			this.is_fixed_length = is_fixed_length;
		}
		
		
		internal override void SerialiseXmlAttributes(System.Text.StringBuilder buffer, bool full)
		{
			buffer.Append (@" length=""");
			buffer.Append (this.length.ToString (System.Globalization.CultureInfo.InvariantCulture));
			
			if (this.is_fixed_length)
			{
				buffer.Append (@""" fixed=""1""");
			}
			else
			{
				buffer.Append (@"""");
			}
			
			base.SerialiseXmlAttributes (buffer, full);
		}
		
		internal override void DeserialiseXmlAttributes(System.Xml.XmlElement xml)
		{
			base.DeserialiseXmlAttributes (xml);
			
			string arg_length = xml.GetAttribute ("length");
			string arg_fixed  = xml.GetAttribute ("fixed");
			
			if (arg_length.Length == 0)
			{
				throw new System.ArgumentException ("No length specification found.");
			}
			
			this.length          = System.Int32.Parse (arg_length, System.Globalization.CultureInfo.InvariantCulture);
			this.is_fixed_length = (arg_fixed == "1") ? true : false;
		}
		
		
		public int						Length
		{
			get { return this.length; }
		}
		
		public bool						IsFixedLength
		{
			get
			{
				return this.is_fixed_length;
			}
		}
		
		
		protected override object CloneNewObject()
		{
			return new DbTypeByteArray ();
		}
		
		protected override object CloneCopyToNewObject(object o)
		{
			DbTypeByteArray that = o as DbTypeByteArray;
			
			base.CloneCopyToNewObject (that);
			
			that.length = this.length;
			that.is_fixed_length = this.is_fixed_length;
			
			return that;
		}
		
		
		private int						length;
		private bool					is_fixed_length;		// cela a-t-il un sens pour un byte[] ?
	}
}
