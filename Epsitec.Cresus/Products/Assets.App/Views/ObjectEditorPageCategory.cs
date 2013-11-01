//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectEditorPageCategory : AbstractObjectEditorPage
	{
		public ObjectEditorPageCategory(DataAccessor accessor)
			: base (accessor)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateIntController     (parent, ObjectField.CatégorieLevel);
			this.CreateStringController  (parent, ObjectField.CatégorieNuméro, editWidth: 90);
			this.CreateStringController  (parent, ObjectField.CatégorieNom);
			this.CreateStringController  (parent, ObjectField.CatégorieDescription, lineCount: 5);
			this.CreateDecimalController (parent, ObjectField.CatégorieTauxAmortissement, DecimalFormat.Rate);
			this.CreateStringController  (parent, ObjectField.CatégorieTypeAmortissement, editWidth: 90);
			this.CreateStringController  (parent, ObjectField.CatégoriePériodicité, editWidth: 90);
			this.CreateDecimalController (parent, ObjectField.CatégorieValeurRésiduelle, DecimalFormat.Amount);
		}
	}
}
