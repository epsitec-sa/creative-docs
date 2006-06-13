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
			DbEnumValue[] enumValues = Common.Types.Collection.ToArray (e_type.Values);
			DbEnumValue[] copyValues = Common.Types.Collection.ToArray (e_copy.Values);
			
			Assert.AreEqual ("M", enumValues[0].Name);
			Assert.AreEqual ("M", e_type[1].Name);
			Assert.AreEqual ("M", e_type["M"].Name);
			Assert.AreEqual (1, enumValues[0].Rank);
			Assert.AreEqual (1, e_type["M"].Rank);
			
			Assert.AreEqual ("MME", copyValues[1].Name);
			Assert.AreEqual ("MME", e_copy[2].Name);
			Assert.AreEqual ("MME", e_copy["MME"].Name);
			Assert.AreEqual (2, copyValues[1].Rank);
			Assert.AreEqual (2, e_copy["MME"].Rank);

			Assert.AreEqual ("MLLE", copyValues[2].Name);
			Assert.AreEqual ("MLLE", e_copy[3].Name);
			Assert.AreEqual ("MLLE", e_copy["MLLE"].Name);
			Assert.AreEqual (3, copyValues[2].Rank);
			Assert.AreEqual (3, e_copy["MLLE"].Rank);
			
			Assert.IsNull (e_type["X"]);
			
			Assert.AreEqual ("Monsieur", e_type["M"].Caption);
			Assert.AreEqual ("Madame", e_type["MME"].Caption);
			Assert.AreEqual ("Mademoiselle", e_type["MLLE"].Caption);
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
			
			Assert.AreEqual (@"<enumval rank=""2""/>", xml_p);
			Assert.AreEqual (@"<enumval rank=""2"" attr.capt=""Madame"" attr.name=""MME""/>", xml_f);
			
			xml_p = DbEnumValue.SerializeToXml (ev_2, false);
			xml_f = DbEnumValue.SerializeToXml (ev_2, true);
			
			Assert.AreEqual (@"<enumval rank=""3""/>", xml_p);
			Assert.AreEqual (@"<enumval rank=""3"" attr.capt=""&gt;Mademoiselle&lt;"" attr.name=""MLLE""/>", xml_f);
			
			string full_xml = DbTypeFactory.SerializeToXml (e_type, true);
			System.Console.Out.WriteLine ("XML: {0}", full_xml);
			
			DbEnumValue ev_1x = DbEnumValue.CreateEnumValue (DbEnumValue.SerializeToXml (ev_1, true));
			DbEnumValue ev_2x = DbEnumValue.CreateEnumValue (DbEnumValue.SerializeToXml (ev_2, true));
			DbEnumValue ev_3x = DbEnumValue.CreateEnumValue (DbEnumValue.SerializeToXml (ev_3, true));
			
			Assert.AreEqual (ev_1.Name,    ev_1x.Name);
			Assert.AreEqual (ev_1.Caption, ev_1x.Caption);
			Assert.AreEqual (ev_1.Rank,    ev_1x.Rank);
			
			Assert.AreEqual (ev_2.Name,    ev_2x.Name);
			Assert.AreEqual (ev_2.Caption, ev_2x.Caption);
			Assert.AreEqual (ev_2.Rank,    ev_2x.Rank);
			
			Assert.AreEqual (ev_3.Name,    ev_3x.Name);
			Assert.AreEqual (ev_3.Caption, ev_3x.Caption);
			Assert.AreEqual (ev_3.Rank,    ev_3x.Rank);
			
			DbTypeEnum copy = DbTypeFactory.CreateType (full_xml) as DbTypeEnum;
			
			Assert.IsNotNull (copy);
			Assert.AreEqual (3, copy.Count);
			Assert.AreEqual (ev_1.Name,    copy["MME"] .Name);
			Assert.AreEqual (ev_1.Rank,    copy["MME"] .Rank);
			Assert.AreEqual (ev_1.Caption, copy["MME"] .Caption);
			Assert.AreEqual (ev_2.Name,    copy["MLLE"].Name);
			Assert.AreEqual (ev_2.Rank,    copy["MLLE"].Rank);
			Assert.AreEqual (ev_2.Caption, copy["MLLE"].Caption);
			Assert.AreEqual (ev_3.Name,    copy["M"]   .Name);
			Assert.AreEqual (ev_3.Rank,    copy["M"]   .Rank);
			Assert.AreEqual (ev_3.Caption, copy["M"]   .Caption);

			DbEnumValue[] enumValues = Common.Types.Collection.ToArray (e_type.Values);
			DbEnumValue[] copyValues = Common.Types.Collection.ToArray (copy.Values);

			Assert.AreEqual (3, enumValues.Length);
			
			for (int i = 0; i < 3; i++)
			{
				Assert.AreEqual (enumValues[i].Name, copyValues[i].Name);
				Assert.AreEqual (enumValues[i].Rank, copyValues[i].Rank);
				Assert.AreEqual (enumValues[i].Caption, copyValues[i].Caption);
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
			DbKey k1 = new DbKey (1, DbRowStatus.ArchiveCopy);
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
				Assert.AreEqual (k0, DbKey.DeserializeFromXmlAttributes (xml));
			}
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				buffer.Append ("<test");
				DbKey.SerializeToXmlAttributes (buffer, k1);
				buffer.Append ("/>");
				System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
				doc.LoadXml (buffer.ToString ());
				System.Xml.XmlElement  xml = doc.DocumentElement;
				Assert.AreEqual (k1, DbKey.DeserializeFromXmlAttributes (xml));
			}
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				buffer.Append ("<test");
				DbKey.SerializeToXmlAttributes (buffer, k2);
				buffer.Append ("/>");
				System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
				doc.LoadXml (buffer.ToString ());
				System.Xml.XmlElement  xml = doc.DocumentElement;
				Assert.AreEqual (k2, DbKey.DeserializeFromXmlAttributes (xml));
			}
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				buffer.Append ("<test");
				DbKey.SerializeToXmlAttributes (buffer, k3);
				buffer.Append ("/>");
				System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
				doc.LoadXml (buffer.ToString ());
				System.Xml.XmlElement  xml = doc.DocumentElement;
				Assert.AreEqual (k3, DbKey.DeserializeFromXmlAttributes (xml));
			}
		}
	}
}
