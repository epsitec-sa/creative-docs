using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbTypeFactoryTest
	{
		[Test] public void CheckNewTypeBase()
		{
			DbType type = DbTypeFactory.NewType ("<type class='base'/>");
			
			Assertion.Assert (type.GetType () == typeof (DbType));
		}
		
		[Test] public void CheckNewTypeNum()
		{
			DbTypeNum type;
			
			type = DbTypeFactory.NewType ("<type class='num' type='Int16'/>") as DbTypeNum;
			
			Assertion.Assert (type.GetType () == typeof (DbTypeNum));
			Assertion.AssertEquals (DbRawType.Int16, type.NumDef.InternalRawType);
			
			type = DbTypeFactory.NewType ("<type class='num' digits='5' shift='2' min='0.00' max='200.00'/>") as DbTypeNum;
			Assertion.Assert (type.GetType () == typeof (DbTypeNum));
			Assertion.AssertEquals (DbRawType.Unsupported, type.NumDef.InternalRawType);
			Assertion.AssertEquals (5, type.NumDef.DigitPrecision);
			Assertion.AssertEquals (2, type.NumDef.DigitShift);
			Assertion.AssertEquals (  0.00M, type.NumDef.MinValue);
			Assertion.AssertEquals (200.00M, type.NumDef.MaxValue);
		}
		
		[Test] public void CheckNewTypeEnum()
		{
			DbTypeEnum type;
			
			type = DbTypeFactory.NewType ("<type class='enum' nmlen='5'/>") as DbTypeEnum;
			
			Assertion.Assert (type.GetType () == typeof (DbTypeEnum));
			Assertion.AssertEquals (5, type.MaxNameLength);
		}
		
		[Test] public void CheckNewTypeString()
		{
			DbTypeString type;
			
			type = DbTypeFactory.NewType ("<type class='str' length='100'/>") as DbTypeString;
			Assertion.Assert (type.GetType () == typeof (DbTypeString));
			Assertion.AssertEquals (100, type.Length);
			Assertion.AssertEquals (false, type.IsFixedLength);
			
			type = DbTypeFactory.NewType ("<type class='str' length='100' fixed='0'/>") as DbTypeString;
			Assertion.AssertEquals (false, type.IsFixedLength);
			
			type = DbTypeFactory.NewType ("<type class='str' length='100' fixed='1'/>") as DbTypeString;
			Assertion.AssertEquals (true, type.IsFixedLength);
		}
		
		[Test] [ExpectedException (typeof (System.FormatException))] public void CheckNewTypeXmlEx1()
		{
			DbType type = DbTypeFactory.NewType ("<foo><bar>x</bar></foo>");
		}
		
		[Test] [ExpectedException (typeof (System.FormatException))] public void CheckNewTypeXmlEx2()
		{
			DbType type = DbTypeFactory.NewType ("<type x='a'></type>");
		}
		
		[Test] public void CheckSerializeToXml()
		{
			DbType type;
			string xml;
			DbType temp;
			
			type = new DbType (DbSimpleType.Guid);
			xml  = DbTypeFactory.SerializeToXml (type, true);
			temp = DbTypeFactory.NewType (xml);
			
			System.Console.Out.WriteLine ("XML: {0}", xml);
			Assertion.Assert (temp.GetType () == type.GetType ());
			
			type = new DbTypeEnum ();
			xml  = DbTypeFactory.SerializeToXml (type, true);
			temp = DbTypeFactory.NewType (xml);
			
			System.Console.Out.WriteLine ("XML: {0}", xml);
			Assertion.Assert (temp.GetType () == type.GetType ());
			
			type = new DbTypeNum (DbNumDef.FromRawType (DbRawType.Int16));
			xml  = DbTypeFactory.SerializeToXml (type, true);
			temp = DbTypeFactory.NewType (xml);
			
			System.Console.Out.WriteLine ("XML: {0}", xml);
			Assertion.Assert (temp.GetType () == type.GetType ());
			
			type = new DbTypeString (100);
			xml  = DbTypeFactory.SerializeToXml (type, true);
			temp = DbTypeFactory.NewType (xml);
			
			System.Console.Out.WriteLine ("XML: {0}", xml);
			Assertion.Assert (temp.GetType () == type.GetType ());
		}
	}
}
