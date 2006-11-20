//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>IXmlSerializable</c> interface can be used to serialize an instance
	/// to XML.
	/// </summary>
	public interface IXmlSerializable
	{
		/// <summary>
		/// Serializes the instance using the specified XML writer.
		/// </summary>
		/// <param name="xmlWriter">The XML writer.</param>
		void Serialize(System.Xml.XmlTextWriter xmlWriter);
	}
}
