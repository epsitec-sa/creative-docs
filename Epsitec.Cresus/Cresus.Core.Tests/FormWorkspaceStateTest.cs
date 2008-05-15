//	Copyright � 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public class FormWorkspaceTest
	{
		[TestFixtureSetUp]
		public void Initialize()
		{
			Epsitec.Cresus.Core.States.StateFactory.Setup ();
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
			PersistanceManager manager = new PersistanceManager ();

			AdresseEntity   adr  = context.CreateEmptyEntity<AdresseEntity> ();
			Localit�1Entity loc1 = context.CreateEmptyEntity<Localit�1Entity> ();
			Localit�1Entity loc2 = context.CreateEmptyEntity<Localit�1Entity> ();

			manager.Map["loc1"] = loc1;
			manager.Map["loc2"] = loc2;

			context.PersistanceManagers.Add (manager);

			using (adr.DefineOriginalValues ())
			{
				adr.CasePostale = "Case postale 16";
				adr.Rue = "Rue du Lac 6";
				adr.Localit� = loc1;
			}

			using (loc1.DefineOriginalValues ())
			{
				loc1.Nom = "Yvonand";
				loc1.Num�ro = "1462";
			}

			using (loc2.DefineOriginalValues ())
			{
				loc1.Nom = "Yverdon-les-Bains";
				loc1.Num�ro = "1400";
			}

			DialogData    data = new DialogData (adr, DialogDataMode.Isolated);
			AdresseEntity temp = data.Data as AdresseEntity;
			XElement element;

			//	No changes :
			element = States.FormWorkspaceState.SaveDialogData (data);
			Assert.AreEqual (@"<dialogData />", element.ToString (SaveOptions.DisableFormatting));
			
			//	Single text change :
			temp.CasePostale = "CP 16";
			element = States.FormWorkspaceState.SaveDialogData (data);
			Assert.AreEqual (@"<dialogData><data path=""[8V14]"" value=""CP 16"" /></dialogData>", element.ToString (SaveOptions.DisableFormatting));

			//	Cascaded text change :
			temp.Localit�.Num�ro = "CH-1462";
			element = States.FormWorkspaceState.SaveDialogData (data);
			Assert.AreEqual (@"<dialogData><data path=""[8V14]"" value=""CP 16"" /><data path=""[8V15].[8V19]"" value=""CH-1462"" /></dialogData>", element.ToString (SaveOptions.DisableFormatting));

			temp.Localit� = loc2;
			element = States.FormWorkspaceState.SaveDialogData (data);
			Assert.AreEqual (@"<dialogData><data path=""[8V14]"" value=""CP 16"" /><ref path=""[8V15]"" id=""loc2"" /></dialogData>", element.ToString (SaveOptions.DisableFormatting));

			data.RevertChanges ();
			element = States.FormWorkspaceState.SaveDialogData (data);
			Assert.AreEqual (@"<dialogData />", element.ToString (SaveOptions.DisableFormatting));

			context.PersistanceManagers.Remove (manager);
		}

		[Test]
		public void Check02RestoreDialogData()
		{
			EntityContext      context = EntityContext.Current;
			PersistanceManager manager = new PersistanceManager ();

			AdresseEntity   adr  = context.CreateEmptyEntity<AdresseEntity> ();
			Localit�1Entity loc1 = context.CreateEmptyEntity<Localit�1Entity> ();
			Localit�1Entity loc2 = context.CreateEmptyEntity<Localit�1Entity> ();

			manager.Map["loc1"] = loc1;
			manager.Map["loc2"] = loc2;

			context.PersistanceManagers.Add (manager);

			using (adr.DefineOriginalValues ())
			{
				adr.CasePostale = "Case postale 16";
				adr.Rue = "Rue du Lac 6";
				adr.Localit� = loc1;
			}

			using (loc1.DefineOriginalValues ())
			{
				loc1.Nom = "Yvonand";
				loc1.Num�ro = "1462";
			}

			using (loc2.DefineOriginalValues ())
			{
				loc1.Nom = "Yverdon-les-Bains";
				loc1.Num�ro = "1400";
			}

			DialogData    data = new DialogData (adr, DialogDataMode.Isolated);
			AdresseEntity temp = data.Data as AdresseEntity;
			XElement element = XElement.Parse (@"<dialogData><data path=""[8V14]"" value=""CP 16"" /><ref path=""[8V15]"" id=""loc2"" /></dialogData>");

			States.FormWorkspaceState.RestoreDialogData (data, element);

			Assert.AreEqual ("CP 16", temp.CasePostale);
			Assert.AreEqual (loc2, temp.Localit�);

			context.PersistanceManagers.Remove (manager);
		}

		private class PersistanceManager : IEntityPersistanceManager
		{
			#region IEntityPersistanceManager Members

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

			public AbstractEntity GetPeristedEntity(string id, Druid entityId)
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
	}
}
