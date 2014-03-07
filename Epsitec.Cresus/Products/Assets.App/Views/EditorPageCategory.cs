//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageCategory : AbstractEditorPage
	{
		public EditorPageCategory(DataAccessor accessor, BaseType baseType, BaseType subBaseType, bool isTimeless)
			: base (accessor, baseType, subBaseType, isTimeless)
		{
		}


		protected internal override void CreateUI(Widget parent)
		{
			parent = this.CreateScrollable (parent);

			this.CreateStringController  (parent, ObjectField.Number, editWidth: 90);
			this.CreateStringController  (parent, ObjectField.Name);
			this.CreateStringController  (parent, ObjectField.Description, lineCount: 5);
			this.CreateDecimalController (parent, ObjectField.AmortizationRate, DecimalFormat.Rate);
			this.CreateEnumController    (parent, ObjectField.AmortizationType, EnumDictionaries.DictAmortizationTypes, editWidth: 90);
			this.CreateEnumController    (parent, ObjectField.Periodicity, EnumDictionaries.DictPeriodicities, editWidth: 90);
			this.CreateEnumController    (parent, ObjectField.Prorata, EnumDictionaries.DictProrataTypes, editWidth: 90);
			this.CreateDecimalController (parent, ObjectField.Round, DecimalFormat.Amount);
			this.CreateDecimalController (parent, ObjectField.ResidualValue, DecimalFormat.Amount);

			this.CreateSepartor (parent);

			this.CreateAccountGuidController (parent, ObjectField.Account1);
			this.CreateAccountGuidController (parent, ObjectField.Account2);
			this.CreateAccountGuidController (parent, ObjectField.Account3);
			this.CreateAccountGuidController (parent, ObjectField.Account4);
			this.CreateAccountGuidController (parent, ObjectField.Account5);
			this.CreateAccountGuidController (parent, ObjectField.Account6);
			//this.CreateAccountGuidController (parent, ObjectField.Account7);
			//this.CreateAccountGuidController (parent, ObjectField.Account8);
		}
	}
}
