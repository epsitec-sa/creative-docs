//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectEditor : AbstractEditor
	{
		public ObjectEditor(DataAccessor accessor)
			: base (accessor)
		{
			this.pageTypes = new List<ObjectPageType> ();
		}

		public override void CreateUI(Widget parent)
		{
			this.topTitle = new TopTitle
			{
				Parent = parent,
			};

			var box = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.navigatorController = new NavigatorController ();
			this.navigatorController.CreateUI (box);

			this.editFrameBox = new FrameBox
			{
				Parent    = box,
				Dock      = DockStyle.Fill,
				Padding   = new Margins (10),
				BackColor = ColorManager.EditBackgroundColor,
			};

			this.AddPage (ObjectPageType.Summary);

			this.navigatorController.ItemClicked += delegate (object sender, int rank)
			{
				this.NavigatorGoBack (rank);
			};
		}



		private void NavigatorGoBack(int rank)
		{
			var type = this.pageTypes[rank];

			int count = this.pageTypes.Count - rank;
			this.pageTypes.RemoveRange (rank, count);

			this.navigatorController.Items.RemoveRange (rank, count);

			this.AddPage (type);
		}

		private void AddPage(ObjectPageType type)
		{
			this.currentPage = AbstractObjectEditorPage.CreatePage (this.accessor, type);
			this.currentPage.SetObject (this.editFrameBox, this.objectGuid, this.timestamp);

			this.currentPage.Navigate += delegate (object sender, Timestamp timestamp)
			{
				this.OnNavigate (timestamp);
			};

			this.currentPage.PageOpen += delegate (object sender, ObjectPageType openType)
			{
				this.AddPage (openType);
			};

			this.pageTypes.Add (type);

			this.navigatorController.Items.Add (this.currentPage.PageTitle);
			this.navigatorController.Selection = this.navigatorController.Items.Count-1;
			this.navigatorController.UpdateUI ();
		}



		public void SetObject(Guid objectGuid, Timestamp? timestamp)
		{
			if (timestamp == null || !timestamp.HasValue)
			{
				timestamp = new Timestamp (System.DateTime.MaxValue, 0);
			}

			this.objectGuid = objectGuid;
			this.timestamp = timestamp.Value;

			if (this.objectGuid.IsEmpty)
			{
				this.eventType = EventType.Unknown;
			}
			else
			{
				this.eventType = this.accessor.GetObjectEventType (this.objectGuid, this.timestamp).GetValueOrDefault (EventType.Unknown);
			}

			this.currentPage.SetObject (this.editFrameBox, this.objectGuid, this.timestamp);

			this.topTitle.SetTitle (this.ObjectTitle);
		}


		private string ObjectTitle
		{
			//	Retourne le type de l'événement ainsi que la date.
			get
			{
				var list = new List<string> ();

#if false
				if (this.properties != null)
				{
					var nom = DataAccessor.GetStringProperty (this.properties, (int) ObjectField.Nom);
					if (!string.IsNullOrEmpty (nom))
					{
						list.Add (nom);
					}
				}
#endif

				if (this.timestamp.Date != System.DateTime.MaxValue)
				{
					var d = Helpers.Converters.DateToString (this.timestamp.Date);
					list.Add (d);
				}

				var ed = StaticDescriptions.GetEventDescription (this.eventType);
				if (!string.IsNullOrEmpty (ed))
				{
					list.Add (ed);
				}

				return string.Join (" — ", list);
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
		#endregion


		private readonly List<ObjectPageType>		pageTypes;

		private NavigatorController					navigatorController;
		private FrameBox							editFrameBox;
		private TopTitle							topTitle;
		private AbstractObjectEditorPage			currentPage;

		private Guid								objectGuid;
		private Timestamp							timestamp;
		private EventType							eventType;
	}
}
