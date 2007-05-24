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

			this.boxes = new List<MyWidgets.EntityBox>();
			this.links = new List<MyWidgets.EntityLink>();

			MyWidgets.EntityBox box1 = new MyWidgets.EntityBox(this.scrollable.Panel);
			box1.Title = "Facture";
			box1.SetContent("Numéro;Client;Articles;TVA;Rabais;Frais de port");
			box1.SetManualBounds(new Rectangle(20+(180+40)*0, 1000-20-100, 180, 100));
			box1.GeometryChanged += new EventHandler(this.HandleBoxGeometryChanged);

			MyWidgets.EntityBox box2 = new MyWidgets.EntityBox(this.scrollable.Panel);
			box2.Title = "Client";
			box2.SetContent("Numéro;Titre;Nom;Prénom;Entreprise;Adresse;NPA;Ville;Pays;Téléphone professionnel;Téléphone privé;Téléphone mobile;E-mail professionnel;E-mail privé;Site web");
			box2.SetManualBounds(new Rectangle(20+(180+40)*1, 1000-20-100, 180, 100));
			box2.GeometryChanged += new EventHandler(this.HandleBoxGeometryChanged);

			MyWidgets.EntityBox box3 = new MyWidgets.EntityBox(this.scrollable.Panel);
			box3.Title = "Article";
			box3.SetContent("Numéro;Désignation;Quantité;Prix");
			box3.SetManualBounds(new Rectangle(20+(180+40)*2, 1000-20-100, 180, 100));
			box3.GeometryChanged += new EventHandler(this.HandleBoxGeometryChanged);

			this.boxes.Add(box1);
			this.boxes.Add(box2);
			this.boxes.Add(box3);

			MyWidgets.EntityLink link1 = new MyWidgets.EntityLink(this.scrollable.Panel);
			MyWidgets.EntityLink link2 = new MyWidgets.EntityLink(this.scrollable.Panel);

			this.links.Add(link1);
			this.links.Add(link2);

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
			//	Met à jour la géométrie de toutes les boîtes et de toutes les liaisons.
			foreach (MyWidgets.EntityBox box in this.boxes)
			{
				Rectangle bounds = box.ActualBounds;
				double top = bounds.Top;
				double h = box.GetBestHeight();
				bounds.Bottom = top-h;
				bounds.Height = h;
				box.SetManualBounds(bounds);
			}

			this.UpdateLink(this.links[0], this.boxes[0], 1, this.boxes[1], Relation.Reference);  // lien client
			this.UpdateLink(this.links[1], this.boxes[0], 2, this.boxes[2], Relation.Collection);  // lien articles
		}

		protected void UpdateLink(MyWidgets.EntityLink link, MyWidgets.EntityBox src, int srcRank, MyWidgets.EntityBox dst, Relation relation)
		{
			//	Met à jour la géométrie d'une liaison.
			link.SetManualBounds(this.scrollable.Panel.Client.Bounds);
			link.Relation = relation;

			Rectangle srcBounds = src.ActualBounds;
			Rectangle dstBounds = dst.ActualBounds;

			double v = src.GetLinkVerticalPosition(srcRank);
			if (double.IsNaN(v))
			{
				link.Visibility = false;
			}
			else
			{
				link.Visibility = true;

				Point p = new Point(0, v);
				p = src.MapClientToParent(p);

				double dv = dst.GetLinkVerticalDestination(p.Y);

				if (srcBounds.Right < dstBounds.Left)
				{
					link.Source = new Point(srcBounds.Right, p.Y);
					link.Destination = new Point(dstBounds.Left, dv);
				}
				else
				{
					link.Source = new Point(srcBounds.Left, p.Y);
					link.Destination = new Point(dstBounds.Right, dv);
				}
			}
		}


		private void HandleBoxGeometryChanged(object sender)
		{
			//	Appelé lorsque la géométrie d'une boîte a changé (changement compact/étendu).
			this.UpdateGeometry();
		}


		
		List<MyWidgets.EntityBox> boxes;
		List<MyWidgets.EntityLink> links;
	}
}
