//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTypeDateTime définit le type DateTime système. Il n'y
	/// a aucune paramétrisation.
	/// </summary>
	public class DbTypeDateTime : DbType
	{
		public DbTypeDateTime() : base (DbSimpleType.DateTime)
		{
		}
		
		public DbTypeDateTime(params string[] attributes) : base (DbSimpleType.DateTime, attributes)
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
			return new DbTypeDateTime ();
		}
		
		protected override object CloneCopyToNewObject(object o)
		{
			DbTypeDateTime that = o as DbTypeDateTime;
			
			base.CloneCopyToNewObject (that);
			
			return that;
		}
	}
}
