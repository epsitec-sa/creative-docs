using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.UnitTests.Entities;

using Epsitec.Common.Types;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Common.Support.UnitTests
{


	[TestClass]
	public sealed class UnitTestEntityModifications
	{

		
		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void HasCollectionChangedTest1()
		{
			AbstractContactEntity contact1 = new AbstractContactEntity ();
			AbstractContactEntity contact2 = new AbstractContactEntity ();
			AbstractContactEntity contact3 = new AbstractContactEntity ();
			
			NaturalPersonEntity person = new NaturalPersonEntity ();
			Assert.IsFalse (person.HasValueChanged (Druid.Parse ("[L0AS]")));

			using (person.DefineOriginalValues ())
			{
				person.Contacts.Add (contact1);
				person.Contacts.Add (contact2);
			}
			Assert.IsFalse (person.HasCollectionChanged (Druid.Parse ("[L0AS]")));

			person.Contacts.Add (contact3);
			Assert.IsTrue (person.HasCollectionChanged (Druid.Parse ("[L0AS]")));

			person.Contacts.Remove (contact3);
			Assert.IsFalse (person.HasCollectionChanged (Druid.Parse ("[L0AS]")));

			person.Contacts.Remove (contact2);
			Assert.IsTrue (person.HasCollectionChanged (Druid.Parse ("[L0AS]")));

			person.Contacts.Add (contact2);
			Assert.IsFalse (person.HasCollectionChanged (Druid.Parse ("[L0AS]")));
		}


		[TestMethod]
		public void HasCollectionChangedTest2()
		{
			AbstractContactEntity contact = new AbstractContactEntity ();

			NaturalPersonEntity person = new NaturalPersonEntity ();
			Assert.IsFalse (person.HasValueChanged (Druid.Parse ("[L0AS]")));

			person.Contacts.Add (contact);
			Assert.IsTrue (person.HasCollectionChanged (Druid.Parse ("[L0AS]")));

			person.Contacts.Remove (contact);
			Assert.IsFalse (person.HasCollectionChanged (Druid.Parse ("[L0AS]")));
		}


		[TestMethod]
		public void HasCollectionChangedTest3()
		{
			AbstractContactEntity contact1 = new AbstractContactEntity ();
			AbstractContactEntity contact2 = new AbstractContactEntity ();

			NaturalPersonEntity person = new NaturalPersonEntity ();
			;
			Assert.IsFalse (person.HasCollectionChanged (Druid.Parse ("[L0AS]")));

			using (person.DefineOriginalValues ())
			{
				person.Contacts.Add (contact1);
				person.Contacts.Add (contact2);
			}
			Assert.IsFalse (person.HasCollectionChanged (Druid.Parse ("[L0AS]")));

			person.Contacts[0] = contact2;
			person.Contacts[1] = contact1;
			Assert.IsTrue (person.HasCollectionChanged (Druid.Parse ("[L0AS]")));

			person.Contacts[0] = contact1;
			person.Contacts[1] = contact2;
			Assert.IsFalse (person.HasCollectionChanged (Druid.Parse ("[L0AS]")));
		}


		[TestMethod]
		public void HasCollectionChangedTest4()
		{
			AbstractContactEntity contact1 = new AbstractContactEntity ();
			AbstractContactEntity contact2 = new AbstractContactEntity ();

			NaturalPersonEntity person = new NaturalPersonEntity ();
			Assert.IsFalse (person.HasCollectionChanged (Druid.Parse ("[L0AS]")));

			person.Contacts.Add (contact1);
			person.Contacts.Add (contact2);
			Assert.IsTrue (person.HasCollectionChanged (Druid.Parse ("[L0AS]")));

			person.Contacts[0] = contact2;
			person.Contacts[1] = contact1;
			Assert.IsTrue (person.HasCollectionChanged (Druid.Parse ("[L0AS]")));

			person.Contacts[0] = contact1;
			person.Contacts[1] = contact2;
			Assert.IsTrue (person.HasCollectionChanged (Druid.Parse ("[L0AS]")));

			person.Contacts.Remove (contact1);
			person.Contacts.Remove (contact2);
			Assert.IsFalse (person.HasCollectionChanged (Druid.Parse ("[L0AS]")));
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void HasCollectionChangedTest5()
		{
			NaturalPersonEntity person = null;

			person.HasCollectionChanged (Druid.Parse ("[L0AS]"));
		}


		[TestMethod]
		public void HasReferenceChangedTest1()
		{
			PersonGenderEntity gender1 = new PersonGenderEntity ();
			PersonGenderEntity gender2 = new PersonGenderEntity ();

			NaturalPersonEntity person = new NaturalPersonEntity ();
			Assert.IsFalse (person.HasValueChanged (Druid.Parse ("[L0A11]")));

			using (person.DefineOriginalValues ())
			{
				person.Gender = gender1;
			}
			Assert.IsFalse (person.HasReferenceChanged (Druid.Parse ("[L0A11]")));

			person.Gender = gender2;
			Assert.IsTrue (person.HasReferenceChanged (Druid.Parse ("[L0A11]")));

			person.Gender = gender1;
			Assert.IsFalse (person.HasReferenceChanged (Druid.Parse ("[L0A11]")));
		}


		[TestMethod]
		public void HasReferenceChangedTest2()
		{
			PersonGenderEntity gender1 = new PersonGenderEntity ();
			PersonGenderEntity gender2 = new PersonGenderEntity ();

			NaturalPersonEntity person = new NaturalPersonEntity ();
			Assert.IsFalse (person.HasReferenceChanged (Druid.Parse ("[L0A11]")));

			person.Gender = gender1;
			Assert.IsTrue (person.HasReferenceChanged (Druid.Parse ("[L0A11]")));

			person.Gender = gender2;
			Assert.IsTrue (person.HasReferenceChanged (Druid.Parse ("[L0A11]")));

			person.Gender = gender1;
			Assert.IsTrue (person.HasReferenceChanged (Druid.Parse ("[L0A11]")));
		}


		[TestMethod]
		public void HasReferenceChangedTest3()
		{
			PersonGenderEntity gender1 = new PersonGenderEntity ();
			PersonGenderEntity gender2 = new PersonGenderEntity ();

			NaturalPersonEntity person = new NaturalPersonEntity ();
			using (person.DefineOriginalValues ())
			{
				person.Gender = gender1;
			}
			Assert.IsFalse (person.HasReferenceChanged (Druid.Parse ("[L0A11]")));

			person.Gender = gender2;
			Assert.IsTrue (person.HasReferenceChanged (Druid.Parse ("[L0A11]")));

			person.Gender = gender1;
			Assert.IsFalse (person.HasReferenceChanged (Druid.Parse ("[L0A11]")));

			person.Gender = null;
			Assert.IsTrue (person.HasReferenceChanged (Druid.Parse ("[L0A11]")));
		}


		[TestMethod]
		public void HasReferenceChangedTest4()
		{
			PersonGenderEntity gender1 = new PersonGenderEntity ();

			NaturalPersonEntity person = new NaturalPersonEntity ();
			Assert.IsFalse (person.HasReferenceChanged (Druid.Parse ("[L0A11]")));

			person.Gender = gender1;
			Assert.IsTrue (person.HasReferenceChanged (Druid.Parse ("[L0A11]")));

			person.Gender = null;
			Assert.IsFalse (person.HasReferenceChanged (Druid.Parse ("[L0A11]")));
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void HasReferenceChangedTest5()
		{
			NaturalPersonEntity person = null;

			person.HasReferenceChanged (Druid.Parse ("[L0A11]"));
		}


		[TestMethod]
		public void HasValueChangedTest1()
		{
			NaturalPersonEntity person = new NaturalPersonEntity ();
			Assert.IsFalse (person.HasValueChanged (Druid.Parse ("[L0AV]")));

			using (person.DefineOriginalValues ())
			{
				person.Firstname = "Dupond";
			}
			Assert.IsFalse (person.HasValueChanged (Druid.Parse ("[L0AV]")));

			person.Firstname = "De-La-Motte";
			Assert.IsTrue (person.HasValueChanged (Druid.Parse ("[L0AV]")));

			person.Firstname = "Dupond";
			Assert.IsFalse (person.HasValueChanged (Druid.Parse ("[L0AV]")));
		}


		[TestMethod]
		public void HasValueChangedTest2()
		{
			NaturalPersonEntity person = new NaturalPersonEntity ();
			Assert.IsFalse (person.HasValueChanged (Druid.Parse ("[L0AV]")));

			person.Firstname = "De-La-Motte";
			Assert.IsTrue (person.HasValueChanged (Druid.Parse ("[L0AV]")));

			person.Firstname = "Dupond";
			Assert.IsTrue (person.HasValueChanged (Druid.Parse ("[L0AV]")));

			person.Firstname = "De-La-Motte";
			Assert.IsTrue (person.HasValueChanged (Druid.Parse ("[L0AV]")));
		}


		[TestMethod]
		public void HasValueChangedTest3()
		{
			NaturalPersonEntity person = new NaturalPersonEntity ();
			using (person.DefineOriginalValues ())
			{
				person.BirthDate = new Date (1950, 12, 12);
			}
			Assert.IsFalse (person.HasValueChanged (Druid.Parse ("[L0A61]")));

			person.BirthDate = new Date (1951, 12, 12);
			Assert.IsTrue (person.HasValueChanged (Druid.Parse ("[L0A61]")));

			person.BirthDate = new Date (1950, 12, 12);
			Assert.IsFalse (person.HasValueChanged (Druid.Parse ("[L0A61]")));

			person.BirthDate = null;
			Assert.IsTrue (person.HasValueChanged (Druid.Parse ("[L0A61]")));
		}


		[TestMethod]
		public void HasValueChangedTest4()
		{
			NaturalPersonEntity person = new NaturalPersonEntity ();
			Assert.IsFalse (person.HasValueChanged (Druid.Parse ("[L0A61]")));

			person.BirthDate = new Date (1951, 12, 12);
			Assert.IsTrue (person.HasValueChanged (Druid.Parse ("[L0A61]")));

			person.BirthDate = null;
			Assert.IsFalse (person.HasValueChanged (Druid.Parse ("[L0A61]")));
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void HasValueChangedTest5()
		{
			NaturalPersonEntity person = null;

			person.HasValueChanged (Druid.Parse ("[L0AV]"));
		}


	}


}
