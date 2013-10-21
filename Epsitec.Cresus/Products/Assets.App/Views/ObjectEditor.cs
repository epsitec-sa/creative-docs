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

			this.AddPage (EditionObjectPageType.Summary);

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
			//	une "menu" des pages enfants possibles.
			int count = this.navigatorLevels[rank].ChildrenPages.Count ();

			if (count == 1)
			{
				//	S'il n'y a qu'une page enfant possible, pas besoin de poser la question;
				//	on y va directement.
				var type = this.navigatorLevels[rank].ChildrenPages.First ();
				this.NavigatorGoTo (rank+1, type);
			}
			else if (count > 1)
			{
				//	Cherche s'il existe une page enfant déjà ouverte, pour la mettre
				//	en évidence dans le menu.
				int sel = -1;

				if (rank < this.navigatorLevels.Count-1)
				{
					var np = this.navigatorLevels[rank+1].PageType;
					sel = this.navigatorLevels[rank].ChildrenPages.ToList ().IndexOf (np);
				}

				//	Crée un popup en guise de menu.
				var popup = new SimplePopup
				{
					SelectedItem = sel,
				};

				foreach (var type in this.navigatorLevels[rank].ChildrenPages)
				{
					popup.Items.Add (StaticDescriptions.GetObjectPageDescription (type));
				}

				popup.Create (target);

				popup.ItemClicked += delegate (object sender, int i)
				{
					var type = this.navigatorLevels[rank].ChildrenPages.ElementAt (i);
					this.NavigatorGoTo (rank+1, type);
				};
			}
		}

		private void NavigatorGoBack(int rank)
		{
			//	Revient en arrière à un niveau quelconque.
			var type = this.navigatorLevels[rank].PageType;
			this.NavigatorGoTo (rank, type);
		}

		private void NavigatorGoTo(int rank, EditionObjectPageType type)
		{
			//	Effectue un autre branchement à un niveau quelconque.
			int count = this.navigatorLevels.Count - rank;

			this.navigatorLevels.RemoveRange (rank, count);
			this.navigatorController.Items.RemoveRange (rank, count);

			this.AddPage (type);
		}

		private void AddPage(EditionObjectPageType type)
		{
			//	Ajoute une nouvelle page à la fin de la liste actuelle.
			this.currentPage = AbstractObjectEditorPage.CreatePage (this.accessor, type);
			this.currentPage.SetObject (this.editFrameBox, this.objectGuid, this.timestamp);

			this.currentPage.Navigate += delegate (object sender, Timestamp timestamp)
			{
				this.OnNavigate (timestamp);
			};

			this.currentPage.PageOpen += delegate (object sender, EditionObjectPageType openType)
			{
				this.AddPage (openType);
			};

			var ccpt = this.CurrentChildrenPageTypes.ToArray ();

			this.navigatorLevels.Add (new NavigatorLevel (type, ccpt));

			this.navigatorController.Items.Add (StaticDescriptions.GetObjectPageDescription (type));
			this.navigatorController.Selection = this.navigatorController.Items.Count-1;
			this.navigatorController.HasLastArrow = ccpt.Any ();
			this.navigatorController.UpdateUI ();
		}

		private struct NavigatorLevel
		{
			public NavigatorLevel(EditionObjectPageType pageType, IEnumerable<EditionObjectPageType> childrenPages)
			{
				this.PageType      = pageType;
				this.ChildrenPages = childrenPages;
			}

			public readonly EditionObjectPageType				PageType;
			public readonly IEnumerable<EditionObjectPageType>	ChildrenPages;
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
				this.hasEvent = false;
				this.eventType = EventType.Unknown;
			}
			else
			{
				var ts = timestamp.GetValueOrDefault (new Timestamp (System.DateTime.MaxValue, 0));
				this.hasEvent = this.accessor.HasObjectEvent (this.objectGuid, ts);
				this.eventType = this.accessor.GetObjectEventType (this.objectGuid, this.timestamp).GetValueOrDefault (EventType.Unknown);
			}

			//	Si le type d'événement à changé, il faut réinitialiser le navigateur,
			//	car il permet peut-être d'atteindre des pages interdites.
			if (this.lastEventType != this.eventType)
			{
				this.AdaptPages ();
				this.lastEventType = this.eventType;
			}

			this.currentPage.SetObject (this.editFrameBox, this.objectGuid, this.timestamp);

			this.topTitle.SetTitle (this.EventDescription);
		}

		private void AdaptPages()
		{
			//	Remet à zéro la barre de navigation et affiche la page universelle
			//	du résumé.
			this.navigatorLevels.Clear ();
			this.navigatorController.Items.Clear ();

			this.AddPage (EditionObjectPageType.Summary);
		}

		private IEnumerable<EditionObjectPageType> CurrentChildrenPageTypes
		{
			//	Retourne la liste des pages autorisées en fonction du type de
			//	l'événement courant.
			get
			{
				var types = ObjectEditor.GetAvailablePages (this.hasEvent, this.eventType).ToArray ();
				return this.currentPage.ChildrenPageTypes.Where (x => types.Contains (x));
			}
		}

		public static IEnumerable<EditionObjectPageType> GetAvailablePages(bool hasEvent, EventType type)
		{
			//	Retourne les pages autorisées pour un type d'événement donné.
			yield return EditionObjectPageType.Summary;

			if (hasEvent)
			{
				switch (type)
				{
					case EventType.AmortissementAuto:
					case EventType.AmortissementExtra:
					case EventType.Augmentation:
					case EventType.Diminution:
						yield return EditionObjectPageType.Values;
						break;

					case EventType.Modification:
					case EventType.Réorganisation:
						yield return EditionObjectPageType.General;
						yield return EditionObjectPageType.Amortissements;
						yield return EditionObjectPageType.Compta;
						break;

					default:
						yield return EditionObjectPageType.General;
						yield return EditionObjectPageType.Values;
						yield return EditionObjectPageType.Amortissements;
						yield return EditionObjectPageType.Compta;
						break;
				}
			}
		}


		private string EventDescription
		{
			//	Retourne un texte décrivant l'événement, composé de la date
			//	et du type de l'événement.
			//	Par exemple "31.03.2014 — Amortissement"
			get
			{
				var list = new List<string> ();

				//	Met la date de l'événement, si elle est connue.
				if (this.timestamp.Date != System.DateTime.MaxValue)
				{
					var d = Helpers.Converters.DateToString (this.timestamp.Date);
					list.Add (d);
				}

				//	Met le type de l'événement, s'il est connu.
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

		private NavigatorController					navigatorController;
		private FrameBox							editFrameBox;
		private TopTitle							topTitle;
		private AbstractObjectEditorPage			currentPage;

		private Guid								objectGuid;
		private Timestamp							timestamp;
		private bool								hasEvent;
		private EventType							eventType;
		private EventType							lastEventType;
	}
}
