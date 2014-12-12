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
	public abstract class AbstractEditorPageCategory : AbstractEditorPage
	{
		public AbstractEditorPageCategory(DataAccessor accessor, CommandContext commandContext, BaseType baseType, bool isTimeless)
			: base (accessor, commandContext, baseType, isTimeless)
		{
			this.controllers = new Dictionary<ObjectField, AbstractFieldController> ();
		}


		protected void CreateCommonUI(Widget parent)
		{
			this.methodController      = this.CreateMethodGuidController (parent, ObjectField.MethodGuid);
			this.periodicityController = this.CreateEnumController       (parent, ObjectField.Periodicity, EnumDictionaries.DictPeriodicities, editWidth: 90);

			this.controllers.Clear ();

			foreach (var argument in ArgumentsLogic.GetSortedArguments (this.accessor))
			{
				var type  = (ArgumentType) ObjectProperties.GetObjectPropertyInt (argument, null, ObjectField.ArgumentType);
				var field = (ObjectField)  ObjectProperties.GetObjectPropertyInt (argument, null, ObjectField.ArgumentField);

				AbstractFieldController c = null;

				switch (type)
				{
					case ArgumentType.Decimal:
						c = this.CreateDecimalController (parent, field, DecimalFormat.Real);
						break;

					case ArgumentType.Amount:
						c = this.CreateDecimalController (parent, field, DecimalFormat.Amount);
						break;

					case ArgumentType.Rate:
						c = this.CreateDecimalController (parent, field, DecimalFormat.Rate);
						break;

					case ArgumentType.Years:
						c = this.CreateDecimalController (parent, field, DecimalFormat.Years);
						break;

					case ArgumentType.Int:
						c = this.CreateIntController (parent, field);
						break;

					case ArgumentType.Bool:
						c = this.CreateBoolController (parent, field);
						break;

					case ArgumentType.Date:
						c = this.CreateDateController (parent, field);
						break;

					case ArgumentType.String:
						c = this.CreateStringController (parent, field);
						break;

					default:
						throw new System.InvalidOperationException (string.Format ("Invalid ArgumentType {0}", type));
				}
			}

			this.methodController.ValueEdited += delegate
			{
				this.UpdateControllers ();
			};

			this.UpdateControllers ();
		}

		protected void CreateAccountsUI(Widget parent, System.DateTime? forcedDate)
		{
			foreach (var field in DataAccessor.AccountFields)
			{
				this.CreateAccountController (parent, field, forcedDate);
			}

			this.entrySamples = new EntrySamples (this.accessor, forcedDate);
			this.entrySamples.CreateUI (parent);
		}


		public override void SetObject(Guid objectGuid, Timestamp timestamp)
		{
			base.SetObject (objectGuid, timestamp);
			this.UpdateControllers ();
		}

		protected void UpdateControllers()
		{
			var methodGuid = this.accessor.EditionAccessor.GetFieldGuid (ObjectField.MethodGuid);

			foreach (var pair in this.controllers)
			{
				var field      = pair.Key;
				var controller = pair.Value;

				var hidden = MethodsLogic.IsHidden (this.accessor, methodGuid, field);
				controller.IsReadOnly = hidden;
			}
		}


		private readonly Dictionary<ObjectField, AbstractFieldController> controllers;

		private MethodGuidFieldController		methodController;
		private EnumFieldController				periodicityController;
	}
}
