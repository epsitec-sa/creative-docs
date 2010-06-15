using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.Core
{


	[TestClass]
	public class UnitTestEntityConstrainer
	{


		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();
			Database.CreateAndConnectToDatabase ();
			Database2.PupulateDatabase (false);
		}


		[TestMethod]
		public void ValidEntityContainer()
		{
			TestHelper.PrintStartTest ("Valid Entity Container");

			NaturalPersonEntity person = new NaturalPersonEntity ();
			UriContactEntity uriContact = new UriContactEntity ();
			PersonTitleEntity title = new PersonTitleEntity ();

			person.Title = title;
			person.Contacts.Add (uriContact);

			EntityConstrainer entityConstrainer = new EntityConstrainer ();

			Assert.IsFalse (this.IsExceptionThrown (() =>
				entityConstrainer.AddLocalConstraint (person, new BinaryComparisonFieldWithField (new Field (new Druid ("[L0AU]")), BinaryComparator.IsEqual, new Field (new Druid ("[L0AV]"))))
			));

			Assert.IsFalse (this.IsExceptionThrown (() =>
				entityConstrainer.AddLocalConstraint (person, new BinaryComparisonFieldWithValue (new Field (new Druid ("[L0A11]")), BinaryComparator.IsEqual, new Constant (Type.Boolean, true)))
			));

			Assert.IsFalse (this.IsExceptionThrown (() =>
				entityConstrainer.AddLocalConstraint (uriContact, new UnaryComparison (new Field (new Druid ("[L0A92]")), UnaryComparator.IsNull))
			));

			Assert.IsFalse (this.IsExceptionThrown (() =>
				entityConstrainer.AddLocalConstraint (title, new BinaryOperation (new UnaryComparison (new Field (new Druid ("[L0AT1]")), UnaryComparator.IsNotNull), BinaryOperator.Or, new UnaryComparison (new Field (new Druid ("[L0AS1]")), UnaryComparator.IsNotNull)))
			));
		}


		[TestMethod]
		public void InvalidEntityContainer()
		{
			TestHelper.PrintStartTest ("Invalid Entity Container");

			NaturalPersonEntity person = new NaturalPersonEntity ();
			UriContactEntity uriContact = new UriContactEntity ();
			PersonTitleEntity title = new PersonTitleEntity ();

			person.Title = title;
			person.Contacts.Add (uriContact);

			EntityConstrainer entityConstrainer = new EntityConstrainer ();

			Assert.IsTrue (this.IsExceptionThrown (() =>
				entityConstrainer.AddLocalConstraint (person, new BinaryComparisonFieldWithField (new Field (new Druid ("[L0AS1]")), BinaryComparator.IsEqual, new Field (new Druid ("[L0AV]"))))
			));

			Assert.IsTrue (this.IsExceptionThrown (() =>
				entityConstrainer.AddLocalConstraint (person, new BinaryComparisonFieldWithValue (new Field (new Druid ("[L0AI]")), BinaryComparator.IsEqual, new Constant (Type.Boolean, true)))
			));

			Assert.IsTrue (this.IsExceptionThrown (() =>
				entityConstrainer.AddLocalConstraint (uriContact, new UnaryComparison (new Field (new Druid ("[L0A93]")), UnaryComparator.IsNull))
			));

			Assert.IsTrue (this.IsExceptionThrown (() =>
				entityConstrainer.AddLocalConstraint (title, new BinaryOperation (new UnaryComparison (new Field (new Druid ("[L0A61]")), UnaryComparator.IsNotNull), BinaryOperator.Or, new UnaryComparison (new Field (new Druid ("[L0AS1]")), UnaryComparator.IsNotNull)))
			));
		}


		private bool IsExceptionThrown(System.Action action)
		{
			try
			{
				action ();
				return false;
			}
			catch (System.Exception e)
			{
				return true;
			}
		}


	}


}
