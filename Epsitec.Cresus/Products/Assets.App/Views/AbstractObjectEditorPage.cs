//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractObjectEditorPage
	{
		public AbstractObjectEditorPage(DataAccessor accessor, BaseType baseType)
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
			this.isLocked   = true;
			this.eventType  = EventType.Unknown;

			if (!this.objectGuid.IsEmpty && this.obj != null)
			{
				var e = this.obj.GetEvent (this.timestamp);

				if (e == null)
				{
					this.isLocked = ObjectCalculator.IsEventLocked (this.obj, this.timestamp);
				}
				else
				{
					this.eventType = e.Type;
					this.hasEvent  = true;
					this.isLocked  = false;
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
					c.IsLocked      = this.isLocked;
					c.Value         = ObjectCalculator.GetObjectPropertyString (this.obj, this.timestamp, field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is DecimalFieldController)
				{
					var c = controller as DecimalFieldController;

					c.EventType     = this.eventType;
					c.IsLocked      = this.isLocked;
					c.Value         = ObjectCalculator.GetObjectPropertyDecimal (this.obj, this.timestamp, field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is ComputedAmountFieldController)
				{
					var c = controller as ComputedAmountFieldController;

					c.EventType     = this.eventType;
					c.IsLocked      = this.isLocked;
					c.Value         = ObjectCalculator.GetObjectPropertyComputedAmount (this.obj, this.timestamp, field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is IntFieldController)
				{
					var c = controller as IntFieldController;

					c.EventType     = this.eventType;
					c.IsLocked      = this.isLocked;
					c.Value         = ObjectCalculator.GetObjectPropertyInt (this.obj, this.timestamp, field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is DateFieldController)
				{
					var c = controller as DateFieldController;

					c.EventType     = this.eventType;
					c.IsLocked      = this.isLocked;
					c.Value         = ObjectCalculator.GetObjectPropertyDate (this.obj, this.timestamp, field);
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


		protected void CreateStringController(Widget parent, ObjectField field, int editWidth = 380, int lineCount = 1)
		{
			var controller = new StringFieldController
			{
				Label     = StaticDescriptions.GetObjectFieldDescription (field),
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
				Label         = StaticDescriptions.GetObjectFieldDescription (field),
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
				Label     = StaticDescriptions.GetObjectFieldDescription (field),
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
				Label     = StaticDescriptions.GetObjectFieldDescription (field),
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
				Label     = StaticDescriptions.GetObjectFieldDescription (field),
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
			if (DataAccessor.IsSingletonField (field))
			{
				return PropertyState.Singleton;
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


		public static AbstractObjectEditorPage CreatePage(DataAccessor accessor, BaseType baseType, EditionObjectPageType page)
		{
			switch (page)
			{
				case EditionObjectPageType.Singleton:
					return new ObjectEditorPageSingleton (accessor, baseType);

				case EditionObjectPageType.Summary:
					return new ObjectEditorPageSummary (accessor, baseType);

				case EditionObjectPageType.General:
					return new ObjectEditorPageGeneral (accessor, baseType);

				case EditionObjectPageType.Values:
					return new ObjectEditorPageValues (accessor, baseType);

				case EditionObjectPageType.Amortissements:
					return new ObjectEditorPageAmortissements (accessor, baseType);

				case EditionObjectPageType.Compta:
					return new ObjectEditorPageCompta (accessor, baseType);

				case EditionObjectPageType.Category:
					return new ObjectEditorPageCategory (accessor, baseType);

				default:
					System.Diagnostics.Debug.Fail ("Unsupported page type");
					return null;
			}
		}


		#region Events handler
		private void OnNavigate(Timestamp timestamp)
		{
			if (this.Navigate != null)
			{
				this.Navigate (this, timestamp);
			}
		}

		public delegate void NavigateEventHandler(object sender, Timestamp timestamp);
		public event NavigateEventHandler Navigate;


		protected void OnPageOpen(EditionObjectPageType type, ObjectField field)
		{
			if (this.PageOpen != null)
			{
				this.PageOpen (this, type, field);
			}
		}

		public delegate void PageOpenEventHandler(object sender, EditionObjectPageType type, ObjectField field);
		public event PageOpenEventHandler PageOpen;


		protected void OnValueEdited(ObjectField field)
		{
			if (this.ValueEdited != null)
			{
				this.ValueEdited (this, field);
			}
		}

		public delegate void ValueEditedEventHandler(object sender, ObjectField field);
		public event ValueEditedEventHandler ValueEdited;
		#endregion


		protected readonly DataAccessor				accessor;
		protected readonly BaseType					baseType;
		private Dictionary<ObjectField, AbstractFieldController> fieldControllers;

		protected Guid								objectGuid;
		protected DataObject						obj;
		protected Timestamp							timestamp;
		protected bool								hasEvent;
		protected bool								isLocked;
		protected EventType							eventType;
		protected int								tabIndex;
	}
}
