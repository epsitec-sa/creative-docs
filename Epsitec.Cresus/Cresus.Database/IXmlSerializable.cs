//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// 
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
