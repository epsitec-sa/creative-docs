//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;



namespace Epsitec.Cresus.Core.UnitTests
{
	/// <summary>
	/// The <c>UnitTestFormState</c> unit test applies to <see cref="FormState"/>.
	/// </summary>
	[TestClass]
	public class UnitTestFormState
	{
		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}

		[TestMethod]
		public void Test01SaveDialogData()
		{
			var context = EntityContext.Current;
			var manager = new EntityPersistenceManager ();
			
			context.PersistenceManagers.Add (manager);

			var address = UnitTestFormState.CreateAddressEntity (context);
			var country = address.Location.Country;
			var location = UnitTestFormState.CreateLocationEntityForYverdon (context, country);
			
			manager.Map["loc1"] = address.Location;
			manager.Map["loc2"] = location;

			Assert.AreEqual ("Case postale 16", address.PostBox.Number);
			Assert.AreEqual ("1462", address.Location.PostalCode);
			Assert.AreEqual ("Yvonand", address.Location.Name);
			Assert.AreEqual ("Suisse", address.Location.Country.Name);
			
			var data = new DialogData<AddressEntity> (address, DialogDataMode.Isolated);
			var temp = data.Data;
			XElement element;

#if false
			//	No changes :
			element = States.FormState.SaveDialogData (data);
			Assert.AreEqual (@"<dialogData />", element.ToString (SaveOptions.DisableFormatting));

			//	Cascaded text change :
			temp.PostBox.Number = "Postfach 16";

			Assert.AreEqual ("Case postale 16", address.PostBox.Number);
			Assert.AreEqual ("Postfach 16", temp.PostBox.Number);
			
			element = States.FormState.SaveDialogData (data);
			Assert.AreEqual (@"<dialogData><data path=""[L0AH].[L0AG]"" value=""Postfach 16"" /></dialogData>", element.ToString (SaveOptions.DisableFormatting));

			//	Replace location with another one :
			temp.Location = location;

			Assert.AreEqual ("1462", address.Location.PostalCode);
			Assert.AreEqual ("1400", temp.Location.PostalCode);

			element = States.FormState.SaveDialogData (data);
			Assert.AreEqual (@"<dialogData><ref path=""[L0AE]"" id=""loc2"" /><data path=""[L0AH].[L0AG]"" value=""Postfach 16"" /></dialogData>", element.ToString (SaveOptions.DisableFormatting));

			data.RevertChanges ();

			Assert.AreEqual ("Case postale 16", temp.PostBox.Number);
			Assert.AreEqual ("1462", temp.Location.PostalCode);
			Assert.AreEqual ("Yvonand", temp.Location.Name);
			Assert.AreEqual ("Suisse", temp.Location.Country.Name);

			element = States.FormState.SaveDialogData (data);
			Assert.AreEqual (@"<dialogData />", element.ToString (SaveOptions.DisableFormatting));
#endif

			context.PersistenceManagers.Remove (manager);
		}


		[TestMethod]
		public void Test02RestoreDialogData()
		{
			var context = EntityContext.Current;
			var manager = new EntityPersistenceManager ();

			context.PersistenceManagers.Add (manager);

			var address = UnitTestFormState.CreateAddressEntity (context);
			var country = address.Location.Country;
			var location = UnitTestFormState.CreateLocationEntityForYverdon (context, country);

			manager.Map["loc1"] = address.Location;
			manager.Map["loc2"] = location;

			var data = new DialogData<AddressEntity> (address, DialogDataMode.Isolated);
			var temp = data.Data;

			//	Restore CasePostale (value) and Localité (reference)
			XElement element = XElement.Parse (@"<dialogData><ref path=""[L0AE]"" id=""loc2"" /><data path=""[L0AH].[L0AG]"" value=""Postfach 16"" /></dialogData>");

#if false
			States.FormState.RestoreDialogData (data, element);

			Assert.AreEqual ("Postfach 16", temp.PostBox.Number);
			Assert.AreEqual ("Yverdon-les-Bains", temp.Location.Name);
			Assert.AreEqual (location, temp.Location);
#endif

			context.PersistenceManagers.Remove (manager);
		}


		private static AddressEntity CreateAddressEntity(EntityContext context)
		{
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
			
			return address;
		}

		private static LocationEntity CreateLocationEntityForYverdon(EntityContext context, CountryEntity country)
		{
			var location = context.CreateEmptyEntity<LocationEntity> ();

			using (location.DefineOriginalValues ())
			{
				location.Country = country;
				location.PostalCode = "1400";
				location.Name = "Yverdon-les-Bains";
			}

			return location;
		}
	}
}
