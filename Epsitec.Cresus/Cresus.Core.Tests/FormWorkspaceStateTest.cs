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
			EntityContext context = EntityContext.Current;

			AdresseEntity   adr = context.CreateEmptyEntity<AdresseEntity> ();
			Localité1Entity loc = context.CreateEmptyEntity<Localité1Entity> ();

			using (adr.DefineOriginalValues ())
			{
				adr.CasePostale = "Case postale 16";
				adr.Rue = "Rue du Lac 6";
				adr.Localité = loc;
			}

			using (loc.DefineOriginalValues ())
			{
				loc.Nom = "Yvonand";
				loc.Numéro = "1462";
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

			temp.Localité.Numéro = "CH-1462";
			element = States.FormWorkspaceState.SaveDialogData (data);
			Assert.AreEqual (@"<dialogData><data path=""[8V14]"" value=""CP 16"" /><data path=""[8V15].[8V19]"" value=""CH-1462"" /></dialogData>", element.ToString (SaveOptions.DisableFormatting));

			System.Console.Out.WriteLine (element.ToString (SaveOptions.DisableFormatting));
		}
	}
}
