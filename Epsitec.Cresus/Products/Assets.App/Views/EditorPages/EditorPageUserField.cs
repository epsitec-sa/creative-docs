//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.EditorPages
{
	public class EditorPageUserField : AbstractEditorPage
	{
		public EditorPageUserField(DataAccessor accessor, CommandContext commandContext, BaseType baseType, bool isTimeless)
			: base (accessor, commandContext, baseType, isTimeless)
		{
		}


		protected internal override void CreateUI(Widget parent)
		{
			parent = this.CreateScrollable (parent, hasColorsExplanation: false);

			this.CreateStringController (parent, ObjectField.Name);

			Dictionary<int, string> dict;
			if (this.baseType == BaseType.AssetsUserFields)
			{
				dict = EnumDictionaries.GetDictFieldTypes ();
			}
			else
			{
				dict = EnumDictionaries.GetDictFieldTypes (hasComplexTypes: false);
			}
			this.typeController = this.CreateEnumController (parent, ObjectField.UserFieldType, dict, editWidth: 100);

			this.CreateBoolController (parent, ObjectField.UserFieldRequired);

			this.CreateSepartor (parent);

			new StaticText
			{
				Parent  = parent,
				Text    = Res.Strings.EditorPages.UserFields.TreeTable.ToString (),
				Dock    = DockStyle.Top,
				Margins = new Epsitec.Common.Drawing.Margins (0, 0, 20, 10),
			};

			this.CreateIntController (parent, ObjectField.UserFieldColumnWidth);

			new StaticText
			{
				Parent  = parent,
				Text    = Res.Strings.EditorPages.UserFields.Edition.ToString (),
				Dock    = DockStyle.Top,
				Margins = new Epsitec.Common.Drawing.Margins (0, 0, 20, 10),
			};

			this.lineWidthController = this.CreateIntController (parent, ObjectField.UserFieldLineWidth);
			this.lineCountController = this.CreateIntController (parent, ObjectField.UserFieldLineCount);
			                           this.CreateIntController (parent, ObjectField.UserFieldTopMargin);

			new StaticText
			{
				Parent  = parent,
				Text    = Res.Strings.EditorPages.UserFields.Summary.ToString (),
				Dock    = DockStyle.Top,
				Margins = new Epsitec.Common.Drawing.Margins (0, 0, 20, 10),
			};

			this.CreateIntController (parent, ObjectField.UserFieldSummaryOrder);

			if (this.baseType == BaseType.AssetsUserFields)
			{
				new StaticText
				{
					Parent  = parent,
					Text    = Res.Strings.EditorPages.UserFields.MCH2Summary.ToString (),
					Dock    = DockStyle.Top,
					Margins = new Epsitec.Common.Drawing.Margins (0, 0, 20, 10),
				};

				this.CreateIntController (parent, ObjectField.UserFieldMCH2SummaryOrder);
			}

			//	Connexion des événements.
			this.typeController.ValueEdited += delegate (object sender, ObjectField val1)
			{
				this.UpdateType ();
			};
		}

		private void UpdateType()
		{
			if (this.FieldType == FieldType.String)
			{
				if (!this.lineWidthController.Value.HasValue)
				{
					this.lineWidthController.Value = AbstractFieldController.maxWidth;
					this.lineWidthController.ValueChanged ();
				}

				if (!this.lineCountController.Value.HasValue)
				{
					this.lineCountController.Value = 1;
					this.lineCountController.ValueChanged ();
				}
			}
			else
			{
				this.lineWidthController.Value = null;
				this.lineCountController.Value = null;

				this.lineWidthController.ValueChanged ();
				this.lineCountController.ValueChanged ();
			}
		}

		private FieldType FieldType
		{
			get
			{
				if (this.typeController.Value.HasValue)
				{
					return (FieldType) this.typeController.Value.Value;
				}
				else
				{
					return FieldType.Unknown;
				}
			}
		}


		private EnumFieldController				typeController;
		private IntFieldController				lineWidthController;
		private IntFieldController				lineCountController;
	}
}
