using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyList représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyList : AbstractProperty
	{
		public PropertyList()
		{
		}

		[XmlAttribute]
		public int Choice
		{
			get { return this.choice; }
			set { this.choice = value; }
		}

		public void Clear()
		{
			this.list.Clear();
		}

		[XmlIgnore]
		public int Count
		{
			get { return this.list.Count; }
		}

		public void Add(string text)
		{
			this.list.Add(text);
		}

		public string Get(int index)
		{
			return this.list[index] as string;
		}


		// Effectue une copie de la propriété.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyList p = property as PropertyList;

			p.Choice = this.choice;

			p.Clear();
			for ( int i=0 ; i<this.Count ; i++ )
			{
				p.Add(this.Get(i));
			}
		}

		// Compare deux propriétés.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyList p = property as PropertyList;
			if ( p.Choice != this.choice )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel()
		{
			return new PanelList();
		}

		protected int							choice = 0;
		protected System.Collections.ArrayList	list = new System.Collections.ArrayList();
	}
}
