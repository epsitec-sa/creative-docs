using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbTypeFactoryTest
	{
		[Test] public void CheckNewTypeBase()
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			
			doc.LoadXml ("<type class='base' />");
			DbType type = DbTypeFactory.NewType (doc.DocumentElement);
			
			Assertion.Assert (type.GetType () == typeof (DbType));
		}
		
		[Test] public void CheckNewTypeNum()
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			
			doc.LoadXml ("<type class='num' />");
			DbType type = DbTypeFactory.NewType (doc.DocumentElement);
			
			Assertion.Assert (type.GetType () == typeof (DbTypeNum));
		}
		
		[Test] public void CheckNewTypeEnum()
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			
			doc.LoadXml ("<type class='enum' />");
			DbType type = DbTypeFactory.NewType (doc.DocumentElement);
			
			Assertion.Assert (type.GetType () == typeof (DbTypeEnum));
		}
		
		[Test] public void CheckNewTypeString()
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			
			doc.LoadXml ("<type class='str' />");
			DbType type = DbTypeFactory.NewType (doc.DocumentElement);
			
			Assertion.Assert (type.GetType () == typeof (DbTypeString));
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckNewTypeXmlEx1()
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			doc.LoadXml ("<foo><bar>x</bar></foo>");
			DbType type = DbTypeFactory.NewType (doc.DocumentElement);
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckNewTypeXmlEx2()
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			doc.LoadXml ("<type x='a'></type>");
			DbType type = DbTypeFactory.NewType (doc.DocumentElement);
		}
	}
}
