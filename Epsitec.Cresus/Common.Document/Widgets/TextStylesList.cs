using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// TextStylesList repr�sente la liste des styles de texte (Paragraph ou Character).
	/// </summary>
	public class TextStylesList : AbstractStyleList
	{
		public TextStylesList() : base()
		{
		}


		public Text.TextStyle[] List
		{
			//	Liste des styles de texte repr�sent�s dans la liste.
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
			//	Ligne s�lectionn�e dans la liste.
			get
			{
				if ( this.list == null )
				{
					return -1;
				}
				else
				{
					return this.document.SelectedTextStyle;
				}
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
			//	Cr�e un �chantillon.
			return new TextSample();
		}

		protected override void ListSample(AbstractSample sample, int rank)
		{
			//	 Met � jour l'�chantillon d'une ligne de la liste.
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


		protected Text.TextStyle[]				list;
	}
}
