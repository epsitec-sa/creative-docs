//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.AddressBook.Entities;
using Epsitec.Cresus.Core;

using System.Collections.Generic;
using System.Xml.Linq;

using NUnit.Framework;
using Epsitec.Common.Dialogs;

namespace Epsitec.Cresus.Core
{
	[TestFixture]
	public class FormStateTest
	{
		[TestFixtureSetUp]
		public void Initialize()
		{
//-			Epsitec.Cresus.Core.States.StateFactory.Setup ();
#if false
			Epsitec.Common.Document.Engine.Initialize();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive("LookMetal");
			Epsitec.Common.Widgets.Widget.Initialize ();
#endif
		}

		[Test]
		public void Check01SaveDialogData()
		{
			EntityContext      context = EntityContext.Current;
			PersistenceManager manager = new PersistenceManager ();

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

#if false
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

		[Test]
		public void Check02RestoreDialogData()
		{
			EntityContext      context = EntityContext.Current;
			PersistenceManager manager = new PersistenceManager ();

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
			
			Assert.AreEqual ("Case postale 16", temp.CasePostale);
			Assert.AreEqual ("Yvonand", temp.Localité.Nom);
#if false
			XElement element;

			//	Restore CasePostale (value) and Localité (reference)
			element = XElement.Parse (@"<dialogData><data path=""[8V14]"" value=""CP 16"" /><ref path=""[8V15]"" id=""loc2"" /></dialogData>");
			States.FormState.RestoreDialogData (data, element);

			Assert.AreEqual ("CP 16", temp.CasePostale);
			Assert.AreEqual ("Yverdon-les-Bains", temp.Localité.Nom);
			Assert.AreEqual (loc2, temp.Localité);
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
