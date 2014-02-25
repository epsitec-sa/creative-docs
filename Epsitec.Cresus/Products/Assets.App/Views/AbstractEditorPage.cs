﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractEditorPage
	{
		public AbstractEditorPage(DataAccessor accessor, BaseType baseType, BaseType subBaseType, bool isTimeless)
		{
			this.accessor    = accessor;
			this.baseType    = baseType;
			this.subBaseType = subBaseType;
			this.isTimeless  = isTimeless;

			this.fieldControllers = new Dictionary<ObjectField, AbstractFieldController> ();
		}

		public void Dispose()
		{
		}


		public virtual void CreateUI(Widget parent)
		{
		}

		public virtual void SetObject(Guid objectGuid, Timestamp timestamp)
		{
			this.objectGuid = objectGuid;
			this.obj        = this.accessor.GetObject (this.baseType, this.objectGuid);
			this.timestamp  = timestamp;
			this.hasEvent   = false;
			this.eventType  = EventType.Unknown;

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
				}
				else if (controller is EnumFieldController)
				{
					var c = controller as EnumFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldInt (field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is DecimalFieldController)
				{
					var c = controller as DecimalFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldDecimal (field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is ComputedAmountFieldController)
				{
					var c = controller as ComputedAmountFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldComputedAmount (field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is IntFieldController)
				{
					var c = controller as IntFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldInt (field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is DateFieldController)
				{
					var c = controller as DateFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldDate (field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is GroupGuidFieldController)
				{
					var c = controller as GroupGuidFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldGuid (field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is PersonGuidFieldController)
				{
					var c = controller as PersonGuidFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldGuid (field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is GuidRatioFieldController)
				{
					var c = controller as GuidRatioFieldController;

					c.EventType     = this.eventType;
					c.Value         = this.accessor.EditionAccessor.GetFieldGuidRatio (field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is GuidRatioFieldsController)
				{
					var c = controller as GuidRatioFieldsController;
					c.Update ();
				}
			}
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

				case FieldType.Date:
					this.CreateDateController (parent, userField.Field);
					break;

				case FieldType.GuidPerson:
					this.CreatePersonGuidController (parent, userField.Field);
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown FieldType {0}", userField.Type.ToString ()));
			}
		}

		protected void CreateGroupGuidController(Widget parent, ObjectField field)
		{
			var controller = new GroupGuidFieldController
			{
				Accessor  = this.accessor,
				Field     = field,
				Label     = this.accessor.GetFieldName (field),
				EditWidth = AbstractFieldController.maxWidth,
				TabIndex  = ++this.tabIndex,
			};

			controller.CreateUI (parent);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldGuid (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
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
		}

		protected void CreatePersonGuidController(Widget parent, ObjectField field)
		{
			var controller = new PersonGuidFieldController
			{
				Accessor  = this.accessor,
				Field     = field,
				Label     = this.accessor.GetFieldName (field),
				EditWidth = AbstractFieldController.maxWidth,
				TabIndex  = ++this.tabIndex,
			};

			controller.CreateUI (parent);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldGuid (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
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
		}

		protected void CreateGuidRatiosController(Widget parent)
		{
			var controller = new GuidRatioFieldsController (this.accessor);

			controller.CreateUI (parent);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.OnValueEdited (of);
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
		}

		protected void CreateStringController(Widget parent, ObjectField field, int editWidth = AbstractFieldController.maxWidth, int lineCount = 1)
		{
			var controller = new StringFieldController
			{
				Field     = field,
				Label     = this.accessor.GetFieldName (field),
				EditWidth = editWidth,
				LineCount = lineCount,
				TabIndex  = ++this.tabIndex,
			};

			controller.CreateUI (parent);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldString (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
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
		}

		protected void CreateEnumController(Widget parent, ObjectField field, Dictionary<int, string> enums, int editWidth = AbstractFieldController.maxWidth)
		{
			var controller = new EnumFieldController
			{
				Field     = field,
				Label     = this.accessor.GetFieldName (field),
				EditWidth = editWidth,
				Enums     = enums,
				TabIndex  = ++this.tabIndex,
			};

			controller.CreateUI (parent);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldInt (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
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
		}

		protected void CreateDecimalController(Widget parent, ObjectField field, DecimalFormat format)
		{
			var controller = new DecimalFieldController
			{
				Field         = field,
				Label         = this.accessor.GetFieldName (field),
				DecimalFormat = format,
				TabIndex      = ++this.tabIndex,
			};

			controller.CreateUI (parent);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldDecimal (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
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
		}

		protected void CreateComputedAmountController(Widget parent, ObjectField field)
		{
			var controller = new ComputedAmountFieldController
			{
				Field     = field,
				Label     = this.accessor.GetFieldName (field),
				TabIndex  = ++this.tabIndex,
			};

			controller.CreateUI (parent);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldComputedAmount (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
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
		}

		protected void CreateIntController(Widget parent, ObjectField field)
		{
			var controller = new IntFieldController
			{
				Field     = field,
				Label     = this.accessor.GetFieldName (field),
				TabIndex  = ++this.tabIndex,
			};

			controller.CreateUI (parent);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldInt (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
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
		}

		protected void CreateDateController(Widget parent, ObjectField field)
		{
			var controller = new DateFieldController
			{
				Field     = field,
				Label     = this.accessor.GetFieldName (field),
				TabIndex  = ++this.tabIndex,
			};

			controller.CreateUI (parent);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, controller.Value);

				controller.Value         = this.accessor.EditionAccessor.GetFieldDate (of);
				controller.PropertyState = this.GetPropertyState (of);

				this.OnValueEdited (of);
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


		protected Widget CreateScrollable(Widget parent)
		{
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

		protected void CreateRightGrey(Widget parent)
		{
			//	Crée la bande grise à droite, qui prolonge visuellement l'ascenseur.
			new FrameBox
			{
				Parent         = parent,
				Anchor         = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right,
				PreferredWidth = AbstractView.scrollerDefaultBreadth,
				BackColor      = ColorManager.WindowBackgroundColor,
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
			else if (DataAccessor.IsOneShotField (field))
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


		public static AbstractEditorPage CreatePage(DataAccessor accessor, BaseType baseType, BaseType subBaseType, PageType page)
		{
			switch (page)
			{
				case PageType.OneShot:
					return new EditorPageOneShot (accessor, baseType, subBaseType, isTimeless: false);

				case PageType.Summary:
					return new EditorPageSummary (accessor, baseType, subBaseType, isTimeless: false);

				case PageType.Asset:
					return new EditorPageAsset (accessor, baseType, subBaseType, isTimeless: false);

				case PageType.Persons:
					return new EditorPagePersons (accessor, baseType, subBaseType, isTimeless: false);

				case PageType.Values:
					return new EditorPageValues (accessor, baseType, subBaseType, isTimeless: false);

				case PageType.Amortization:
					return new EditorPageAmortization (accessor, baseType, subBaseType, isTimeless: false);

				case PageType.AmortizationPreview:
					return new EditorPageAmortizationPreview (accessor, baseType, subBaseType, isTimeless: false);

				case PageType.Groups:
					return new EditorPageGroups (accessor, baseType, subBaseType, isTimeless: false);

				case PageType.Category:
					return new EditorPageCategory (accessor, baseType, subBaseType, isTimeless: true);

				case PageType.Group:
					return new EditorPageGroup (accessor, baseType, subBaseType, isTimeless: true);

				case PageType.Person:
					return new EditorPagePerson (accessor, baseType, subBaseType, isTimeless: true);

				case PageType.UserFields:
					return new EditorPageUserField (accessor, baseType, subBaseType, isTimeless: true);

				case PageType.Account:
					return new EditorPageAccount (accessor, baseType, subBaseType, isTimeless: true);

				default:
					System.Diagnostics.Debug.Fail ("Unsupported page type");
					return null;
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
			this.ValueEdited.Raise (this, field);
		}

		public event EventHandler<ObjectField> ValueEdited;
		#endregion


		protected readonly DataAccessor				accessor;
		protected readonly BaseType					baseType;
		protected readonly BaseType					subBaseType;
		protected readonly bool						isTimeless;
		private Dictionary<ObjectField, AbstractFieldController> fieldControllers;

		protected Scrollable						scrollable;
		protected Guid								objectGuid;
		protected DataObject						obj;
		protected Timestamp							timestamp;
		protected bool								hasEvent;
		protected EventType							eventType;
		protected int								tabIndex;
	}
}
