using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbTypeFactoryTest
	{
#if false
		[Test] public void CheckNewTypeBase()
		{
			DbType type = DbTypeFactory.CreateType ("<type class='base'/>");
			
			Assert.IsTrue (type.GetType () == typeof (DbType));
		}
		
		[Test] public void CheckNewTypeNum()
		{
			DbTypeNum type;
			
			type = DbTypeFactory.CreateType ("<type class='num' type='Int16'/>") as DbTypeNum;
			
			Assert.IsTrue (type.GetType () == typeof (DbTypeNum));
			Assert.AreEqual (DbRawType.Int16, type.NumDef.InternalRawType);
			
			type = DbTypeFactory.CreateType ("<type class='num' digits='5' shift='2' min='0.00' max='200.00'/>") as DbTypeNum;
			Assert.IsTrue (type.GetType () == typeof (DbTypeNum));
			Assert.AreEqual (DbRawType.Unsupported, type.NumDef.InternalRawType);
			Assert.AreEqual (5, type.NumDef.DigitPrecision);
			Assert.AreEqual (2, type.NumDef.DigitShift);
			Assert.AreEqual (  0.00M, type.NumDef.MinValue);
			Assert.AreEqual (200.00M, type.NumDef.MaxValue);
		}
		
		[Test] public void CheckNewTypeEnum()
		{
			DbTypeEnum type;
			
			type = DbTypeFactory.CreateType ("<type class='enum' nmlen='5'/>") as DbTypeEnum;
			
			Assert.IsTrue (type.GetType () == typeof (DbTypeEnum));
			Assert.AreEqual (5, type.MaxNameLength);
		}
		
		[Test] public void CheckNewTypeString()
		{
			DbTypeString type;
			
			type = DbTypeFactory.CreateType ("<type class='str' length='100'/>") as DbTypeString;
			Assert.IsTrue (type.GetType () == typeof (DbTypeString));
			Assert.AreEqual (100, type.MaximumLength);
			Assert.AreEqual (false, type.IsFixedLength);
			
			type = DbTypeFactory.CreateType ("<type class='str' length='100' fixed='0'/>") as DbTypeString;
			Assert.AreEqual (false, type.IsFixedLength);
			
			type = DbTypeFactory.CreateType ("<type class='str' length='100' fixed='1'/>") as DbTypeString;
			Assert.AreEqual (true, type.IsFixedLength);
		}
		
		[Test] [ExpectedException (typeof (System.FormatException))] public void CheckNewTypeXmlEx1()
		{
			DbType type = DbTypeFactory.CreateType ("<foo><bar>x</bar></foo>");
		}
		
		[Test] [ExpectedException (typeof (System.FormatException))] public void CheckNewTypeXmlEx2()
		{
			DbType type = DbTypeFactory.CreateType ("<type x='a'></type>");
		}
		
		[Test] public void CheckSerializeToXml()
		{
			DbType type;
			string xml;
			DbType temp;
			
			type = new DbType (DbSimpleType.Guid);
			xml  = DbTypeFactory.SerializeToXml (type, true);
			temp = DbTypeFactory.CreateType (xml);
			
			System.Console.Out.WriteLine ("XML: {0}", xml);
			Assert.IsTrue (temp.GetType () == type.GetType ());
			
			type = new DbTypeEnum ();
			xml  = DbTypeFactory.SerializeToXml (type, true);
			temp = DbTypeFactory.CreateType (xml);
			
			System.Console.Out.WriteLine ("XML: {0}", xml);
			Assert.IsTrue (temp.GetType () == type.GetType ());
			
			type = new DbTypeNum (DbNumDef.FromRawType (DbRawType.Int16));
			xml  = DbTypeFactory.SerializeToXml (type, true);
			temp = DbTypeFactory.CreateType (xml);
			
			System.Console.Out.WriteLine ("XML: {0}", xml);
			Assert.IsTrue (temp.GetType () == type.GetType ());
			
			type = new DbTypeString (100);
			xml  = DbTypeFactory.SerializeToXml (type, true);
			temp = DbTypeFactory.CreateType (xml);
			
			System.Console.Out.WriteLine ("XML: {0}", xml);
			Assert.IsTrue (temp.GetType () == type.GetType ());
			
			type = new DbTypeByteArray ();
			xml  = DbTypeFactory.SerializeToXml (type, true);
			temp = DbTypeFactory.CreateType (xml);
			
			System.Console.Out.WriteLine ("XML: {0}", xml);
			Assert.IsTrue (temp.GetType () == type.GetType ());
		}
#endif
	}
}
