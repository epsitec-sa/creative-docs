//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageGroup : AbstractEditorPage
	{
		public EditorPageGroup(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateIntController     (parent, ObjectField.Level);
			//?this.CreateStringController  (parent, ObjectField.Numéro, editWidth: 90);
			//?this.CreateStringController  (parent, ObjectField.Nom);
			this.CreateStringController  (parent, ObjectField.Description, lineCount: 5);
			this.CreateStringController  (parent, ObjectField.Famille);
			this.CreateStringController  (parent, ObjectField.Membre);
		}
	}
}
