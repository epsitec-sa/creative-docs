//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectEditor : AbstractEditor
	{
		public ObjectEditor(DataAccessor accessor)
			: base (accessor)
		{
			this.navigatorLevels = new List<NavigatorLevel> ();
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

			this.navigatorController.ArrowClicked += delegate (object sender, Widget button, int rank)
			{
				this.NavigatorArrowClicked (button, rank);
			};
		}


		private void NavigatorArrowClicked(Widget target, int rank)
		{
			//	On a cliqué sur un triangle ">" dans le navigateur. Il faut proposer
			//	une "menu" des enfants possibles.
			int count = this.navigatorLevels[rank].Childrens.Count ();

			if (count == 1)
			{
				//	S'il n'y a qu'un enfant possible, pas besoin de poser la question;
				//	on y va directement.
				var type = this.navigatorLevels[rank].Childrens.First ();
				this.NavigatorGoTo (rank+1, type);
			}
			else if (count > 1)
			{
				//	Cherche s'il existe un enfant déjà ouvert, pour le mettre en évidence
				//	dans le menu.
				int sel = -1;

				if (rank < this.navigatorLevels.Count-1)
				{
					var np = this.navigatorLevels[rank+1].Type;
					sel = this.navigatorLevels[rank].Childrens.ToList ().IndexOf (np);
				}

				//	Crée un popup en guise de menu.
				var popup = new SimplePopup
				{
					Selected = sel,
				};

				foreach (var type in this.navigatorLevels[rank].Childrens)
				{
					popup.Items.Add (StaticDescriptions.GetObjectPageDescription (type));
				}

				popup.Create (target);

				popup.ItemClicked += delegate (object sender, int i)
				{
					var type = this.navigatorLevels[rank].Childrens.ElementAt (i);
					this.NavigatorGoTo (rank+1, type);
				};
			}
		}

		private void NavigatorGoBack(int rank)
		{
			//	Revient en arrière à un niveau quelconque.
			var type = this.navigatorLevels[rank].Type;
			this.NavigatorGoTo (rank, type);
		}

		private void NavigatorGoTo(int rank, ObjectPageType type)
		{
			//	Effectue un autre branchement à un niveau quelconque.
			int count = this.navigatorLevels.Count - rank;

			this.navigatorLevels.RemoveRange (rank, count);
			this.navigatorController.Items.RemoveRange (rank, count);

			this.AddPage (type);
		}

		private void AddPage(ObjectPageType type)
		{
			//	Ajoute une nouvelle page à la fin de la liste actuelle.
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

			this.navigatorLevels.Add (new NavigatorLevel (type, this.currentPage.ChildrenPageTypes));

			this.navigatorController.Items.Add (StaticDescriptions.GetObjectPageDescription (type));
			this.navigatorController.Selection = this.navigatorController.Items.Count-1;
			this.navigatorController.HasLastArrow = this.currentPage.ChildrenPageTypes.Any ();
			this.navigatorController.UpdateUI ();
		}

		private struct NavigatorLevel
		{
			public NavigatorLevel(ObjectPageType type, IEnumerable<ObjectPageType> childrens)
			{
				this.Type      = type;
				this.Childrens = childrens;
			}

			public readonly ObjectPageType					Type;
			public readonly IEnumerable<ObjectPageType>		Childrens;
		}


		public void SetObject(Guid objectGuid, Timestamp? timestamp)
		{
			//	Spécifie l'objet sélectionné dans le TreeTable de gauche.
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


		private readonly List<NavigatorLevel>		navigatorLevels;
		//?private readonly List<ObjectPageType>		pageTypes;
		//?private readonly List<IEnumerable<ObjectPageType>>	childrenPageTypes;

		private NavigatorController					navigatorController;
		private FrameBox							editFrameBox;
		private TopTitle							topTitle;
		private AbstractObjectEditorPage			currentPage;

		private Guid								objectGuid;
		private Timestamp							timestamp;
		private EventType							eventType;
	}
}
