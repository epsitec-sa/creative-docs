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
			
			string type_class = root.GetAttribute ("class");
			
			if (type_class == "")
			{
				throw new System.ArgumentException (string.Format ("Tag <type> does not define attribute named 'class'."));
			}
			
			DbType type = null;
			
			switch (type_class)
			{
				case "base":	type = new DbType (root);		break;
				case "num":		type = new DbTypeNum (root);	break;
				case "str":		type = new DbTypeString (root);	break;
				case "enum":	type = new DbTypeEnum (root);	break;
				
				default:
					throw new System.ArgumentException (string.Format ("Unsupported value for <type class='{0}'>.", type_class));
					
			}
			
			return type;
		}
	}
}
