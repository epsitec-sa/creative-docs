using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbTypeTest
	{
		[Test] public void CheckDbTypeEnum()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			list.Add (new DbEnumValue ("id=2", "name=MME", "capt=Madame"));
			list.Add (new DbEnumValue ("id=3", "name=MLLE", "capt=Mademoiselle"));
			list.Add (new DbEnumValue ("id=1", "name=M", "capt=Monsieur"));
			
			DbTypeEnum e_type = new DbTypeEnum (list);
			DbTypeEnum e_copy = e_type.Clone () as DbTypeEnum;
			
			Assertion.AssertEquals ("1", e_type[0].Id);
			Assertion.AssertEquals ("M", e_type[0].Name);
			Assertion.AssertEquals ("1", e_type["M"].Id);
			Assertion.AssertEquals ("M", e_type["M"].Name);
			
			Assertion.AssertEquals ("2",   e_copy[1].Id);
			Assertion.AssertEquals ("MME", e_copy[1].Name);
			Assertion.AssertEquals ("2",   e_copy["MME"].Id);
			Assertion.AssertEquals ("MME", e_copy["MME"].Name);
			
			Assertion.AssertNull (e_type["X"]);
			
			Assertion.AssertEquals ("Monsieur", e_type["M"].Caption);
			Assertion.AssertEquals ("Madame", e_type["MME"].Caption);
			Assertion.AssertEquals ("Mademoiselle", e_type["MLLE"].Caption);
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckDbTypeEnumEx1()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			list.Add (new DbEnumValue ("id=2", "name=A"));
			list.Add (new DbEnumValue ("id=3", "name=B"));
			list.Add (new DbEnumValue ("id=1", "name=A"));
			
			DbTypeEnum e_type = new DbTypeEnum (list);
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckDbTypeEnumEx2()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			list.Add (new DbEnumValue ("id=1", "name=A"));
			list.Add (new DbEnumValue ("id=2", "name=B"));
			list.Add (new DbEnumValue ("id=1", "name=C"));
			
			DbTypeEnum e_type = new DbTypeEnum (list);
		}
	}
}
