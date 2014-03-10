//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageEntry : AbstractEditorPage
	{
		public EditorPageEntry(DataAccessor accessor, BaseType baseType, BaseType subBaseType, bool isTimeless)
			: base (accessor, baseType, subBaseType, isTimeless)
		{
		}


		protected internal override void CreateUI(Widget parent)
		{
			parent = this.CreateScrollable (parent);

			this.CreateDateController      (parent, ObjectField.EntryDate);
			this.CreateGroupGuidController (parent, ObjectField.EntryDebitAccount, BaseType.Accounts);
			this.CreateGroupGuidController (parent, ObjectField.EntryCreditAccount, BaseType.Accounts);
			this.CreateStringController    (parent, ObjectField.EntryStamp, editWidth: 100);
			this.CreateStringController    (parent, ObjectField.EntryTitle);
			this.CreateDecimalController   (parent, ObjectField.EntryAmount, DecimalFormat.Amount);
		}
	}
}
