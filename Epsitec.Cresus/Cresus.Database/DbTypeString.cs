//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 19/11/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTypeString définit les paramètres d'un texte 'string'
	/// (DbSimpleType.String).
	/// </summary>
	public class DbTypeString : DbType
	{
		public DbTypeString(int length) : this (length, true)
		{
		}
		
		public DbTypeString(int length, bool is_fixed_length) : base (DbSimpleType.String)
		{
			this.length = length;
			this.is_fixed_length = is_fixed_length;
		}
		
		public DbTypeString(int length, params string[] attributes) : this (length, true, attributes)
		{
		}
		
		public DbTypeString(int length, bool is_fixed_length, params string[] attributes) : base (DbSimpleType.String, attributes)
		{
			this.length = length;
			this.is_fixed_length = is_fixed_length;
		}
		
		public DbTypeString(System.Xml.XmlElement xml) : base (DbSimpleType.String)
		{
			string arg_length = xml.GetAttribute ("length");
			string arg_fixed  = xml.GetAttribute ("fixed");
			
			if (arg_length == "")
			{
				throw new System.ArgumentException ("No length specification found.");
			}
			
			this.length          = System.Int32.Parse (arg_length, System.Globalization.CultureInfo.InvariantCulture);
			this.is_fixed_length = (arg_fixed == "1") ? true : false;
		}
		
		
		internal override void SerialiseXmlAttributes(System.Text.StringBuilder buffer)
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
		
		
		public override object Clone()
		{
			DbTypeString type = base.Clone () as DbTypeString;
			
			type.length = this.length;
			type.is_fixed_length = this.is_fixed_length;
			
			return type;
		}
		
		
		protected int					length;
		protected bool					is_fixed_length;
	}
}
