//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectEditor : AbstractEditor
	{
		public ObjectEditor(DataAccessor accessor, BaseType baseType, bool isTimeless)
			: base (accessor, baseType, isTimeless)
		{
		}


		public override PageType PageType
		{
			get
			{
				return this.currentPageType;
			}
			set
			{
				this.OpenPage (value);
			}
		}


		public override void CreateUI(Widget parent)
		{
			this.topTitle = new TopTitle
			{
				Parent = parent,
			};

			var box = new FrameBox
			{
				Parent    = parent,
				Dock      = DockStyle.Fill,
				BackColor = ColorManager.EditBackgroundColor,
			};

			this.tabPagesController = new TabPagesController ();
			this.tabPagesController.CreateUI (box);

			this.editFrameBox = new FrameBox
			{
				Parent         = box,
				Dock           = DockStyle.Left,
				PreferredWidth = 560,
				Padding        = new Margins (10),
				BackColor      = ColorManager.EditBackgroundColor,
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
			this.OpenPage (this.MainPage);
		}


		private void TabPageClicked(int rank)
		{
			var type = this.AvailablePages.ToArray ()[rank];
			this.OpenPage (type);
		}

		private void OpenPage(PageType type, ObjectField focusedField = ObjectField.Unknown)
		{
			//	Ajoute une nouvelle page à la fin de la liste actuelle.
			if (this.currentPage != null)
			{
				this.currentPage.Dispose ();
				this.currentPage = null;
			}

			this.editFrameBox.Children.Clear ();

			this.currentPage = AbstractEditorPage.CreatePage (this.accessor, this.baseType, type);
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

			this.currentPage.PageOpen += delegate (object sender, PageType openType, ObjectField field)
			{
				this.OpenPage (openType, field);
			};

			this.currentPage.Goto += delegate (object sender, ViewState viewState)
			{
				this.OnGoto (viewState);
			};

			this.UpdateSelectedTabPages (type);
		}


		public void SetObject(Guid objectGuid, Timestamp? timestamp)
		{
			//	Spécifie l'objet sélectionné dans le TreeTable de gauche.
			this.StartEdition (objectGuid, timestamp);

			if (!timestamp.HasValue)
			{
				timestamp = Timestamp.MaxValue;
			}

			this.objectGuid = objectGuid;
			this.obj        = this.accessor.GetObject (this.baseType, this.objectGuid);
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

			//	Il faut réinitialiser le navigateur, car il permet peut-être d'atteindre
			//	des pages interdites.
			this.AdaptPages ();

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


		private void UpdateSelectedTabPages(PageType selectedPage)
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


		private PageType MainPage
		{
			get
			{
				switch (baseType)
				{
					case BaseType.Categories:
						return PageType.Category;

					case BaseType.Groups:
						return PageType.Group;

					case BaseType.Persons:
						return PageType.Person;

					default:
						var pages = ObjectEditor.GetAvailablePages (this.baseType, true, this.eventType).ToArray ();

						if (pages.Length >= 2)
						{
							return pages[2];
						}
						else
						{
							return PageType.OneShot;
						}
				}
			}
		}

		public IEnumerable<PageType> AvailablePages
		{
			get
			{
				return ObjectEditor.GetAvailablePages (this.baseType, this.hasEvent, this.eventType);
			}
		}

		public static IEnumerable<PageType> GetAvailablePages(BaseType baseType, bool hasEvent, EventType type)
		{
			switch (baseType)
			{
				case BaseType.Objects:
					return ObjectEditor.GetObjectAvailablePages (hasEvent, type);

				case BaseType.Categories:
					return ObjectEditor.GetCategoryAvailablePages (hasEvent, type);

				case BaseType.Groups:
					return ObjectEditor.GetGroupAvailablePages (hasEvent, type);

				case BaseType.Persons:
					return ObjectEditor.GetPersonAvailablePages (hasEvent, type);

				default:
					return null;
			}
		}

		private static IEnumerable<PageType> GetObjectAvailablePages(bool hasEvent, EventType type)
		{
			//	Retourne les pages autorisées pour un type d'événement donné.
			yield return PageType.Summary;

			if (hasEvent)
			{
				yield return PageType.OneShot;

				switch (type)
				{
					case EventType.AmortissementAuto:
						break;

					case EventType.AmortissementExtra:
					case EventType.Augmentation:
					case EventType.Diminution:
						yield return PageType.Values;
						yield return PageType.Amortissements;
						break;

					case EventType.Modification:
					case EventType.Réorganisation:
						yield return PageType.Object;
						yield return PageType.Groups;
						break;

					default:  // accès à toutes les pages
						yield return PageType.Object;
						yield return PageType.Persons;
						yield return PageType.Groups;
						yield return PageType.Values;
						yield return PageType.Amortissements;
						break;
				}
			}
		}

		private static IEnumerable<PageType> GetCategoryAvailablePages(bool hasEvent, EventType type)
		{
			//	Retourne les pages autorisées pour un type d'événement donné.
			yield return PageType.Category;
		}

		private static IEnumerable<PageType> GetGroupAvailablePages(bool hasEvent, EventType type)
		{
			//	Retourne les pages autorisées pour un type d'événement donné.
			yield return PageType.Group;
		}

		private static IEnumerable<PageType> GetPersonAvailablePages(bool hasEvent, EventType type)
		{
			//	Retourne les pages autorisées pour un type d'événement donné.
			yield return PageType.Person;
		}


		private string EventDescription
		{
			//	Retourne un texte décrivant l'événement, composé de la date
			//	et du type de l'événement.
			//	Par exemple "Evénement du 31.03.2014 — Amortissement"
			get
			{
				if (this.isTimeless)
				{
					return null;
				}
				else
				{
					return LogicDescriptions.GetEventDescription (this.timestamp, this.eventType);
				}
			}
		}


		#region Events handler
		private void OnNavigate(Timestamp timestamp)
		{
			this.Navigate.Raise (this, timestamp);
		}

		public event EventHandler<Timestamp> Navigate;


		private void OnGoto(ViewState viewState)
		{
			this.Goto.Raise (this, viewState);
		}

		public event EventHandler<ViewState> Goto;
		#endregion


		private TopTitle							topTitle;
		private TabPagesController					tabPagesController;
		private FrameBox							editFrameBox;
		private AbstractEditorPage					currentPage;
		private PageType							currentPageType;

		private Guid								objectGuid;
		private DataObject							obj;
		private Timestamp							timestamp;
		private bool								hasEvent;
		private EventType							eventType;
	}
}
