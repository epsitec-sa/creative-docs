//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageSingleton : AbstractEditorPage
	{
		public EditorPageSingleton(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
		}


		public override void CreateUI(Widget parent)
		{
			new StaticText
			{
				Parent  = parent,
				Text    = "Informations ponctuelles liées à l'événement:",
				Dock    = DockStyle.Top,
				Margins = new Epsitec.Common.Drawing.Margins (0, 0, 0, 20),
			};

			this.CreateStringController (parent, ObjectField.EvNuméro,      editWidth: 90);
			this.CreateStringController (parent, ObjectField.EvCommentaire, lineCount: 5);
			this.CreateStringController (parent, ObjectField.EvDocuments,   lineCount: 5);
		}
	}
}
