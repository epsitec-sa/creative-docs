//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPagePerson : AbstractEditorPage
	{
		public EditorPagePerson(DataAccessor accessor, BaseType baseType, bool isTimeless)
			: base (accessor, baseType, isTimeless)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateStringController (parent, ObjectField.Titre, editWidth: 120);
			this.CreateStringController (parent, ObjectField.Prénom);
			this.CreateStringController (parent, ObjectField.Nom);
			this.CreateStringController (parent, ObjectField.Entreprise);
			this.CreateStringController (parent, ObjectField.Adresse, lineCount: 2);
			this.CreateStringController (parent, ObjectField.Npa, editWidth: 60);
			this.CreateStringController (parent, ObjectField.Ville);
			this.CreateStringController (parent, ObjectField.Pays);

			this.CreateSepartor (parent);

			this.CreateStringController (parent, ObjectField.Téléphone1, editWidth: 120);
			this.CreateStringController (parent, ObjectField.Téléphone2, editWidth: 120);
			this.CreateStringController (parent, ObjectField.Téléphone3, editWidth: 120);
			this.CreateStringController (parent, ObjectField.Mail);

			this.CreateSepartor (parent);

			this.CreateStringController (parent, ObjectField.Description, lineCount: 5);
		}

		private void CreateSepartor(Widget parent)
		{
			new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = 10,
			};

		}
	}
}
