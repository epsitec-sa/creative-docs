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
			DbEnumValue ev_2 = new DbEnumValue (3, "MLLE");	ev_2.DefineAttributes ("capt=Mademoiselle");
			DbEnumValue ev_3 = new DbEnumValue (1, "M");	ev_3.DefineAttributes ("capt=Monsieur");
			
			list.Add (ev_1);
			list.Add (ev_2);
			list.Add (ev_3);
			
			DbTypeEnum e_type = new DbTypeEnum (list);
			DbTypeEnum e_copy = e_type.Clone () as DbTypeEnum;
			
			Assertion.AssertEquals ("M", e_type[0].Name);
			Assertion.AssertEquals ("M", e_type["M"].Name);
			Assertion.AssertEquals (1, e_type[0].Rank);
			Assertion.AssertEquals (1, e_type["M"].Rank);
			
			Assertion.AssertEquals ("MME", e_copy[1].Name);
			Assertion.AssertEquals ("MME", e_copy["MME"].Name);
			Assertion.AssertEquals (2, e_copy[1].Rank);
			Assertion.AssertEquals (2, e_copy["MME"].Rank);
			
			Assertion.AssertNull (e_type["X"]);
			
			Assertion.AssertEquals ("Monsieur", e_type["M"].Caption);
			Assertion.AssertEquals ("Madame", e_type["MME"].Caption);
			Assertion.AssertEquals ("Mademoiselle", e_type["MLLE"].Caption);
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckDbTypeEnumEx1()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			list.Add (new DbEnumValue ("name=A"));
			list.Add (new DbEnumValue ("name=B"));
			list.Add (new DbEnumValue ("name=A"));
			
			DbTypeEnum e_type = new DbTypeEnum (list);
		}
	}
}
