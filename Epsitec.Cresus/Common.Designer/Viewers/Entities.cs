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
			this.scrollable.HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.scrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.scrollable.Panel.IsAutoFitting = false;
			this.scrollable.Panel.ContainerLayoutMode = ContainerLayoutMode.None;
			this.scrollable.Panel.SurfaceSize = new Size(1000, 1000);

			this.boxes = new List<Epsitec.Common.Designer.MyWidgets.EntityBox>();

			MyWidgets.EntityBox box1 = new MyWidgets.EntityBox(this.scrollable.Panel);
			box1.Title = "Facture";
			box1.SetContent("Numéro;Client;Articles;Frais de port");
			box1.SetManualBounds(new Rectangle(20+(180+40)*0, 1000-20-150, 180, 150));
			box1.GeometryChanged += new EventHandler(this.HandleBoxGeometryChanged);

			MyWidgets.EntityBox box2 = new MyWidgets.EntityBox(this.scrollable.Panel);
			box2.Title = "Client";
			box2.SetContent("Titre;Nom;Prénom;Adresse;NPA;Ville;Pays");
			box2.SetManualBounds(new Rectangle(20+(180+40)*1, 1000-20-200, 180, 200));
			box2.GeometryChanged += new EventHandler(this.HandleBoxGeometryChanged);

			MyWidgets.EntityBox box3 = new MyWidgets.EntityBox(this.scrollable.Panel);
			box3.Title = "Article";
			box3.SetContent("Désignation;Quantité;Prix");
			box3.SetManualBounds(new Rectangle(20+(180+40)*2, 1000-20-150, 180, 150));
			box3.GeometryChanged += new EventHandler(this.HandleBoxGeometryChanged);

			this.boxes.Add(box1);
			this.boxes.Add(box2);
			this.boxes.Add(box3);

			this.UpdateGeometry();
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


		protected void UpdateGeometry()
		{
			foreach (MyWidgets.EntityBox box in this.boxes)
			{
				Rectangle bounds = box.ActualBounds;
				double top = bounds.Top;
				double h = box.GetBestHeight();
				bounds.Bottom = top-h;
				bounds.Height = h;
				box.SetManualBounds(bounds);
			}
		}


		private void HandleBoxGeometryChanged(object sender)
		{
			this.UpdateGeometry();
		}


		
		List<MyWidgets.EntityBox> boxes;
	}
}
