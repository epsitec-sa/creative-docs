using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectPage est la classe de l'objet graphique "page".
	/// </summary>
	public class ObjectPage : AbstractObject
	{
		public ObjectPage()
		{
			this.objects = new System.Collections.ArrayList();
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectPage();
		}

		[XmlAttribute]
		public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

		[XmlIgnore]
		public int CurrentLayer
		{
			get { return this.currentLayer; }
			set { this.currentLayer = value; }
		}


		// Reprend toutes les caractéristiques d'un objet.
		public override void CloneObject(AbstractObject src)
		{
			base.CloneObject(src);
			ObjectPage page = src as ObjectPage;
			this.name = page.name;
		}


		protected string		name = "";
		protected int			currentLayer;
	}
}
