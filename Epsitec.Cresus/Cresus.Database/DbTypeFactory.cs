//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 20/11/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTypeFactory permet d'instancier le bon type DbType (et
	/// ses d�riv�s).
	/// </summary>
	public class DbTypeFactory
	{
		private DbTypeFactory()
		{
		}
		
		public static DbType NewType(string xml)
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			doc.LoadXml (xml);
			return DbTypeFactory.NewType (doc.DocumentElement);
		}
		
		public static DbType NewType(System.Xml.XmlElement xml)
		{
			if (xml.Name == "null")
			{
				return null;
			}
			
			if (xml.Name != "type")
			{
				throw new System.FormatException (string.Format ("Expected root element named <type>, but found <{0}>.", xml.Name));
			}
			
			string type_class = xml.GetAttribute ("class");
			
			if (type_class.Length == 0)
			{
				throw new System.FormatException (string.Format ("Tag <type> does not define attribute named 'class'."));
			}
			
			DbType type = null;
			
			switch (type_class)
			{
				case "base":	type = new DbType ();			break;
				case "enum":	type = new DbTypeEnum ();		break;
				case "num":		type = new DbTypeNum ();		break;
				case "str":		type = new DbTypeString ();		break;
				
				default:
					throw new System.FormatException (string.Format ("Unsupported value for <type class='{0}'>.", type_class));
			}
			
			int index = 0;
			
			type.DeserialiseXmlAttributes (xml);
			type.DeserialiseXmlElements (xml.ChildNodes, ref index);
			
			return type;
		}
		
		public static string SerialiseToXml(DbType type, bool full)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			DbTypeFactory.SerialiseToXml (buffer, type, full);
			return buffer.ToString ();
		}
		
		public static void SerialiseToXml(System.Text.StringBuilder buffer, DbType type, bool full)
		{
			if (type != null)
			{
				System.Type type_type = type.GetType ();
				string class_name;
				
				if (type_type == DbTypeFactory.type_base)
				{
					class_name = "base";
				}
				else if (type_type == DbTypeFactory.type_enum)
				{
					class_name = "enum";
				}
				else if (type_type == DbTypeFactory.type_num)
				{
					class_name = "num";
				}
				else if (type_type == DbTypeFactory.type_str)
				{
					class_name = "str";
				}
				else
				{
					throw new System.ArgumentException (string.Format ("Unsupported type specified: {0}.", type_type.Name));
				}
				
				buffer.Append (@"<type class=""");
				buffer.Append (class_name);
				buffer.Append (@"""");
				
				//	Ajoute ce qui est propre � chaque classe...
				
				type.SerialiseXmlAttributes (buffer, full);
				
				if (full)
				{
					buffer.Append (@">");
					type.SerialiseXmlElements (buffer, true);
					buffer.Append (@"</type>");
				}
				else
				{
					buffer.Append (@"/>");
				}
			}
		}
		
		
		static readonly System.Type		type_base = typeof (DbType);
		static readonly System.Type		type_enum = typeof (DbTypeEnum);
		static readonly System.Type		type_num  = typeof (DbTypeNum);
		static readonly System.Type		type_str  = typeof (DbTypeString);
	}
}
