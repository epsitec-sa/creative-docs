//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 20/11/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTypeFactory permet d'instancier le bon type DbType (et
	/// ses dérivés).
	/// </summary>
	public class DbTypeFactory
	{
		private DbTypeFactory()
		{
		}
		
		public static DbType NewType(System.Xml.XmlElement root)
		{
			if (root.Name != "type")
			{
				throw new System.ArgumentException (string.Format ("Expected root element named <type>, but found <{0}>.", root.Name));
			}
			
			return null;
		}
		
		public static DbType NewType(string description)
		{
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] utf8_bytes = encoding.GetBytes (description);
			
			System.IO.MemoryStream   stream = new System.IO.MemoryStream (utf8_bytes, false);
			System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader (stream);
			
			DbType type = null;
			
			try
			{
				type = DbTypeFactory.NewType (reader);
			}
			catch
			{
				//	Mange l'exception.
			}
			finally
			{
				reader.Close ();
				stream.Close ();
			}
			
			return type;
		}
		
		public static DbType NewType(System.Xml.XmlTextReader reader)
		{
			return null;
		}
	}
}
