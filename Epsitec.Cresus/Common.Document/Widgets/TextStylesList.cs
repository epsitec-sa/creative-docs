using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// TextStylesList représente la liste des styles de texte (Paragraph ou Character).
	/// </summary>
	public class TextStylesList : AbstractStyleList
	{
		public TextStylesList() : base()
		{
		}


		public string Caterory
		{
			//	Catérorie "Paragraph" ou "Character".
			get
			{
				return this.caterory;
			}

			set
			{
				this.caterory = value;
			}
		}

		public Text.TextStyle[] List
		{
			//	Liste des styles de texte représentés dans la liste.
			get
			{
				return this.list;
			}

			set
			{
				this.list = value;
			}
		}


		protected override int ListCount
		{
			//	Nombre le lignes de la liste.
			get
			{
				if ( this.list == null )  return 0;
				return this.list.Length;
			}
		}

		protected override int ListSelected
		{
			//	Ligne sélectionnée dans la liste.
			get
			{
				if ( this.list != null )
				{
					if ( this.caterory == "Paragraph" )  return this.document.SelectedParagraphStyle;
					if ( this.caterory == "Character" )  return this.document.SelectedCharacterStyle;
				}
				return -1;
			}
		}

		protected override string ListName(int rank)
		{
			//	Nom d'une ligne de la liste.
			if ( rank == -1 || this.list == null )
			{
				return Res.Strings.Aggregates.NoneLine;
			}
			else
			{
				Text.TextStyle style = this.list[rank];
				return this.document.TextContext.StyleList.StyleMap.GetCaption(style);
			}
		}

		protected override string ListChildrensCount(int rank)
		{
			//	Nombre d'enfants d'une ligne de la liste.
			return "";
		}

		protected override AbstractSample CreateSample()
		{
			//	Crée un échantillon.
			return new TextSample();
		}

		protected override void ListSample(AbstractSample sample, int rank)
		{
			//	 Met à jour l'échantillon d'une ligne de la liste.
			TextSample sm = sample as TextSample;

			if ( rank == -1 || this.list == null )
			{
				sm.TextStyle = null;
			}
			else
			{
				sm.TextStyle = this.list[rank];
			}

			sm.Invalidate();
		}


		protected string						caterory;
		protected Text.TextStyle[]				list;
	}
}
