using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyCombo représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyCombo : AbstractProperty
	{
		public PropertyCombo()
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

		public string GetListName()
		{
			//	Détermine le nom de la propriété dans la liste (Lister).
			return this.list[this.choice] as string;
		}

		public string GetName(int index)
		{
			return this.list[index] as string;
		}


		public override void CopyTo(AbstractProperty property)
		{
			//	Effectue une copie de la propriété.
			base.CopyTo(property);
			PropertyCombo p = property as PropertyCombo;

			p.Choice = this.choice;

			p.Clear();
			for ( int i=0 ; i<this.Count ; i++ )
			{
				p.Add(this.GetName(i));
			}
		}

		public override bool Compare(AbstractProperty property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			PropertyCombo p = property as PropertyCombo;
			if ( p.Choice != this.choice )  return false;

			return true;
		}

		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			return new PanelCombo(drawer);
		}

		protected int							choice = 0;
		protected System.Collections.ArrayList	list = new System.Collections.ArrayList();
	}
}
