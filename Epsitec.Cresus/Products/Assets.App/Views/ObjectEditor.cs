//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

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

		public override void OpenMainPage(EventType eventType)
		{
			//	Après la création d'un événement, on cherche à ouvrir la page la
			//	plus pertinente.
			var pages = ObjectEditor.GetAvailablePages (true, eventType).ToArray ();
			if (pages.Length >= 2)
			{
				this.AddPage (pages[1]);
			}
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

		private void AddPage(EditionObjectPageType type, ObjectField focusedField = ObjectField.Unknown)
		{
			//	Ajoute une nouvelle page à la fin de la liste actuelle.
			if (this.currentPage != null)
			{
				this.currentPage.Dispose ();
				this.currentPage = null;
			}

			this.editFrameBox.Children.Clear ();

			this.currentPage = AbstractObjectEditorPage.CreatePage (this.accessor, type);
			this.currentPage.CreateUI (this.editFrameBox);
			this.currentPage.SetObject (this.accessor.GetObject (this.objectGuid), this.objectGuid, this.timestamp);
			this.currentPage.SetFocus (focusedField);

			this.currentPage.ValueEdited += delegate (object sender, ObjectField field)
			{
				this.SetEditionDirty (field);
			};

			this.currentPage.Navigate += delegate (object sender, Timestamp timestamp)
			{
				this.OnNavigate (timestamp);
			};

			this.currentPage.PageOpen += delegate (object sender, EditionObjectPageType openType, ObjectField field)
			{
				this.AddPage (openType, field);
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
			this.StartEdition (objectGuid, timestamp);

			if (timestamp == null || !timestamp.HasValue)
			{
				timestamp = new Timestamp (System.DateTime.MaxValue, 0);
			}

			this.objectGuid = objectGuid;
			this.timestamp  = timestamp.Value;
			this.hasEvent   = false;
			this.eventType  = EventType.Unknown;

			var obj = this.accessor.GetObject (this.objectGuid);
			if (obj != null)
			{
				var e = obj.GetEvent (this.timestamp);
				if (e != null)
				{
					this.hasEvent  = true;
					this.eventType = e.Type;
				}
			}

			//	Si le type d'événement à changé, il faut réinitialiser le navigateur,
			//	car il permet peut-être d'atteindre des pages interdites.
			if (this.lastEventType != this.eventType)
			{
				this.AdaptPages ();
				this.lastEventType = this.eventType;
			}

			this.currentPage.SetObject (this.accessor.GetObject (this.objectGuid), this.objectGuid, this.timestamp);

			this.topTitle.SetTitle (this.EventDescription);
		}

		private void AdaptPages()
		{
			//	Remet à zéro la barre de navigation et affiche la page universelle
			//	du résumé.
			var currentPageType = this.navigatorLevels.Last ().PageType;

			this.navigatorLevels.Clear ();
			this.navigatorController.Items.Clear ();

			this.AddPage (EditionObjectPageType.Summary);

			//	Si l'ancienne page est compatible avec le nouveau type d'événement,
			//	on l'ouvre.
			if (this.CurrentChildrenPageTypes.Contains (currentPageType))
			{
				this.AddPage (currentPageType);
			}
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
						break;

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

					default:  // accès à toutes les pages
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
			//	Par exemple "Evénement du 31.03.2014 — Amortissement"
			get
			{
				return BusinessLogic.GetEventDescription(this.timestamp, this.eventType);
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

		private TopTitle							topTitle;
		private NavigatorController					navigatorController;
		private FrameBox							editFrameBox;
		private AbstractObjectEditorPage			currentPage;

		private Guid								objectGuid;
		private Timestamp							timestamp;
		private bool								hasEvent;
		private EventType							eventType;
		private EventType							lastEventType;
	}
}
