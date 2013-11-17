//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractEditorPage
	{
		public AbstractEditorPage(DataAccessor accessor, BaseType baseType)
		{
			this.accessor = accessor;
			this.baseType = baseType;

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
					c.Value         = ObjectCalculator.GetObjectPropertyString (this.obj, this.timestamp, field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is DecimalFieldController)
				{
					var c = controller as DecimalFieldController;

					c.EventType     = this.eventType;
					c.Value         = ObjectCalculator.GetObjectPropertyDecimal (this.obj, this.timestamp, field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is ComputedAmountFieldController)
				{
					var c = controller as ComputedAmountFieldController;

					c.EventType     = this.eventType;
					c.Value         = ObjectCalculator.GetObjectPropertyComputedAmount (this.obj, this.timestamp, field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is IntFieldController)
				{
					var c = controller as IntFieldController;

					c.EventType     = this.eventType;
					c.Value         = ObjectCalculator.GetObjectPropertyInt (this.obj, this.timestamp, field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is DateFieldController)
				{
					var c = controller as DateFieldController;

					c.EventType     = this.eventType;
					c.Value         = ObjectCalculator.GetObjectPropertyDate (this.obj, this.timestamp, field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is GuidFieldController)
				{
					var c = controller as GuidFieldController;

					c.EventType     = this.eventType;
					c.Value         = ObjectCalculator.GetObjectPropertyGuid (this.obj, this.timestamp, field);
					c.PropertyState = this.GetPropertyState (field);
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


		protected void CreateGuidController(Widget parent, ObjectField field)
		{
			var controller = new GuidFieldController
			{
				Accessor  = this.accessor,
				BaseType  = this.baseType,
				Label     = DataDescriptions.GetObjectFieldDescription (field),
				EditWidth = 380,
				TabIndex  = ++this.tabIndex,
			};

			controller.CreateUI (parent);

			controller.ValueEdited += delegate
			{
				this.accessor.SetObjectField (field, controller.Value);

				controller.Value         = ObjectCalculator.GetObjectPropertyGuid (this.obj, this.timestamp, field);
				controller.PropertyState = this.GetPropertyState (field);

				this.OnValueEdited (field);
			};

			controller.ShowHistory += delegate (object sender, Widget target)
			{
				this.ShowHistoryPopup (target, field);
			};

			this.fieldControllers.Add (field, controller);
		}

		protected void CreateStringController(Widget parent, ObjectField field, int editWidth = 380, int lineCount = 1)
		{
			var controller = new StringFieldController
			{
				Label     = DataDescriptions.GetObjectFieldDescription (field),
				EditWidth = editWidth,
				LineCount = lineCount,
				TabIndex  = ++this.tabIndex,
			};

			controller.CreateUI (parent);

			controller.ValueEdited += delegate
			{
				this.accessor.SetObjectField (field, controller.Value);

				controller.Value         = ObjectCalculator.GetObjectPropertyString (this.obj, this.timestamp, field);
				controller.PropertyState = this.GetPropertyState (field);

				this.OnValueEdited (field);
			};

			controller.ShowHistory += delegate (object sender, Widget target)
			{
				this.ShowHistoryPopup (target, field);
			};

			this.fieldControllers.Add (field, controller);
		}

		protected void CreateDecimalController(Widget parent, ObjectField field, DecimalFormat format)
		{
			var controller = new DecimalFieldController
			{
				Label         = DataDescriptions.GetObjectFieldDescription (field),
				DecimalFormat = format,
				TabIndex      = ++this.tabIndex,
			};

			controller.CreateUI (parent);

			controller.ValueEdited += delegate
			{
				this.accessor.SetObjectField (field, controller.Value);

				controller.Value         = ObjectCalculator.GetObjectPropertyDecimal (this.obj, this.timestamp, field);
				controller.PropertyState = this.GetPropertyState (field);

				this.OnValueEdited (field);
			};

			controller.ShowHistory += delegate (object sender, Widget target)
			{
				this.ShowHistoryPopup (target, field);
			};

			this.fieldControllers.Add (field, controller);
		}

		protected void CreateComputedAmountController(Widget parent, ObjectField field)
		{
			var controller = new ComputedAmountFieldController
			{
				Label     = DataDescriptions.GetObjectFieldDescription (field),
				TabIndex  = ++this.tabIndex,
			};

			controller.CreateUI (parent);

			controller.ValueEdited += delegate
			{
				this.accessor.SetObjectField (field, controller.Value);

				controller.Value         = ObjectCalculator.GetObjectPropertyComputedAmount (this.obj, this.timestamp, field);
				controller.PropertyState = this.GetPropertyState (field);

				this.OnValueEdited (field);
			};

			controller.ShowHistory += delegate (object sender, Widget target)
			{
				this.ShowHistoryPopup (target, field);
			};

			this.fieldControllers.Add (field, controller);
		}

		protected void CreateIntController(Widget parent, ObjectField field)
		{
			var controller = new IntFieldController
			{
				Label     = DataDescriptions.GetObjectFieldDescription (field),
				TabIndex  = ++this.tabIndex,
			};

			controller.CreateUI (parent);

			controller.ValueEdited += delegate
			{
				this.accessor.SetObjectField (field, controller.Value);

				controller.Value         = ObjectCalculator.GetObjectPropertyInt (this.obj, this.timestamp, field);
				controller.PropertyState = this.GetPropertyState (field);

				this.OnValueEdited (field);
			};

			controller.ShowHistory += delegate (object sender, Widget target)
			{
				this.ShowHistoryPopup (target, field);
			};

			this.fieldControllers.Add (field, controller);
		}

		protected void CreateDateController(Widget parent, ObjectField field)
		{
			var controller = new DateFieldController
			{
				Label     = DataDescriptions.GetObjectFieldDescription (field),
				TabIndex  = ++this.tabIndex,
			};

			controller.CreateUI (parent);

			controller.ValueEdited += delegate
			{
				this.accessor.SetObjectField (field, controller.Value);

				controller.Value         = ObjectCalculator.GetObjectPropertyDate (this.obj, this.timestamp, field);
				controller.PropertyState = this.GetPropertyState (field);

				this.OnValueEdited (field);
			};

			controller.ShowHistory += delegate (object sender, Widget target)
			{
				this.ShowHistoryPopup (target, field);
			};

			this.fieldControllers.Add (field, controller);
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
			if (DataAccessor.IsOneShotField (field))
			{
				return PropertyState.OneShot;
			}
			else if (this.hasEvent)
			{
				var p = ObjectCalculator.GetObjectSyntheticProperty (this.obj, this.timestamp, field);

				if (p == null)
				{
					return PropertyState.Synthetic;
				}
				else
				{
					return p.State;
				}
			}
			else
			{
				return PropertyState.Readonly;
			}
		}


		public static AbstractEditorPage CreatePage(DataAccessor accessor, BaseType baseType, EditionObjectPageType page)
		{
			switch (page)
			{
				case EditionObjectPageType.OneShot:
					return new EditorPageOneShot (accessor, baseType);

				case EditionObjectPageType.Summary:
					return new EditorPageSummary (accessor, baseType);

				case EditionObjectPageType.Grouping:
					return new EditorPageGrouping (accessor, baseType);

				case EditionObjectPageType.Object:
					return new EditorPageObject (accessor, baseType);

				case EditionObjectPageType.Values:
					return new EditorPageValues (accessor, baseType);

				case EditionObjectPageType.Amortissements:
					return new EditorPageAmortissements (accessor, baseType);

				case EditionObjectPageType.Compta:
					return new EditorPageCompta (accessor, baseType);

				case EditionObjectPageType.Category:
					return new EditorPageCategory (accessor, baseType);

				case EditionObjectPageType.Group:
					return new EditorPageGroup (accessor, baseType);

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


		protected void OnPageOpen(EditionObjectPageType type, ObjectField field)
		{
			this.PageOpen.Raise (this, type, field);
		}

		public event EventHandler<EditionObjectPageType, ObjectField> PageOpen;


		protected void OnValueEdited(ObjectField field)
		{
			this.ValueEdited.Raise (this, field);
		}

		public event EventHandler<ObjectField> ValueEdited;
		#endregion


		protected readonly DataAccessor				accessor;
		protected readonly BaseType					baseType;
		private Dictionary<ObjectField, AbstractFieldController> fieldControllers;

		protected Guid								objectGuid;
		protected DataObject						obj;
		protected Timestamp							timestamp;
		protected bool								hasEvent;
		protected EventType							eventType;
		protected int								tabIndex;
	}
}
