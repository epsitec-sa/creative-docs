//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Text;
using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class UnitTestFormState
	{
		public UnitTestFormState()
		{
			ResourceManagerPool.Default = new ResourceManagerPool ("default");
			ResourceManagerPool.Default.AddResourceProbingPath (@"S:\Epsitec.Cresus\Cresus.Core");
			Epsitec.Cresus.Core.States.StateFactory.Setup ();
		}


		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void TestMethod1()
		{
			EntityContext      context = EntityContext.Current;
			PersistenceManager manager = new PersistenceManager ();

			AddressEntity  address  = context.CreateEmptyEntity<AddressEntity> ();
			LocationEntity location = context.CreateEmptyEntity<LocationEntity> ();
			PostBoxEntity  postBox  = context.CreateEmptyEntity<PostBoxEntity> ();
			StreetEntity   street   = context.CreateEmptyEntity<StreetEntity> ();
			CountryEntity  country  = context.CreateEmptyEntity<CountryEntity> ();

			using (address.DefineOriginalValues ())
			{
				address.Location = location;
				address.PostBox = postBox;
				address.Street = street;
			}

			using (location.DefineOriginalValues ())
			{
				location.Country = country;
				location.PostalCode = "1462";
				location.Name = "Yvonand";
			}

			using (postBox.DefineOriginalValues ())
			{
				postBox.Number = "Case postale 16";
			}

			using (street.DefineOriginalValues ())
			{
				street.Complement = "Franc Castel";
				street.StreetName = "Rue du Lac 5";
			}

			using (country.DefineOriginalValues ())
			{
				country.Code = "CH";
				country.Name = "Suisse";
			}

			context.PersistenceManagers.Add (manager);

#if false
			AdresseEntity   adr  = context.CreateEmptyEntity<AdresseEntity> ();
			Localité1Entity loc1 = context.CreateEmptyEntity<Localité1Entity> ();
			Localité1Entity loc2 = context.CreateEmptyEntity<Localité1Entity> ();

			manager.Map["loc1"] = loc1;
			manager.Map["loc2"] = loc2;

			context.PersistenceManagers.Add (manager);

			using (adr.DefineOriginalValues ())
			{
				adr.CasePostale = "Case postale 16";
				adr.Rue = "Rue du Lac 6";
				adr.Localité = loc1;
			}

			using (loc1.DefineOriginalValues ())
			{
				loc1.Nom = "Yvonand";
				loc1.Numéro = "1462";
			}

			using (loc2.DefineOriginalValues ())
			{
				loc2.Nom = "Yverdon-les-Bains";
				loc2.Numéro = "1400";
			}

			DialogData    data = new DialogData (adr, DialogDataMode.Isolated);
			AdresseEntity temp = data.Data as AdresseEntity;
			XElement element;

			//	No changes :
			element = States.FormState.SaveDialogData (data);
			Assert.AreEqual (@"<dialogData />", element.ToString (SaveOptions.DisableFormatting));

			//	Single text change :
			temp.CasePostale = "CP 16";
			element = States.FormState.SaveDialogData (data);
			Assert.AreEqual (@"<dialogData><data path=""[8V14]"" value=""CP 16"" /></dialogData>", element.ToString (SaveOptions.DisableFormatting));

			//	Cascaded text change :
			temp.Localité.Numéro = "CH-1462";
			element = States.FormState.SaveDialogData (data);
			Assert.AreEqual (@"<dialogData><data path=""[8V14]"" value=""CP 16"" /><data path=""[8V15].[8V19]"" value=""CH-1462"" /></dialogData>", element.ToString (SaveOptions.DisableFormatting));

			temp.Localité = loc2;
			element = States.FormState.SaveDialogData (data);
			Assert.AreEqual (@"<dialogData><data path=""[8V14]"" value=""CP 16"" /><ref path=""[8V15]"" id=""loc2"" /></dialogData>", element.ToString (SaveOptions.DisableFormatting));

			data.RevertChanges ();
			element = States.FormState.SaveDialogData (data);
			Assert.AreEqual (@"<dialogData />", element.ToString (SaveOptions.DisableFormatting));
#endif

			context.PersistenceManagers.Remove (manager);
		}
		#region PersistenceManager Class

		/// <summary>
		/// The <c>PersistenceManager</c> class implements a simple persistence
		/// manager which associates entities with ids through a dictionary.
		/// </summary>
		private class PersistenceManager : IEntityPersistenceManager
		{
			#region IEntityPersistenceManager Members

			public string GetPersistedId(AbstractEntity entity)
			{
				foreach (var item in this.Map)
				{
					if (item.Value == entity)
					{
						return item.Key;
					}
				}

				return null;
			}

			public AbstractEntity GetPeristedEntity(string id)
			{
				AbstractEntity entity;

				if (this.Map.TryGetValue (id, out entity))
				{
					return entity;
				}
				else
				{
					return null;
				}
			}

			#endregion

			public readonly Dictionary<string, AbstractEntity> Map = new Dictionary<string, AbstractEntity> ();
		}

		#endregion
	}
}
