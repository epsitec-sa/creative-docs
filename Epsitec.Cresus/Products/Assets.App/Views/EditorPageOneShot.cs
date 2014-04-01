//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageOneShot : AbstractEditorPage
	{
		public EditorPageOneShot(DataAccessor accessor, BaseType baseType, BaseType subBaseType, bool isTimeless)
			: base (accessor, baseType, subBaseType, isTimeless)
		{
		}


		protected internal override void CreateUI(Widget parent)
		{
			parent = this.CreateScrollable (parent);

			new StaticText
			{
				Parent  = parent,
				Text    = "Informations ponctuelles liées à l'événement:",
				Dock    = DockStyle.Top,
				Margins = new Epsitec.Common.Drawing.Margins (0, 0, 0, 20),
			};

			this.CreateStringController (parent, ObjectField.OneShotNumber,    editWidth: 90);
			this.CreateDateController   (parent, ObjectField.OneShotDateOperation);
			this.CreateStringController (parent, ObjectField.OneShotComment,   lineCount: 5);
			this.CreateStringController (parent, ObjectField.OneShotDocuments, lineCount: 5);
		}
	}
}
