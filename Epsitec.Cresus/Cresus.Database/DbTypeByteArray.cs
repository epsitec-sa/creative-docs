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
		public DbTypeByteArray() : base (DbSimpleType.ByteArray)
		{
		}
		
		public DbTypeByteArray(params string[] attributes) : base (DbSimpleType.ByteArray, attributes)
		{
		}
		
		
		internal override void SerializeXmlAttributes(System.Text.StringBuilder buffer, bool full)
		{
			base.SerializeXmlAttributes (buffer, full);
		}
		
		internal override void DeserializeXmlAttributes(System.Xml.XmlElement xml)
		{
			base.DeserializeXmlAttributes (xml);
		}
		
		
		protected override object CloneNewObject()
		{
			return new DbTypeByteArray ();
		}
		
		protected override object CloneCopyToNewObject(object o)
		{
			DbTypeByteArray that = o as DbTypeByteArray;
			
			base.CloneCopyToNewObject (that);
			
			return that;
		}
	}
}
