using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	//	ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	//	sous peine de plantée lors de la désérialisation.
	public enum LayerType
	{
		None   = 0,		// aucun
		Show   = 1,		// affiché normalement
		Dimmed = 2,		// affiché estompé
		Hide   = 3,		// caché complètement
	}

	//	ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	//	sous peine de plantée lors de la désérialisation.
	public enum LayerPrint
	{
		None   = 0,		// aucun
		Show   = 1,		// imprimé normalement
		Dimmed = 2,		// imprimé estompé
		Hide   = 3,		// caché complètement
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
					this.document.SetDirtySerialize(CacheBitmapChanging.All);
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
					this.document.SetDirtySerialize(CacheBitmapChanging.All);
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
					this.document.SetDirtySerialize(CacheBitmapChanging.None);
				}
			}
		}


		public override void CloneObject(Objects.Abstract src)
		{
			//	Reprend toutes les caractéristiques d'un objet.
			base.CloneObject(src);
			Layer layer = src as Layer;
			this.layerType = layer.layerType;
			this.layerPrint = layer.layerPrint;
			this.magnet = layer.magnet;
		}


		public static string LayerPositionName(int rank, int total)
		{
			//	Retourne la chaîne pour nommer la position d'un calque.
			if ( total == 1 )
			{
				return Res.Strings.Layer.PositionName.Unique;
			}
			else
			{
				if ( rank == 0 )
				{
					return Res.Strings.Layer.PositionName.Back;
				}
				else if ( rank < total-1 )
				{
					return string.Format(Res.Strings.Layer.PositionName.Middle, rank.ToString());
				}
				else
				{
					return Res.Strings.Layer.PositionName.Front;
				}
			}
		}

		public static string ShortName(int rank)
		{
			//	Retourne le nom court d'un calque en fonction de son rang.
			return ((char)('A'+rank)).ToString();
		}

		
		#region Menu
		public static VMenu CreateMenu(UndoableList layers, int currentLayer, string cmd, Support.EventHandler<MessageEventArgs> message)
		{
			//	Construit le menu pour choisir un calque.
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

				string icon = Misc.Icon("RadioNo");
				if ( ii == currentLayer )
				{
					icon = Misc.Icon("RadioYes");
					name = Misc.Bold(name);
				}

				if (!string.IsNullOrEmpty(cmd))
				{
					Misc.CreateStructuredCommandWithName(cmd);
				}

				MenuItem item = new MenuItem(cmd, icon, name, "", ii.ToString());

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


		#region CacheBitmap
		protected override void CacheBitmapCreate()
		{
			//	Crée le bitmap caché.
			//?System.Diagnostics.Debug.WriteLine(string.Format("CacheBitmapCreate layer #{0}", this.LayerNumber));
			if (this.cacheBitmapSize.IsEmpty)
			{
				this.cacheBitmap = null;
			}
			else
			{
				Size size = this.cacheBitmapSize;
				size -= new Size(2, 2);  // laisse un cadre d'un pixel
				this.cacheBitmap = this.document.Printer.CreateMiniatureBitmap(size, false, this.PageNumber, this.LayerNumber);
			}
		}
		#endregion

	
		#region OpletType
		protected void InsertOpletType()
		{
			//	Ajoute un oplet pour mémoriser le type du calque.
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletType oplet = new OpletType(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		//	Mémorise le nom de l'objet.
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
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise l'objet.
			base.GetObjectData(info, context);

			info.AddValue("LayerType", this.layerType);

			if ( this.document.Type != DocumentType.Pictogram )
			{
				info.AddValue("LayerPrint", this.layerPrint);
				info.AddValue("Magnet", this.magnet);
			}
		}

		protected Layer(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise l'objet.
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
