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
	public class CategoryEditor : AbstractEditor
	{
		public CategoryEditor(DataAccessor accessor, BaseType baseType)
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

			this.navigatorController = new NavigatorController ();
			this.navigatorController.CreateUI (box);

			this.editFrameBox = new FrameBox
			{
				Parent    = box,
				Dock      = DockStyle.Fill,
				Padding   = new Margins (10),
				BackColor = ColorManager.EditBackgroundColor,
			};

			this.categoryPage = new ObjectEditorPageCategory (this.accessor);
			this.categoryPage.CreateUI (this.editFrameBox);
			this.categoryPage.SetObject (this.catGuid, this.timestamp);
			this.categoryPage.SetFocus (ObjectField.Nom);

			this.categoryPage.ValueEdited += delegate (object sender, ObjectField field)
			{
				this.SetEditionDirty (field);
			};

		}



		public void SetCategory(Guid catGuid, Timestamp? timestamp)
		{
			//	Spécifie la catégorie sélectionnée dans le TreeTable de gauche.
			this.StartEdition (catGuid, timestamp);

			if (timestamp == null || !timestamp.HasValue)
			{
				timestamp = new Timestamp (System.DateTime.MaxValue, 0);
			}

			this.catGuid    = catGuid;
			this.timestamp  = timestamp.Value;

			this.categoryPage.SetObject (this.catGuid, this.timestamp);

			this.topTitle.SetTitle (this.CategoryDescription);
		}


		private string CategoryDescription
		{
			get
			{
				var cat = this.accessor.GetObject (BaseType.Categories, this.catGuid);

				if (cat == null)
				{
					return null;
				}
				else
				{
					return ObjectCalculator.GetObjectPropertyString (cat, null, ObjectField.Nom);
				}
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
		private NavigatorController					navigatorController;
		private FrameBox							editFrameBox;
		private ObjectEditorPageCategory			categoryPage;

		private Guid								catGuid;
		private Timestamp							timestamp;
	}
}
