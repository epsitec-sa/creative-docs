using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	// ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	// sous peine de plant�e lors de la d�s�rialisation.
	public enum LayerType
	{
		None   = 0,		// aucun
		Show   = 1,		// affich� normalement
		Dimmed = 2,		// affich� estomp�
		Hide   = 3,		// cach� compl�tement
	}

	// ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	// sous peine de plant�e lors de la d�s�rialisation.
	public enum LayerPrint
	{
		None   = 0,		// aucun
		Show   = 1,		// imprim� normalement
		Dimmed = 2,		// imprim� estomp�
		Hide   = 3,		// cach� compl�tement
	}

	/// <summary>
	/// La classe Layer est la classe de l'objet graphique "calque".
	/// </summary>
	[System.Serializable()]
	public class Layer : Objects.Abstract
	{
		public Layer(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
			this.objects = new UndoableList(this.document, UndoableListType.ObjectsInsideDocument);
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			if ( type == Properties.Type.ModColor )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Layer(document, model);
		}

		public LayerType Type
		{
			get
			{
				return this.layerType;
			}
			
			set
			{
				if ( this.layerType != value )
				{
					this.InsertOpletType();
					this.layerType = value;
					this.document.Modifier.ActiveViewer.DrawingContext.UpdateAfterLayerChanged();
					this.document.IsDirtySerialize = true;
				}
			}
		}

		public LayerPrint Print
		{
			get
			{
				return this.layerPrint;
			}
			
			set
			{
				if ( this.layerPrint != value )
				{
					this.InsertOpletType();
					this.layerPrint = value;
					this.document.IsDirtySerialize = true;
				}
			}
		}

		public bool Magnet
		{
			get
			{
				return this.magnet;
			}
			
			set
			{
				if ( this.magnet != value )
				{
					this.InsertOpletType();
					this.magnet = value;
					this.document.Modifier.ActiveViewer.DrawingContext.UpdateAfterPageChanged();
					this.document.IsDirtySerialize = true;
				}
			}
		}


		// Reprend toutes les caract�ristiques d'un objet.
		public override void CloneObject(Objects.Abstract src)
		{
			base.CloneObject(src);
			Layer layer = src as Layer;
			this.layerType = layer.layerType;
			this.layerPrint = layer.layerPrint;
			this.magnet = layer.magnet;
		}


		// Retourne la cha�ne pour nommer la position d'un calque.
		public static string LayerPositionName(int rank, int total)
		{
			if ( total == 1 )
			{
				return "Unique";
			}
			else
			{
				if ( rank == 0 )
				{
					return "Fond";
				}
				else if ( rank < total-1 )
				{
					return string.Format("Fond+{0}", rank.ToString());
				}
				else
				{
					return "Dessus";
				}
			}
		}

		// Retourne le nom court d'un calque en fonction de son rang.
		public static string ShortName(int rank)
		{
			return ((char)('A'+rank)).ToString();
		}

		
		#region Menu
		// Construit le menu pour choisir un calque.
		public static VMenu CreateMenu(UndoableList layers, int currentLayer, MessageEventHandler message)
		{
			int total = layers.Count;
			VMenu menu = new VMenu();
			for ( int i=0 ; i<total ; i++ )
			{
				int ii = total-i-1;
				Objects.Layer layer = layers[ii] as Objects.Layer;

				string name = "";
				if ( layer.Name == "" )
				{
					name = string.Format("{0}: {1}", Layer.ShortName(ii), Layer.LayerPositionName(ii, total));
				}
				else
				{
					name = string.Format("{0}: {1}", Layer.ShortName(ii), layer.Name);
				}

				string icon = "manifest:Epsitec.App.DocumentEditor.Images.ActiveNo.icon";
				if ( ii == currentLayer )
				{
					icon = "manifest:Epsitec.App.DocumentEditor.Images.ActiveYes.icon";
				}

				MenuItem item = new MenuItem("LayerSelect(this.Name)", icon, name, "", ii.ToString());

				if ( message != null )
				{
					item.Pressed += message;
				}

				menu.Items.Add(item);
			}
			menu.AdjustSize();
			return menu;
		}
		#endregion

		
		#region OpletType
		// Ajoute un oplet pour m�moriser le type du calque.
		protected void InsertOpletType()
		{
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletType oplet = new OpletType(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		// M�morise le nom de l'objet.
		protected class OpletType : AbstractOplet
		{
			public OpletType(Layer host)
			{
				this.host = host;
				this.layerType = host.layerType;
				this.layerPrint = host.layerPrint;
				this.magnet = host.magnet;
			}

			protected void Swap()
			{
				LayerType type = host.layerType;
				host.layerType = this.layerType;  // host.layerType <-> this.layerType
				this.layerType = type;

				LayerPrint prnt = host.layerPrint;
				host.layerPrint = this.layerPrint;  // host.layerPrint <-> this.layerPrint
				this.layerPrint = prnt;
				
				Misc.Swap(ref this.magnet, ref host.magnet);

				this.host.document.Notifier.NotifyLayersChanged();
				this.host.document.Notifier.NotifyArea(this.host.document.Modifier.ActiveViewer);
			}

			public override IOplet Undo()
			{
				this.Swap();
				return this;
			}

			public override IOplet Redo()
			{
				this.Swap();
				return this;
			}

			protected Layer					host;
			protected LayerType				layerType;
			protected LayerPrint			layerPrint;
			protected bool					magnet;
		}
		#endregion

		
		#region Serialization
		// S�rialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("LayerType", this.layerType);

			if ( this.document.Type != DocumentType.Pictogram )
			{
				info.AddValue("LayerPrint", this.layerPrint);
				info.AddValue("Magnet", this.magnet);
			}
		}

		// Constructeur qui d�s�rialise l'objet.
		protected Layer(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.layerType = (LayerType) info.GetValue("LayerType", typeof(LayerType));

			if ( this.document.Type != DocumentType.Pictogram )
			{
				this.layerPrint = (LayerPrint) info.GetValue("LayerPrint", typeof(LayerPrint));

				if ( this.document.IsRevisionGreaterOrEqual(1,0,15) )
				{
					this.magnet = info.GetBoolean("Magnet");
				}
			}
		}
		#endregion

		
		protected LayerType			layerType = LayerType.Dimmed;
		protected LayerPrint		layerPrint = LayerPrint.Show;
		protected bool				magnet = false;
	}
}
