using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbTypeTest
	{
		[Test] public void CheckDbTypeEnum()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			DbEnumValue ev_1 = new DbEnumValue (2, "MME");	ev_1.DefineAttributes ("capt=Madame");
			DbEnumValue ev_2 = new DbEnumValue (3, "MLLE");	ev_2.DefineAttributes ("capt='Mademoiselle'");
			DbEnumValue ev_3 = new DbEnumValue (1, "M");	ev_3.DefineAttributes (@"capt=""Monsieur""");
			
			list.Add (ev_1);
			list.Add (ev_2);
			list.Add (ev_3);
			
			DbTypeEnum e_type = new DbTypeEnum (list);
			DbTypeEnum e_copy = e_type.Clone () as DbTypeEnum;
			
			Assertion.AssertEquals ("M", e_type.Values[0].Name);
			Assertion.AssertEquals ("M", e_type[1].Name);
			Assertion.AssertEquals ("M", e_type["M"].Name);
			Assertion.AssertEquals (1, e_type.Values[0].Rank);
			Assertion.AssertEquals (1, e_type["M"].Rank);
			
			Assertion.AssertEquals ("MME", e_copy.Values[1].Name);
			Assertion.AssertEquals ("MME", e_copy[2].Name);
			Assertion.AssertEquals ("MME", e_copy["MME"].Name);
			Assertion.AssertEquals (2, e_copy.Values[1].Rank);
			Assertion.AssertEquals (2, e_copy["MME"].Rank);
			
			Assertion.AssertEquals ("MLLE", e_copy.Values[2].Name);
			Assertion.AssertEquals ("MLLE", e_copy[3].Name);
			Assertion.AssertEquals ("MLLE", e_copy["MLLE"].Name);
			Assertion.AssertEquals (3, e_copy.Values[2].Rank);
			Assertion.AssertEquals (3, e_copy["MLLE"].Rank);
			
			Assertion.AssertNull (e_type["X"]);
			
			Assertion.AssertEquals ("Monsieur", e_type["M"].Caption);
			Assertion.AssertEquals ("Madame", e_type["MME"].Caption);
			Assertion.AssertEquals ("Mademoiselle", e_type["MLLE"].Caption);
		}
		
		[Test] public void CheckDbTypeEnumXml()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			DbEnumValue ev_1 = new DbEnumValue (2, "MME");	ev_1.DefineAttributes ("capt=Madame");
			DbEnumValue ev_2 = new DbEnumValue (3, "MLLE");	ev_2.DefineAttributes ("capt=>Mademoiselle<");
			DbEnumValue ev_3 = new DbEnumValue (1, "M");	ev_3.DefineAttributes ("capt=Monsieur");
			
			list.Add (ev_1);
			list.Add (ev_2);
			list.Add (ev_3);
			
			DbTypeEnum e_type = new DbTypeEnum (list);
			
			string xml_p = DbEnumValue.SerializeToXml (ev_1, false);
			string xml_f = DbEnumValue.SerializeToXml (ev_1, true);
			
			Assertion.AssertEquals (@"<enumval rank=""2""/>", xml_p);
			Assertion.AssertEquals (@"<enumval rank=""2"" attr.capt=""Madame"" attr.name=""MME""/>", xml_f);
			
			xml_p = DbEnumValue.SerializeToXml (ev_2, false);
			xml_f = DbEnumValue.SerializeToXml (ev_2, true);
			
			Assertion.AssertEquals (@"<enumval rank=""3""/>", xml_p);
			Assertion.AssertEquals (@"<enumval rank=""3"" attr.capt=""&gt;Mademoiselle&lt;"" attr.name=""MLLE""/>", xml_f);
			
			string full_xml = DbTypeFactory.SerializeToXml (e_type, true);
			System.Console.Out.WriteLine ("XML: {0}", full_xml);
			
			DbEnumValue ev_1x = DbEnumValue.CreateEnumValue (DbEnumValue.SerializeToXml (ev_1, true));
			DbEnumValue ev_2x = DbEnumValue.CreateEnumValue (DbEnumValue.SerializeToXml (ev_2, true));
			DbEnumValue ev_3x = DbEnumValue.CreateEnumValue (DbEnumValue.SerializeToXml (ev_3, true));
			
			Assertion.AssertEquals (ev_1.Name,    ev_1x.Name);
			Assertion.AssertEquals (ev_1.Caption, ev_1x.Caption);
			Assertion.AssertEquals (ev_1.Rank,    ev_1x.Rank);
			
			Assertion.AssertEquals (ev_2.Name,    ev_2x.Name);
			Assertion.AssertEquals (ev_2.Caption, ev_2x.Caption);
			Assertion.AssertEquals (ev_2.Rank,    ev_2x.Rank);
			
			Assertion.AssertEquals (ev_3.Name,    ev_3x.Name);
			Assertion.AssertEquals (ev_3.Caption, ev_3x.Caption);
			Assertion.AssertEquals (ev_3.Rank,    ev_3x.Rank);
			
			DbTypeEnum copy = DbTypeFactory.CreateType (full_xml) as DbTypeEnum;
			
			Assertion.AssertNotNull (copy);
			Assertion.AssertEquals (3, copy.Count);
			Assertion.AssertEquals (ev_1.Name,    copy["MME"] .Name);
			Assertion.AssertEquals (ev_1.Rank,    copy["MME"] .Rank);
			Assertion.AssertEquals (ev_1.Caption, copy["MME"] .Caption);
			Assertion.AssertEquals (ev_2.Name,    copy["MLLE"].Name);
			Assertion.AssertEquals (ev_2.Rank,    copy["MLLE"].Rank);
			Assertion.AssertEquals (ev_2.Caption, copy["MLLE"].Caption);
			Assertion.AssertEquals (ev_3.Name,    copy["M"]   .Name);
			Assertion.AssertEquals (ev_3.Rank,    copy["M"]   .Rank);
			Assertion.AssertEquals (ev_3.Caption, copy["M"]   .Caption);
			
			for (int i = 0; i < 3; i++)
			{
				Assertion.AssertEquals (e_type.Values[i].Name,    copy.Values[i].Name);
				Assertion.AssertEquals (e_type.Values[i].Rank,    copy.Values[i].Rank);
				Assertion.AssertEquals (e_type.Values[i].Caption, copy.Values[i].Caption);
			}
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckDbTypeEnumEx1()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			list.Add (new DbEnumValue ("name=A"));
			list.Add (new DbEnumValue ("name=B"));
			list.Add (new DbEnumValue ("name=A"));
			
			DbTypeEnum e_type = new DbTypeEnum (list);
		}
		
		[Test] public void CheckDbKeyXml()
		{
			DbKey k0 = null;
			DbKey k1 = new DbKey (1, 2, 3);
			DbKey k2 = new DbKey ();
			DbKey k3 = new DbKey (10);
			
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				buffer.Append ("<test");
				DbKey.SerializeToXmlAttributes (buffer, k0);
				buffer.Append ("/>");
				System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
				doc.LoadXml (buffer.ToString ());
				System.Xml.XmlElement  xml = doc.DocumentElement;
				Assertion.AssertEquals (k0, DbKey.DeserializeFromXmlAttributes (xml));
			}
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				buffer.Append ("<test");
				DbKey.SerializeToXmlAttributes (buffer, k1);
				buffer.Append ("/>");
				System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
				doc.LoadXml (buffer.ToString ());
				System.Xml.XmlElement  xml = doc.DocumentElement;
				Assertion.AssertEquals (k1, DbKey.DeserializeFromXmlAttributes (xml));
			}
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				buffer.Append ("<test");
				DbKey.SerializeToXmlAttributes (buffer, k2);
				buffer.Append ("/>");
				System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
				doc.LoadXml (buffer.ToString ());
				System.Xml.XmlElement  xml = doc.DocumentElement;
				Assertion.AssertEquals (k2, DbKey.DeserializeFromXmlAttributes (xml));
			}
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				buffer.Append ("<test");
				DbKey.SerializeToXmlAttributes (buffer, k3);
				buffer.Append ("/>");
				System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
				doc.LoadXml (buffer.ToString ());
				System.Xml.XmlElement  xml = doc.DocumentElement;
				Assertion.AssertEquals (k3, DbKey.DeserializeFromXmlAttributes (xml));
			}
		}
	}
}
