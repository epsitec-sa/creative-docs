//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractObjectEditorPage
	{
		public AbstractObjectEditorPage(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.fieldControllers = new Dictionary<ObjectField, AbstractFieldController> ();
		}

		public void Dispose()
		{
		}


		public virtual IEnumerable<EditionObjectPageType> ChildrenPageTypes
		{
			get
			{
				return new List<EditionObjectPageType> ();
			}
		}


		public virtual void CreateUI(Widget parent)
		{
		}

		public virtual void SetObject(Guid objectGuid, Timestamp timestamp)
		{
			this.objectGuid = objectGuid;
			this.timestamp = timestamp;

			if (this.objectGuid.IsEmpty)
			{
				this.hasEvent   = false;
				this.eventType  = EventType.Unknown;
				this.properties = null;
			}
			else
			{
				this.hasEvent   = this.accessor.HasObjectEvent (this.objectGuid, this.timestamp);
				this.eventType  = this.accessor.GetObjectEventType (this.objectGuid, this.timestamp).GetValueOrDefault (EventType.Unknown);
				this.properties = this.accessor.GetObjectSyntheticProperties (this.objectGuid, this.timestamp);
			}

			foreach (var pair in this.fieldControllers)
			{
				var field      = pair.Key;
				var controller = pair.Value;

				if (controller is StringFieldController)
				{
					var c = controller as StringFieldController;

					c.Value         = DataAccessor.GetStringProperty (this.properties, (int) field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is DecimalFieldController)
				{
					var c = controller as DecimalFieldController;

					c.Value         = DataAccessor.GetDecimalProperty (this.properties, (int) field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is ComputedAmountFieldController)
				{
					var c = controller as ComputedAmountFieldController;

					c.Value         = DataAccessor.GetComputedAmountProperty (this.properties, (int) field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is IntFieldController)
				{
					var c = controller as IntFieldController;

					c.Value         = DataAccessor.GetIntProperty (this.properties, (int) field);
					c.PropertyState = this.GetPropertyState (field);
				}
				else if (controller is DateFieldController)
				{
					var c = controller as DateFieldController;

					c.Value         = DataAccessor.GetDateProperty (this.properties, (int) field);
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

				controller.Value         = DataAccessor.GetStringProperty (this.properties, (int) field);
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

				controller.Value         = DataAccessor.GetDecimalProperty (this.properties, (int) field);
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
				Label    = StaticDescriptions.GetObjectFieldDescription (field),
				TabIndex = ++this.tabIndex,
			};

			controller.CreateUI (parent);

			controller.ValueEdited += delegate
			{
				this.accessor.SetObjectField (field, controller.Value);

				controller.Value         = DataAccessor.GetComputedAmountProperty (this.properties, (int) field);
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
				Label    = StaticDescriptions.GetObjectFieldDescription (field),
				TabIndex = ++this.tabIndex,
			};

			controller.CreateUI (parent);

			controller.ValueEdited += delegate
			{
				this.accessor.SetObjectField (field, controller.Value);

				controller.Value         = DataAccessor.GetIntProperty (this.properties, (int) field);
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
				Label    = StaticDescriptions.GetObjectFieldDescription (field),
				TabIndex = ++this.tabIndex,
			};

			controller.CreateUI (parent);

			controller.ValueEdited += delegate
			{
				this.accessor.SetObjectField (field, controller.Value);

				controller.Value         = DataAccessor.GetDateProperty (this.properties, (int) field);
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
			var popup = new HistoryPopup (this.accessor, this.objectGuid, this.timestamp, (int) field);

			popup.Create (target, leftOrRight: true);

			popup.Navigate += delegate (object sender, Timestamp timestamp)
			{
				this.OnNavigate (timestamp);
			};
		}

		protected PropertyState GetPropertyState(ObjectField field)
		{
			if (this.hasEvent)
			{
				return DataAccessor.GetPropertyState (this.properties, (int) field);
			}
			else
			{
				return PropertyState.Readonly;
			}
		}


		public static AbstractObjectEditorPage CreatePage(DataAccessor accessor, EditionObjectPageType page)
		{
			switch (page)
			{
				case EditionObjectPageType.Summary:
					return new ObjectEditorPageSummary (accessor);

				case EditionObjectPageType.General:
					return new ObjectEditorPageGeneral (accessor);

				case EditionObjectPageType.Values:
					return new ObjectEditorPageValues (accessor);

				case EditionObjectPageType.Amortissements:
					return new ObjectEditorPageAmortissements (accessor);

				case EditionObjectPageType.Compta:
					return new ObjectEditorPageCompta (accessor);

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
		private Dictionary<ObjectField, AbstractFieldController> fieldControllers;

		protected Guid								objectGuid;
		protected Timestamp							timestamp;
		protected bool								hasEvent;
		protected EventType							eventType;
		protected IEnumerable<AbstractDataProperty>	properties;
		protected int								tabIndex;
	}
}
