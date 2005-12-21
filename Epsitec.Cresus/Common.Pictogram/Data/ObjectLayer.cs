using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
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
	public class ObjectLayer : AbstractObject
	{
		public ObjectLayer()
		{
			this.objects = new UndoList();

			PropertyModColor modColor = new PropertyModColor();
			modColor.Type = PropertyType.ModColor;
			modColor.ExtendedSize = true;
			this.AddProperty(modColor);
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectLayer();
		}

		[XmlIgnore]
		public bool Actif
		{
			get { return this.actif; }
			set { this.actif = value; }
		}

		[XmlAttribute]
		public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

		[XmlAttribute]
		public LayerType Type
		{
			get { return this.layerType; }
			set { this.layerType = value; }
		}


		public override void CloneObject(AbstractObject src)
		{
			//	Reprend toutes les caractéristiques d'un objet.
			base.CloneObject(src);
			ObjectLayer layer = src as ObjectLayer;
			this.actif     = layer.actif;
			this.name      = layer.name;
			this.layerType = layer.layerType;
		}


		protected bool				actif = false;
		protected string			name = "";
		protected LayerType			layerType = LayerType.Dimmed;
	}
}
