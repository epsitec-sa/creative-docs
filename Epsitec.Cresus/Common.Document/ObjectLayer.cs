using Epsitec.Common.Support;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	public enum LayerType
	{
		None,		// aucun
		Show,		// affiché normalement
		Dimmed,		// affiché estompé
		Hide,		// caché complètement
	}

	/// <summary>
	/// La classe ObjectLayer est la classe de l'objet graphique "calque".
	/// </summary>
	[System.Serializable()]
	public class ObjectLayer : AbstractObject
	{
		public ObjectLayer(Document document, AbstractObject model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
			this.objects = new UndoableList(this.document, UndoableListType.ObjectsInsideDocument);
		}

		protected override bool ExistingProperty(PropertyType type)
		{
			if ( type == PropertyType.ModColor )  return true;
			return false;
		}

		protected override AbstractObject CreateNewObject(Document document, AbstractObject model)
		{
			return new ObjectLayer(document, model);
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


		// Reprend toutes les caractéristiques d'un objet.
		public override void CloneObject(AbstractObject src)
		{
			base.CloneObject(src);
			ObjectLayer layer = src as ObjectLayer;
			this.layerType = layer.layerType;
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
			public OpletType(ObjectLayer host)
			{
				this.host = host;
				this.layerType = host.layerType;
			}

			protected void Swap()
			{
				LayerType temp = host.layerType;
				host.layerType = this.layerType;  // host.layerType <-> this.layerType
				this.layerType = temp;

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

			protected ObjectLayer					host;
			protected LayerType						layerType;
		}
		#endregion

		
		#region Serialization
		// Sérialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("LayerType", this.layerType);
		}

		// Constructeur qui désérialise l'objet.
		protected ObjectLayer(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.layerType = (LayerType) info.GetValue("LayerType", typeof(LayerType));
		}
		#endregion

		
		protected LayerType			layerType = LayerType.Dimmed;
	}
}
