using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbTypeFactoryTest
	{
		[Test] public void CheckNewTypeXml()
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			doc.LoadXml ("<type><bar>x</bar></type>");
			DbType type = DbTypeFactory.NewType (doc.DocumentElement);
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckNewTypeXmlEx1()
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			doc.LoadXml ("<foo><bar>x</bar></foo>");
			DbType type = DbTypeFactory.NewType (doc.DocumentElement);
		}
	}
}
