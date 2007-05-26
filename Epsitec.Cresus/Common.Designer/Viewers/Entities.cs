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

#if true
			this.editor = new EntitiesEditor.Editor(this.scrollable.Panel);
			this.editor.SetManualBounds(new Rectangle(Point.Zero, this.scrollable.Panel.SurfaceSize));

			EntitiesEditor.ObjectBox box;

			box = new EntitiesEditor.ObjectBox();
			box.Title = "Facture";
			box.SetContent("Numéro de facture;Date;Client;Articles;TVA;Rabais;Frais de port");
			this.editor.AddBox(box);

			box = new EntitiesEditor.ObjectBox();
			box.Title = "Client";
			box.SetContent("Numéro de client;Titre;Nom;Prénom;Entreprise;Adresse;NPA;Ville;Pays;Téléphone professionnel;Téléphone privé;Téléphone mobile;E-mail professionnel;E-mail privé;Site web");
			this.editor.AddBox(box);

			box = new EntitiesEditor.ObjectBox();
			box.Title = "Article";
			box.SetContent("Numéro d'article;Désignation;Quantité;Prix d'achat;Prix de vente");
			this.editor.AddBox(box);

			box = new EntitiesEditor.ObjectBox();
			box.Title = "Rabais";
			box.SetContent("Normal;Revendeur;Grossiste");
			this.editor.AddBox(box);

			this.editor.AddConnection(new EntitiesEditor.ObjectConnection());
			this.editor.AddConnection(new EntitiesEditor.ObjectConnection());
			this.editor.AddConnection(new EntitiesEditor.ObjectConnection());

			this.editor.UpdateGeometry();
			this.UpdateAll();
#endif

#if false
			this.editor = new MyWidgets.EntityEditor(this.scrollable.Panel);
			//?this.editor.Dock = DockStyle.Fill;  // TODO: pourquoi ça ne marche pas ???
			this.editor.SetManualBounds(new Rectangle(Point.Zero, this.scrollable.Panel.SurfaceSize));

			MyWidgets.EntityBox box1 = new MyWidgets.EntityBox();
			box1.Title = "Facture";
			box1.SetContent("Numéro de facture;Date;Client;Articles;TVA;Rabais;Frais de port");
			this.editor.AddBox(box1);

			MyWidgets.EntityBox box2 = new MyWidgets.EntityBox();
			box2.Title = "Client";
			box2.SetContent("Numéro de client;Titre;Nom;Prénom;Entreprise;Adresse;NPA;Ville;Pays;Téléphone professionnel;Téléphone privé;Téléphone mobile;E-mail professionnel;E-mail privé;Site web");
			this.editor.AddBox(box2);

			MyWidgets.EntityBox box3 = new MyWidgets.EntityBox();
			box3.Title = "Article";
			box3.SetContent("Numéro d'article;Désignation;Quantité;Prix d'achat;Prix de vente");
			this.editor.AddBox(box3);

			MyWidgets.EntityBox box4 = new MyWidgets.EntityBox();
			box4.Title = "Rabais";
			box4.SetContent("Normal;Revendeur;Grossiste");
			this.editor.AddBox(box4);

			MyWidgets.EntityConnection connection1 = new MyWidgets.EntityConnection();
			this.editor.AddConnection(connection1);

			MyWidgets.EntityConnection connection2 = new MyWidgets.EntityConnection();
			this.editor.AddConnection(connection2);

			MyWidgets.EntityConnection connection3 = new MyWidgets.EntityConnection();
			this.editor.AddConnection(connection3);

			this.editor.UpdateGeometry();
			this.UpdateAll();
#endif
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


		protected EntitiesEditor.Editor editor;
	}
}
