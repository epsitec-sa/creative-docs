//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectEditorPageAmortissements : AbstractObjectEditorPage
	{
		public ObjectEditorPageAmortissements(DataAccessor accessor)
			: base (accessor)
		{
		}


		public override IEnumerable<EditionObjectPageType> ChildrenPageTypes
		{
			get
			{
				yield return EditionObjectPageType.Compta;
			}
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateStringController  (parent, ObjectField.NomCatégorie);
			this.CreateDateController    (parent, ObjectField.DateAmortissement1);
			this.CreateDateController    (parent, ObjectField.DateAmortissement2);
			this.CreateDecimalController (parent, ObjectField.TauxAmortissement, DecimalFormat.Rate);
			this.CreateStringController  (parent, ObjectField.TypeAmortissement, editWidth: 90);
			this.CreateIntController     (parent, ObjectField.FréquenceAmortissement);
			this.CreateDecimalController (parent, ObjectField.ValeurRésiduelle, DecimalFormat.Amount);
		}
	}
}
