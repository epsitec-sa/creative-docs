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
		}


		public virtual IEnumerable<ObjectPageType> ChildrenPageTypes
		{
			get
			{
				return new List<ObjectPageType> ();
			}
		}


		public void SetObject(Widget parent, Guid objectGuid, Timestamp timestamp)
		{
			this.objectGuid = objectGuid;
			this.timestamp = timestamp;

			if (this.objectGuid.IsEmpty)
			{
				this.hasEvent = false;
				this.eventType = EventType.Unknown;
				this.properties = null;
			}
			else
			{
				this.hasEvent = this.accessor.HasObjectEvent (this.objectGuid, this.timestamp);
				this.eventType = this.accessor.GetObjectEventType (this.objectGuid, this.timestamp).GetValueOrDefault (EventType.Unknown);
				this.properties = this.accessor.GetObjectSyntheticProperties (this.objectGuid, this.timestamp);
			}

			parent.Children.Clear ();
			this.CreateUI (parent);
		}

		protected virtual void CreateUI(Widget parent)
		{

		}


		protected void CreateStringController(Widget parent, ObjectField field, int editWidth = 380, int lineCount = 1)
		{
			var controller = new StringFieldController
			{
				Label         = StaticDescriptions.GetObjectFieldDescription (field),
				Value         = DataAccessor.GetStringProperty (this.properties, (int) field),
				PropertyState = this.GetPropertyState (field),
				EditWidth     = editWidth,
				LineCount     = lineCount,
				TabIndex      = this.tabIndex++,
			};

			controller.CreateUI (parent);

			controller.ValueChanged += delegate
			{
			};

			controller.ShowHistory += delegate (object sender, Widget target)
			{
				this.ShowHistoryPopup (target, field);
			};
		}

		protected void CreateDecimalController(Widget parent, ObjectField field, bool isRate = false)
		{
			var controller = new DecimalFieldController
			{
				Label         = StaticDescriptions.GetObjectFieldDescription (field),
				Value         = DataAccessor.GetDecimalProperty (this.properties, (int) field),
				PropertyState = this.GetPropertyState (field),
				IsRate        = isRate,
				TabIndex      = this.tabIndex++,
			};

			controller.CreateUI (parent);

			controller.ValueChanged += delegate
			{
			};

			controller.ShowHistory += delegate (object sender, Widget target)
			{
				this.ShowHistoryPopup (target, field);
			};
		}

		protected void CreateComputedAmountController(Widget parent, ObjectField field)
		{
			var controller = new ComputedAmountFieldController
			{
				Label         = StaticDescriptions.GetObjectFieldDescription (field),
				Value         = DataAccessor.GetComputedAmountProperty (this.properties, (int) field),
				PropertyState = this.GetPropertyState (field),
				TabIndex      = this.tabIndex++,
			};

			controller.CreateUI (parent);

			controller.ValueChanged += delegate
			{
			};

			controller.ShowHistory += delegate (object sender, Widget target)
			{
				this.ShowHistoryPopup (target, field);
			};
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


		public static AbstractObjectEditorPage CreatePage(DataAccessor accessor, ObjectPageType page)
		{
			switch (page)
			{
				case ObjectPageType.Summary:
					return new ObjectEditorPageSummary (accessor);

				case ObjectPageType.Infos:
					return new ObjectEditorPageInfos (accessor);

				case ObjectPageType.Values:
					return new ObjectEditorPageValues (accessor);

				case ObjectPageType.Amortissements:
					return new ObjectEditorPageAmortissements (accessor);

				case ObjectPageType.Compta:
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


		protected void OnPageOpen(ObjectPageType type)
		{
			if (this.PageOpen != null)
			{
				this.PageOpen (this, type);
			}
		}

		public delegate void PageOpenEventHandler(object sender, ObjectPageType type);
		public event PageOpenEventHandler PageOpen;
		#endregion


		protected readonly DataAccessor				accessor;

		protected Guid								objectGuid;
		protected Timestamp							timestamp;
		protected bool								hasEvent;
		protected EventType							eventType;
		protected IEnumerable<AbstractDataProperty>	properties;
		protected int								tabIndex;
	}
}
