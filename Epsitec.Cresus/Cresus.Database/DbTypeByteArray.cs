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
		
		
		public DbTypeByteArray(int length) : base (DbSimpleType.ByteArray)
		{
			this.length = length;
		}
		
		public DbTypeByteArray(int length, params string[] attributes) : base (DbSimpleType.ByteArray, attributes)
		{
			this.length = length;
		}
		
		
		public int								Length
		{
			get { return this.length; }
		}
		
		
		internal override void SerializeXmlAttributes(System.Text.StringBuilder buffer, bool full)
		{
			buffer.Append (@" length=""");
			buffer.Append (this.length.ToString (System.Globalization.CultureInfo.InvariantCulture));
			buffer.Append (@"""");
			
			base.SerializeXmlAttributes (buffer, full);
		}
		
		internal override void DeserializeXmlAttributes(System.Xml.XmlElement xml)
		{
			base.DeserializeXmlAttributes (xml);
			
			string arg_length = xml.GetAttribute ("length");
			
			if (arg_length.Length == 0)
			{
				throw new System.ArgumentException ("No length specification found.");
			}
			
			this.length = System.Int32.Parse (arg_length, System.Globalization.CultureInfo.InvariantCulture);
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
			
			return that;
		}
		
		
		private int								length;
	}
}
