//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


namespace Epsitec.Cresus.Core
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
			//	See http://geekswithblogs.net/sdorman/archive/2009/01/31/migrating-from-nunit-to-mstest.aspx
			//	for migration tips, from nUnit to MSTest.

			ResourceManagerPool.Default = new ResourceManagerPool ("default");
			ResourceManagerPool.Default.AddResourceProbingPath (@"S:\Epsitec.Cresus\Cresus.Core");

			Epsitec.Cresus.Core.States.StateFactory.Setup ();
		}

		public UnitTestFormState()
		{
		}


		[TestMethod]
		public void Test01SaveDialogData()
		{
			var context = EntityContext.Current;
			var manager = new EntityPersistenceManager ();

			AddressEntity address = GetSampleAddressEntity (context);
			Assert.IsNotNull (address);

			context.PersistenceManagers.Add (manager);
			
			var data = new DialogData<AddressEntity> (address, DialogDataMode.Isolated);
			var temp = data.Data;
			XElement element;

			//	No changes :
			element = States.FormState.SaveDialogData (data);
			Assert.AreEqual (@"<dialogData />", element.ToString (SaveOptions.DisableFormatting));

#if false
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


		[TestMethod]
		public void Test02RestoreDialogData()
		{
		}


		private static AddressEntity GetSampleAddressEntity(EntityContext context)
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

		#region PersistenceManager Class

		/// <summary>
		/// The <c>PersistenceManager</c> class implements a simple persistence
		/// manager which associates entities with ids through a dictionary.
		/// </summary>
		private class EntityPersistenceManager : IEntityPersistenceManager
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
