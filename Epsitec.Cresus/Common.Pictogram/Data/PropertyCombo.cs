using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyCombo repr�sente une propri�t� d'un objet graphique.
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

		// D�termine le nom de la propri�t� dans la liste (Lister).
		public string GetListName()
		{
			return this.list[this.choice] as string;
		}

		public string GetName(int index)
		{
			return this.list[index] as string;
		}


		// Effectue une copie de la propri�t�.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyCombo p = property as PropertyCombo;

			p.Choice = this.choice;

			p.Clear();
			for ( int i=0 ; i<this.Count ; i++ )
			{
				p.Add(this.GetName(i));
			}
		}

		// Compare deux propri�t�s.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyCombo p = property as PropertyCombo;
			if ( p.Choice != this.choice )  return false;

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			return new PanelCombo(drawer);
		}

		protected int							choice = 0;
		protected System.Collections.ArrayList	list = new System.Collections.ArrayList();
	}
}
