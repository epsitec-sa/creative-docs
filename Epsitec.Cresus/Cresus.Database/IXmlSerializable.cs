//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// 
	/// </summary>
	public interface IXmlSerializable
	{
		void Serialize(System.Xml.XmlTextWriter xmlWriter);
	}
}
