using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Entities : Abstract2
	{
		public Entities(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base(module, context, access, mainWindow)
		{
			HToolBar header = new HToolBar(this.lastPane);
			header.Dock = DockStyle.Top;
			header.Margins = new Margins(0, 0, 1, 0);

			this.scrollable.HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.scrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.scrollable.Panel.IsAutoFitting = false;
			this.scrollable.Panel.ContainerLayoutMode = ContainerLayoutMode.None;
			this.scrollable.Panel.SurfaceSize = new Size(1000, 1000);

			this.editor = new EntitiesEditor.Editor(this.scrollable.Panel);
			this.editor.SetManualBounds(new Rectangle(Point.Zero, this.scrollable.Panel.SurfaceSize));

			HSlider slider = new HSlider(header);
			slider.MinValue = (decimal) 0.2;
			slider.MaxValue = (decimal) 1.0;
			slider.SmallChange = 0.1M;
			slider.LargeChange = 0.2M;
			slider.Resolution = 0.01M;
			slider.Value = (decimal) this.editor.Zoom;
			slider.PreferredWidth = 80;
			slider.Margins = new Margins(10, 0, 4, 4);
			slider.Dock = DockStyle.Left;
			slider.ValueChanged += new EventHandler(this.HandleSliderValueChanged);

			EntitiesEditor.ObjectBox box;

			box = new EntitiesEditor.ObjectBox(this.editor);
			box.Title = "Facture";
			box.SetContent("Numéro de facture;Date;Client;Articles;TVA;Rabais;Frais de port");
			this.editor.AddBox(box);

			box = new EntitiesEditor.ObjectBox(this.editor);
			box.Title = "Client";
			box.SetContent("Numéro de client;Titre;Nom;Prénom;Entreprise;Adresse;NPA;Ville;Pays;Téléphone professionnel;Téléphone privé;Téléphone mobile;E-mail professionnel;E-mail privé;Site web");
			this.editor.AddBox(box);

			box = new EntitiesEditor.ObjectBox(this.editor);
			box.Title = "Article";
			box.SetContent("Numéro d'article;Désignation;Quantité;Prix d'achat;Prix de vente");
			this.editor.AddBox(box);

			box = new EntitiesEditor.ObjectBox(this.editor);
			box.Title = "Rabais";
			box.SetContent("Normal;Revendeur;Grossiste");
			this.editor.AddBox(box);

			this.editor.AddConnection(new EntitiesEditor.ObjectConnection(this.editor));
			this.editor.AddConnection(new EntitiesEditor.ObjectConnection(this.editor));
			this.editor.AddConnection(new EntitiesEditor.ObjectConnection(this.editor));

			this.editor.UpdateGeometry();
			this.UpdateAll();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}


		public override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Entities;
			}
		}


		private void HandleSliderValueChanged(object sender)
		{
			HSlider slider = sender as HSlider;
			this.editor.Zoom = (double) slider.Value;
		}


		protected EntitiesEditor.Editor editor;
	}
}
