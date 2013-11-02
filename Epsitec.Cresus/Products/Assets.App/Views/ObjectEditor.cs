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
		public ObjectEditor(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
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

			this.tabPagesController = new TabPagesController ();
			this.tabPagesController.CreateUI (box);

			this.editFrameBox = new FrameBox
			{
				Parent    = box,
				Dock      = DockStyle.Fill,
				Padding   = new Margins (10),
				BackColor = ColorManager.EditBackgroundColor,
			};

			this.OpenPage (this.AvailablePages.First ());

			this.tabPagesController.ItemClicked += delegate (object sender, int rank)
			{
				this.TabPageClicked (rank);
			};
		}

		public override void OpenMainPage(EventType eventType)
		{
			//	Après la création d'un événement, on cherche à ouvrir la page la
			//	plus pertinente.
			this.eventType = eventType;
			this.OpenPage (this.AvailablePages.First ());
		}


		private void TabPageClicked(int rank)
		{
			var type = this.AvailablePages.ToArray ()[rank];
			this.OpenPage (type);
		}

		private void OpenPage(EditionObjectPageType type, ObjectField focusedField = ObjectField.Unknown)
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
			this.currentPage.SetObject (this.objectGuid, this.timestamp);
			this.currentPage.SetFocus (focusedField);

			this.currentPageType = type;

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
				this.OpenPage (openType, field);
			};

			this.UpdateSelectedTabPages (type);
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

			var obj = this.accessor.GetObject (this.baseType, this.objectGuid);
			if (obj != null)
			{
				var e = obj.GetEvent (this.timestamp);
				if (e != null)
				{
					this.hasEvent  = true;
					this.eventType = e.Type;
				}
			}

			this.UpdateTabPages ();

			//	Si le type d'événement à changé, il faut réinitialiser le navigateur,
			//	car il permet peut-être d'atteindre des pages interdites.
			if (this.lastEventType != this.eventType)
			{
				this.AdaptPages ();
				this.lastEventType = this.eventType;
			}

			this.currentPage.SetObject (this.objectGuid, this.timestamp);

			this.topTitle.SetTitle (this.EventDescription);
		}

		private void AdaptPages()
		{
			if (!this.AvailablePages.Contains (this.currentPageType))
			{
				this.OpenPage (this.AvailablePages.First ());
			}

			this.UpdateSelectedTabPages (this.currentPageType);
		}


		private void UpdateSelectedTabPages(EditionObjectPageType selectedPage)
		{
			int sel = this.AvailablePages.ToList ().IndexOf (selectedPage);
			this.tabPagesController.Selection = sel;
		}

		private void UpdateTabPages()
		{
			this.tabPagesController.Items.Clear ();

			foreach (var page in this.AvailablePages)
			{
				var text = StaticDescriptions.GetObjectPageDescription (page);
				this.tabPagesController.Items.Add (text);
			}

			this.tabPagesController.UpdateUI ();
		}


		public IEnumerable<EditionObjectPageType> AvailablePages
		{
			get
			{
				switch (this.baseType)
				{
					case BaseType.Objects:
						return ObjectEditor.GetObjectAvailablePages (this.hasEvent, this.eventType);

					case BaseType.Categories:
						return ObjectEditor.GetCategoryAvailablePages (this.hasEvent, this.eventType);

					default:
						return null;
				}
			}
		}

		public static IEnumerable<EditionObjectPageType> GetObjectAvailablePages(bool hasEvent, EventType type)
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
						yield return EditionObjectPageType.Amortissements;
						break;

					case EventType.Modification:
					case EventType.Réorganisation:
						yield return EditionObjectPageType.General;
						break;

					default:  // accès à toutes les pages
						yield return EditionObjectPageType.General;
						yield return EditionObjectPageType.Values;
						yield return EditionObjectPageType.Amortissements;
						break;
				}
			}
		}

		private static IEnumerable<EditionObjectPageType> GetCategoryAvailablePages(bool hasEvent, EventType type)
		{
			//	Retourne les pages autorisées pour un type d'événement donné.
			yield return EditionObjectPageType.Category;
			yield return EditionObjectPageType.Compta;
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


		private TopTitle							topTitle;
		private TabPagesController					tabPagesController;
		private FrameBox							editFrameBox;
		private AbstractObjectEditorPage			currentPage;
		private EditionObjectPageType				currentPageType;

		private Guid								objectGuid;
		private Timestamp							timestamp;
		private bool								hasEvent;
		private EventType							eventType;
		private EventType							lastEventType;
	}
}
