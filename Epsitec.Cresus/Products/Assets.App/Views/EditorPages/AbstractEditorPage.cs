//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;
using Epsitec.Cresus.Assets.App.Views.ViewStates;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.EditorPages
{
	public abstract class AbstractEditorPage
	{
		public AbstractEditorPage(DataAccessor accessor, CommandContext commandContext, BaseType baseType, bool isTimeless)
		{
			this.accessor       = accessor;
			this.commandContext = commandContext;
			this.baseType       = baseType;
			this.isTimeless     = isTimeless;

			this.fieldControllers = new Dictionary<ObjectField, AbstractFieldController> ();
			this.colorsExplanationController = new ColorsExplanationController ();
		}

		public void Dispose()
		{
		}


		protected internal virtual void CreateUI(Widget parent)
		{
		}

		protected void CreateColorsExplanation(Widget parent)
		{
			this.colorsExplanationController.CreateUI (parent);
		}


		public virtual void SetObject(Guid objectGuid, Timestamp timestamp)
		{
			this.objectGuid = objectGuid;
			this.obj        = this.accessor.GetObject (this.baseType, this.objectGuid);
			this.timestamp  = timestamp;
			this.hasEvent   = false;
			this.eventType  = EventType.Unknown;
			this.isLocked   = AssetCalculator.IsLocked (this.obj, this.timestamp);

			this.ShowHideLockedWidgets ();

			if (!this.objectGuid.IsEmpty && this.obj != null)
			{
				var e = this.obj.GetEvent (this.timestamp);

				if (e != null)
				{
					this.eventType = e.Type;
					this.hasEvent  = true;
				}
			}

			foreach (var pair in this.fieldControllers)
			{
				var field      = pair.Key;
				var controller = pair.Value;

				if (controller is StringFieldController)
				{
					var c = controller as StringFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldString (field);
					c.PropertyState = this.GetPropertyState (field);
					c.IsReadOnly    = this.isLocked;
				}
				else if (controller is EnumFieldController)
				{
					var c = controller as EnumFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldInt (field);
					c.PropertyState = this.GetPropertyState (field);
					c.IsReadOnly    = this.isLocked;
				}
				else if (controller is DecimalFieldController)
				{
					var c = controller as DecimalFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldDecimal (field);
					c.PropertyState = this.GetPropertyState (field);
					c.IsReadOnly    = this.isLocked;
				}
				else if (controller is ComputedAmountFieldController)
				{
					var c = controller as ComputedAmountFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldComputedAmount (field);
					c.PropertyState = this.GetPropertyState (field);
					c.IsReadOnly    = this.isLocked;
				}
				else if (controller is AmortizedAmountFieldController)
				{
					var c = controller as AmortizedAmountFieldController;

					c.EventType     = this.eventType;
					c.PropertyState = this.GetPropertyState (field);
					c.IsReadOnly    = this.isLocked;

					c.SetValue (
						this.accessor.EditionAccessor.EditedObject,
						this.accessor.EditionAccessor.EditedEvent,
						this.accessor.EditionAccessor.GetFieldAmortizedAmount (field));
				}
				else if (controller is IntFieldController)
				{
					var c = controller as IntFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldInt (field);
					c.PropertyState = this.GetPropertyState (field);
					c.IsReadOnly    = this.isLocked;
				}
				else if (controller is BoolFieldController)
				{
					var c = controller as BoolFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldInt (field) == 1;
					c.PropertyState = this.GetPropertyState (field);
					c.IsReadOnly    = this.isLocked;
				}
				else if (controller is DateFieldController)
				{
					var c = controller as DateFieldController;

					c.EventType     = this.eventType;
					c.MinValue      = this.accessor.EditionAccessor.GetFieldDateMin (field);
					c.MaxValue      = this.accessor.EditionAccessor.GetFieldDateMax (field);
					c.Value         = this.accessor.EditionAccessor.GetFieldDate (field);
					c.PropertyState = this.GetPropertyState (field);
					c.IsReadOnly    = this.isLocked;
				}
				else if (controller is GroupGuidFieldController)
				{
					var c = controller as GroupGuidFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldGuid (field);
					c.PropertyState = this.GetPropertyState (field);
					c.IsReadOnly    = this.isLocked;
				}
				else if (controller is PersonGuidFieldController)
				{
					var c = controller as PersonGuidFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldGuid (field);
					c.PropertyState = this.GetPropertyState (field);
					c.IsReadOnly    = this.isLocked;
				}
				else if (controller is MethodGuidFieldController)
				{
					var c = controller as MethodGuidFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldGuid (field);
					c.PropertyState = this.GetPropertyState (field);
					c.IsReadOnly    = this.isLocked;
				}
				else if (controller is AccountFieldController)
				{
					var c = controller as AccountFieldController;

					c.EventType     = this.eventType;
					c.Date          = this.accessor.EditionAccessor.EventDate;
					c.Value         = this.accessor.EditionAccessor.GetFieldString (field);
					c.PropertyState = this.GetPropertyState (field);
					c.IsReadOnly    = this.isLocked;
				}
				else if (controller is GuidRatioFieldController)
				{
					var c = controller as GuidRatioFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldGuidRatio (field);
					c.PropertyState = this.GetPropertyState (field);
					c.IsReadOnly    = this.isLocked;
				}
				else if (controller is GuidRatioFieldsController)
				{
					var c = controller as GuidRatioFieldsController;

					c.IsReadOnly    = this.isLocked;
					c.Update ();
				}
			}

			this.UpdateColorsExplanation ();
			this.UpdateEntrySamples ();
		}


		public ObjectField GetFocus()
		{
			return this.fieldFocus;
		}

		public void SetFocus(ObjectField field)
		{
			if (this.fieldControllers.ContainsKey (field))
			{
				this.fieldControllers[field].SetFocus ();
			}
			else
			{
				//	Si on n'a pas trouvé le champ, ou s'il n'était pas précisé en
				//	entrée, on met le focus dans le premier champ.
				if (this.fieldControllers.Any ())
				{
					this.fieldControllers.First ().Value.SetFocus ();
				}
			}
		}


		protected virtual void UpdateColorsExplanation()
		{
			//	Met à jour les explications sur les couleurs en bas de la vue.
			this.colorsExplanationController.ClearTypesToShow ();

			if (this.hasEvent)
			{
				foreach (var controller in this.fieldControllers.Values)
				{
					this.colorsExplanationController.AddTypesToShow (controller.FieldColorTypes);
				}
			}

			this.colorsExplanationController.Update ();
		}

		private void UpdateEntrySamples()
		{
			if (this.entrySamples != null)
			{
				this.entrySamples.Update ();
			}
		}


		protected void CreateController(Widget parent, UserField userField)
		{
			//	Crée un contrôleur du bon type pour un UserField.
			if (userField.TopMargin > 0)
			{
				this.CreateSepartor (parent, userField.TopMargin);
			}

			switch (userField.Type)
			{
				case FieldType.String:
					this.CreateStringController (parent, userField.Field, userField.LineWidth.GetValueOrDefault (AbstractFieldController.maxWidth), userField.LineCount.GetValueOrDefault (1));
					break;

				case FieldType.Int:
					this.CreateIntController (parent, userField.Field);
					break;

				case FieldType.Decimal:
					this.CreateDecimalController (parent, userField.Field, DecimalFormat.Real);
					break;

				case FieldType.ComputedAmount:
					this.CreateComputedAmountController (parent, userField.Field);
					break;

				case FieldType.AmortizedAmount:
					this.CreateAmortizedAmountController (parent, userField.Field);
					break;

				case FieldType.Date:
					this.CreateDateController (parent, userField.Field);
					break;

				case FieldType.GuidPerson:
					this.CreatePersonGuidController (parent, userField.Field);
					break;

				case FieldType.GuidMethod:
					this.CreateMethodGuidController (parent, userField.Field);
					break;

				case FieldType.Account:
					this.CreateAccountController (parent, userField.Field);
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown FieldType {0}", userField.Type.ToString ()));
			}
		}

		protected GroupGuidFieldController CreateGroupGuidController(Widget parent, ObjectField field, BaseType baseType)
		{
			var controller = new GroupGuidFieldController (this.accessor)
			{
				Accessor  = this.accessor,
				BaseType  = baseType,
				Field     = field,
				Required  = WarningsLogic.IsRequired (this.accessor, baseType, field),
				Label     = this.accessor.GetFieldName (field),
				EditWidth = AbstractFieldController.maxWidth,
				TabIndex  = this.tabIndex,
			};

			controller.CreateUI (parent);
			this.tabIndex = controller.TabIndex;

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldGuid (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.fieldFocus = of;
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.ShowHistoryPopup (target, of);
			};

			controller.Goto += delegate (object sender, AbstractViewState viewState)
			{
				this.OnGoto (viewState);
			};

			this.fieldControllers.Add (field, controller);

			return controller;
		}

		protected PersonGuidFieldController CreatePersonGuidController(Widget parent, ObjectField field)
		{
			var controller = new PersonGuidFieldController (this.accessor)
			{
				Accessor  = this.accessor,
				Field     = field,
				Required  = WarningsLogic.IsRequired (this.accessor, this.baseType, field),
				Label     = this.accessor.GetFieldName (field),
				EditWidth = AbstractFieldController.maxWidth,
				TabIndex  = this.tabIndex,
			};

			controller.CreateUI (parent);
			this.tabIndex = controller.TabIndex;

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldGuid (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.fieldFocus = of;
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.ShowHistoryPopup (target, of);
			};

			controller.Goto += delegate (object sender, AbstractViewState viewState)
			{
				this.OnGoto (viewState);
			};

			this.fieldControllers.Add (field, controller);

			return controller;
		}

		protected MethodGuidFieldController CreateMethodGuidController(Widget parent, ObjectField field)
		{
			var controller = new MethodGuidFieldController (this.accessor)
			{
				Accessor  = this.accessor,
				Field     = field,
				Required  = WarningsLogic.IsRequired (this.accessor, this.baseType, field),
				Label     = this.accessor.GetFieldName (field),
				EditWidth = AbstractFieldController.maxWidth,
				TabIndex  = this.tabIndex,
			};

			controller.CreateUI (parent);
			this.tabIndex = controller.TabIndex;

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldGuid (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.fieldFocus = of;
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.ShowHistoryPopup (target, of);
			};

			controller.Goto += delegate (object sender, AbstractViewState viewState)
			{
				this.OnGoto (viewState);
			};

			this.fieldControllers.Add (field, controller);

			return controller;
		}

		protected AccountFieldController CreateAccountController(Widget parent, ObjectField field, System.DateTime? forcedDate = null)
		{
			var controller = new AccountFieldController (this.accessor)
			{
				ForcedDate = forcedDate,
				Date       = this.accessor.EditionAccessor.EventDate,
				Field      = field,
				Required   = WarningsLogic.IsRequired (this.accessor, this.baseType, field),
				Label      = this.accessor.GetFieldName (field),
				EditWidth  = AbstractFieldController.maxWidth,
				TabIndex   = this.tabIndex,
			};

			controller.CreateUI (parent);
			this.tabIndex = controller.TabIndex;

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldString (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.fieldFocus = of;
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.ShowHistoryPopup (target, of);
			};

			controller.Goto += delegate (object sender, AbstractViewState viewState)
			{
				this.OnGoto (viewState);
			};

			this.fieldControllers.Add (field, controller);

			return controller;
		}

		protected GuidRatioFieldsController CreateGuidRatiosController(Widget parent)
		{
			var controller = new GuidRatioFieldsController (this.accessor);

			controller.CreateUI (parent);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.OnValueEdited (of);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.fieldFocus = of;
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.ShowHistoryPopup (target, of);
			};

			controller.Goto += delegate (object sender, AbstractViewState viewState)
			{
				this.OnGoto (viewState);
			};

			this.fieldControllers.Add (ObjectField.GroupGuidRatioFirst, controller);

			return controller;
		}

		protected StringFieldController CreateStringController(Widget parent, ObjectField field, int editWidth = AbstractFieldController.maxWidth, int lineCount = 1, int maxLength = 500)
		{
			var controller = new StringFieldController (this.accessor)
			{
				Field     = field,
				Required  = WarningsLogic.IsRequired (this.accessor, this.baseType, field),
				Label     = this.accessor.GetFieldName (field),
				EditWidth = editWidth,
				LineCount = lineCount,
				TabIndex  = this.tabIndex,
				MaxLength = maxLength,
			};

			controller.CreateUI (parent);
			this.tabIndex = controller.TabIndex;

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldString (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.fieldFocus = of;
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.ShowHistoryPopup (target, of);
			};

			controller.Goto += delegate (object sender, AbstractViewState viewState)
			{
				this.OnGoto (viewState);
			};

			this.fieldControllers.Add (field, controller);

			return controller;
		}

		protected EnumFieldController CreateEnumController(Widget parent, ObjectField field, Dictionary<int, string> enums, int editWidth = AbstractFieldController.maxWidth)
		{
			var controller = new EnumFieldController (this.accessor)
			{
				Field     = field,
				Required  = WarningsLogic.IsRequired (this.accessor, this.baseType, field),
				Label     = this.accessor.GetFieldName (field),
				EditWidth = editWidth,
				Enums     = enums,
				TabIndex  = this.tabIndex,
			};

			controller.CreateUI (parent);
			this.tabIndex = controller.TabIndex;

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldInt (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.fieldFocus = of;
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.ShowHistoryPopup (target, of);
			};

			controller.Goto += delegate (object sender, AbstractViewState viewState)
			{
				this.OnGoto (viewState);
			};

			this.fieldControllers.Add (field, controller);

			return controller;
		}

		protected DecimalFieldController CreateDecimalController(Widget parent, ObjectField field, DecimalFormat format, int? editWidth = null)
		{
			var controller = new DecimalFieldController (this.accessor)
			{
				Field         = field,
				Required      = WarningsLogic.IsRequired (this.accessor, this.baseType, field),
				Label         = this.accessor.GetFieldName (field),
				EditWidth     = editWidth.HasValue ? editWidth.Value : 0,
				DecimalFormat = format,
				TabIndex      = this.tabIndex,
			};

			controller.CreateUI (parent);
			this.tabIndex = controller.TabIndex;

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldDecimal (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.fieldFocus = of;
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.ShowHistoryPopup (target, of);
			};

			controller.Goto += delegate (object sender, AbstractViewState viewState)
			{
				this.OnGoto (viewState);
			};

			this.fieldControllers.Add (field, controller);

			return controller;
		}

		protected ComputedAmountFieldController CreateComputedAmountController(Widget parent, ObjectField field)
		{
			var controller = new ComputedAmountFieldController (this.accessor)
			{
				Field     = field,
				Required  = WarningsLogic.IsRequired (this.accessor, this.baseType, field),
				Label     = this.accessor.GetFieldName (field),
				TabIndex  = this.tabIndex,
			};

			controller.CreateUI (parent);
			this.tabIndex = controller.TabIndex;

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldComputedAmount (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.fieldFocus = of;
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.ShowHistoryPopup (target, of);
			};

			controller.Goto += delegate (object sender, AbstractViewState viewState)
			{
				this.OnGoto (viewState);
			};

			this.fieldControllers.Add (field, controller);

			return controller;
		}

		protected AmortizedAmountFieldController CreateAmortizedAmountController(Widget parent, ObjectField field)
		{
			var controller = new AmortizedAmountFieldController (this.accessor)
			{
				Field                 = field,
				Required              = WarningsLogic.IsRequired (this.accessor, this.baseType, field),
				Label                 = this.accessor.GetFieldName (field),
				HideAdditionalButtons = true,
				TabIndex              = this.tabIndex,
			};

			controller.CreateUI (parent);
			this.tabIndex = controller.TabIndex;

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.SetValue (
					this.accessor.EditionAccessor.EditedObject,
					this.accessor.EditionAccessor.EditedEvent,
					this.accessor.EditionAccessor.GetFieldAmortizedAmount (of));

				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.fieldFocus = of;
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.ShowHistoryPopup (target, of);
			};

			controller.Goto += delegate (object sender, AbstractViewState viewState)
			{
				this.OnGoto (viewState);
			};

			this.fieldControllers.Add (field, controller);

			return controller;
		}

		protected IntFieldController CreateIntController(Widget parent, ObjectField field)
		{
			var controller = new IntFieldController (this.accessor)
			{
				Field     = field,
				Required  = WarningsLogic.IsRequired (this.accessor, this.baseType, field),
				Label     = this.accessor.GetFieldName (field),
				TabIndex  = this.tabIndex,
			};

			controller.CreateUI (parent);
			this.tabIndex = controller.TabIndex;

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldInt (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.fieldFocus = of;
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.ShowHistoryPopup (target, of);
			};

			controller.Goto += delegate (object sender, AbstractViewState viewState)
			{
				this.OnGoto (viewState);
			};

			this.fieldControllers.Add (field, controller);

			return controller;
		}

		protected BoolFieldController CreateBoolController(Widget parent, ObjectField field)
		{
			var controller = new BoolFieldController (this.accessor)
			{
				Field     = field,
				Required  = WarningsLogic.IsRequired (this.accessor, this.baseType, field),
				Label     = this.accessor.GetFieldName (field),
				TabIndex  = this.tabIndex,
			};

			controller.CreateUI (parent);
			this.tabIndex = controller.TabIndex;

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value ? 1:0);

				controller.Value         = this.accessor.EditionAccessor.GetFieldInt (of) != 0;
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.fieldFocus = of;
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.ShowHistoryPopup (target, of);
			};

			controller.Goto += delegate (object sender, AbstractViewState viewState)
			{
				this.OnGoto (viewState);
			};

			this.fieldControllers.Add (field, controller);

			return controller;
		}

		protected DateFieldController CreateDateController(Widget parent, ObjectField field, DateRangeCategory rangeCategory = DateRangeCategory.Free)
		{
			var controller = new DateFieldController (this.accessor)
			{
				Field             = field,
				Required          = WarningsLogic.IsRequired (this.accessor, this.baseType, field),
				Label             = this.accessor.GetFieldName (field),
				DateRangeCategory = rangeCategory,
				TabIndex          = this.tabIndex,
			};

			controller.CreateUI (parent);
			this.tabIndex = controller.TabIndex;

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.MinValue      = this.accessor.EditionAccessor.GetFieldDateMin (of);
				controller.MaxValue      = this.accessor.EditionAccessor.GetFieldDateMax (of);
				controller.Value         = this.accessor.EditionAccessor.GetFieldDate    (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
			};

			controller.SetFieldFocus += delegate (object sender, ObjectField of)
			{
				this.fieldFocus = of;
			};

			controller.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.ShowHistoryPopup (target, of);
			};

			controller.Goto += delegate (object sender, AbstractViewState viewState)
			{
				this.OnGoto (viewState);
			};

			this.fieldControllers.Add (field, controller);

			return controller;
		}

		protected void CreateSepartor(Widget parent, int margin = 10)
		{
			new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = margin,
			};
		}

		protected void CreateSubtitle(Widget parent, string text)
		{
			new StaticText
			{
				Parent          = parent,
				Text            = text,
				Dock            = DockStyle.Top,
				Margins         = new Margins (AbstractFieldController.labelWidth+10, 0, 10, 10),
			};
		}


		protected Widget CreateScrollable(Widget parent, bool hasColorsExplanation)
		{
			this.CreateLockedWidgets (parent);

			if (hasColorsExplanation)
			{
				this.CreateColorsExplanation (parent);
			}

			//	Crée la zone scrollable verticalement contenant tous les contrôleurs.
			this.scrollable = new Scrollable
			{
				Parent                 = parent,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode   = ScrollableScrollerMode.ShowAlways,
				Dock                   = DockStyle.Fill,
				Margins                = new Margins (10, 0, 0, 0),
			};

			this.scrollable.Viewport.IsAutoFitting = true;
			this.scrollable.Viewport.Padding = new Margins (0, 10, 10, 10);

			return this.scrollable.Viewport;
		}

		private void ShowHideLockedWidgets()
		{
			if (this.lockedBackground != null)
			{
				this.lockedMark      .Visibility = this.isLocked;
				this.lockedBackground.Visibility = this.isLocked;
			}
		}

		protected void CreateLockedWidgets(Widget parent)
		{
			//	Crée le fond hachuré, visible lorsque l'événements est bloqué.
			this.lockedBackground = new HatchFrameBox
			{
				Parent           = parent,
				Anchor           = AnchorStyles.All,
				Hatch            = true,
				Margins          = new Margins (0, AbstractScroller.DefaultBreadth, 0, 0),
			};

			//	Crée le cadenas, visible en bas à droite lorsque l'événements est bloqué.
			this.lockedMark = new StaticText
			{
				Parent           = parent,
				Anchor           = AnchorStyles.BottomRight,
				PreferredSize    = new Size (64, 64),
				ContentAlignment = ContentAlignment.MiddleCenter,
				Text             = Misc.GetRichTextImg ("Background.Locked", verticalOffset: 0),
				Margins          = new Margins (0, AbstractScroller.DefaultBreadth+10, 0, 10),
			};
		}


		protected void ShowHistoryPopup(Widget target, ObjectField field)
		{
			var popup = new HistoryPopup (this.accessor, this.baseType, this.objectGuid, this.timestamp, field);

			popup.Create (target, leftOrRight: true);

			popup.Navigate += delegate (object sender, Timestamp timestamp)
			{
				this.OnNavigate (timestamp);
			};
		}


		protected PropertyState GetPropertyState(ObjectField field)
		{
			if (this.isTimeless)
			{
				return PropertyState.Timeless;
			}
			else if (DataObject.IsOneShotField (field))
			{
				return PropertyState.OneShot;
			}
			else if (this.hasEvent)
			{
				return this.accessor.EditionAccessor.GetEditionPropertyState (field);
			}
			else
			{
				return PropertyState.Readonly;
			}
		}


		public static AbstractEditorPage CreatePage(DataAccessor accessor, CommandContext commandContext, BaseType baseType, PageType page)
		{
			switch (page)
			{
				case PageType.OneShot:
					return new EditorPageOneShot (accessor, commandContext, baseType, isTimeless: false);

				case PageType.Summary:
					return new EditorPageSummary (accessor, commandContext, baseType, isTimeless: false);

				case PageType.Asset:
					return new EditorPageAsset (accessor, commandContext, baseType, isTimeless: false);

				case PageType.AmortizationValue:
					return new EditorPageAmortizationValue (accessor, commandContext, baseType, isTimeless: false);

				case PageType.AmortizationDefinition:
					return new EditorPageAmortizationDefinition (accessor, commandContext, baseType, isTimeless: false);

				case PageType.Groups:
					return new EditorPageGroups (accessor, commandContext, baseType, isTimeless: false);

				case PageType.Category:
					return new EditorPageCategory (accessor, commandContext, baseType, isTimeless: true);

				case PageType.Group:
					return new EditorPageGroup (accessor, commandContext, baseType, isTimeless: true);

				case PageType.Person:
					return new EditorPagePerson (accessor, commandContext, baseType, isTimeless: true);

				case PageType.UserFields:
					return new EditorPageUserField (accessor, commandContext, baseType, isTimeless: true);

				case PageType.Account:
					return new EditorPageAccount (accessor, commandContext, baseType, isTimeless: true);

				case PageType.Method:
					return new EditorPageMethod (accessor, commandContext, baseType, isTimeless: true);

				case PageType.Argument:
					return new EditorPageArgument (accessor, commandContext, baseType, isTimeless: true);

				default:
					throw new System.InvalidOperationException (string.Format ("Unsupported page type {0}", page.ToString ()));
			}
		}


		#region Events handler
		private void OnNavigate(Timestamp timestamp)
		{
			this.Navigate.Raise (this, timestamp);
		}

		public event EventHandler<Timestamp> Navigate;


		protected void OnPageOpen(PageType type, ObjectField field)
		{
			this.PageOpen.Raise (this, type, field);
		}

		public event EventHandler<PageType, ObjectField> PageOpen;


		protected void OnGoto(AbstractViewState viewState)
		{
			this.Goto.Raise (this, viewState);
		}

		public event EventHandler<AbstractViewState> Goto;

	
		protected void OnValueEdited(ObjectField field)
		{
			this.UpdateColorsExplanation ();
			this.UpdateEntrySamples ();

			this.ValueEdited.Raise (this, field);
		}

		public event EventHandler<ObjectField> ValueEdited;
		#endregion


		protected readonly DataAccessor				accessor;
		protected readonly CommandContext			commandContext;
		protected readonly BaseType					baseType;
		protected readonly bool						isTimeless;
		private Dictionary<ObjectField, AbstractFieldController> fieldControllers;
		protected readonly ColorsExplanationController	colorsExplanationController;

		protected Scrollable						scrollable;
		protected StaticText						lockedMark;
		protected HatchFrameBox						lockedBackground;
		protected Guid								objectGuid;
		protected DataObject						obj;
		protected Timestamp							timestamp;
		protected bool								hasEvent;
		protected EventType							eventType;
		protected int								tabIndex;
		protected bool								isLocked;
		protected EntrySamples						entrySamples;
		private ObjectField							fieldFocus;
	}
}
