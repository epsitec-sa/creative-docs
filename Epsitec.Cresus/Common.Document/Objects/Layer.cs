using Epsitec.Common.Support;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	public enum LayerType
	{
		None,		// aucun
		Show,		// affiché normalement
		Dimmed,		// affiché estompé
		Hide,		// caché complètement
	}

	public enum LayerPrint
	{
		None,		// aucun
		Show,		// imprimé normalement
		Dimmed,		// imprimé estompé
		Hide,		// caché complètement
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
				}
			}
		}


		// Reprend toutes les caractéristiques d'un objet.
		public override void CloneObject(Objects.Abstract src)
		{
			base.CloneObject(src);
			Layer layer = src as Layer;
			this.layerType = layer.layerType;
			this.layerPrint = layer.layerPrint;
		}


		// Retourne la chaîne pour nommer la position d'un calque.
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

		
		#region OpletType
		// Ajoute un oplet pour mémoriser le type du calque.
		protected void InsertOpletType()
		{
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletType oplet = new OpletType(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		// Mémorise le nom de l'objet.
		protected class OpletType : AbstractOplet
		{
			public OpletType(Layer host)
			{
				this.host = host;
				this.layerType = host.layerType;
				this.layerPrint = host.layerPrint;
			}

			protected void Swap()
			{
				LayerType type = host.layerType;
				host.layerType = this.layerType;  // host.layerType <-> this.layerType
				this.layerType = type;

				LayerPrint prnt = host.layerPrint;
				host.layerPrint = this.layerPrint;  // host.layerPrint <-> this.layerPrint
				this.layerPrint = prnt;

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
		}
		#endregion

		
		#region Serialization
		// Sérialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("LayerType", this.layerType);
			info.AddValue("LayerPrint", this.layerPrint);
		}

		// Constructeur qui désérialise l'objet.
		protected Layer(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.layerType = (LayerType) info.GetValue("LayerType", typeof(LayerType));

			if ( Support.Serialization.Helper.FindElement(info, "LayerPrint") )
			{
				this.layerPrint = (LayerPrint) info.GetValue("LayerPrint", typeof(LayerPrint));
			}
		}
		#endregion

		
		protected LayerType			layerType = LayerType.Dimmed;
		protected LayerPrint		layerPrint = LayerPrint.Show;
	}
}
