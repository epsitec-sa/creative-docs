//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;

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
		
		public static INamedType CreateType(string xml)
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			doc.LoadXml (xml);
			return DbTypeFactory.CreateType (doc.DocumentElement);
		}

		public static INamedType CreateType(System.Xml.XmlElement xml)
		{
			if (xml.Name == "null")
			{
				return null;
			}
			
			if (xml.Name != "type")
			{
				throw new System.FormatException (string.Format ("Expected root element named <type>, but found <{0}>.", xml.Name));
			}
			
			string typeCaptionId = xml.GetAttribute ("id");
			
			if (string.IsNullOrEmpty (typeCaptionId))
			{
				throw new System.FormatException (string.Format ("Tag <type> does not define attribute named 'class'."));
			}
			
			return TypeRosetta.GetTypeObject (Druid.Parse (typeCaptionId));
		}
		
		public static string SerializeToXml(INamedType type, bool full)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			DbTypeFactory.SerializeToXml (buffer, type, full);
			return buffer.ToString ();
		}
		
		public static void SerializeToXml(System.Text.StringBuilder buffer, INamedType type, bool full)
		{
			if (type != null)
			{
				buffer.Append (@"<type id=""");
				buffer.Append (type.CaptionId.ToString ());
				buffer.Append (@""" />");
			}
		}
	}
}
