using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectPattern est la classe de l'objet graphique "pattern".
	/// </summary>
	public class ObjectPattern : AbstractObject
	{
		public ObjectPattern()
		{
			this.objects = new UndoList();
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectPattern();
		}

		[XmlAttribute]
		public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

		[XmlAttribute]
		public int Id
		{
			get { return this.id; }
			set { this.id = value; }
		}

		[XmlIgnore]
		public int CurrentPage
		{
			get { return this.currentPage; }
			set { this.currentPage = value; }
		}

		[XmlIgnore]
		public double CurrentZoom
		{
			get { return this.currentZoom; }
			set { this.currentZoom = value; }
		}

		[XmlIgnore]
		public double CurrentOriginX
		{
			get { return this.currentOriginX; }
			set { this.currentOriginX = value; }
		}

		[XmlIgnore]
		public double CurrentOriginY
		{
			get { return this.currentOriginY; }
			set { this.currentOriginY = value; }
		}


		// Reprend toutes les caractéristiques d'un objet.
		public override void CloneObject(AbstractObject src)
		{
			base.CloneObject(src);
			ObjectPattern page = src as ObjectPattern;
			this.name = page.name;
			this.id   = page.id;
		}


		protected string		name = "";
		protected int			id = 0;
		protected int			currentPage = 0;
		protected double		currentZoom = 1;
		protected double		currentOriginX = 0;
		protected double		currentOriginY = 0;
	}
}
