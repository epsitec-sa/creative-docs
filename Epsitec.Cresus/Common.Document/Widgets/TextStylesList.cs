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


		public StyleCategory Category
		{
			//	Catérorie des styles de texte représentés.
			get
			{
				return this.category;
			}

			set
			{
				this.category = value;
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
			//	Nombre de lignes de la liste.
			get
			{
				int count = 0;

				if ( this.list != null )
				{
					count = this.list.Length;;
				}

				if ( this.excludeRank != -1 )
				{
					count --;
				}

				if ( this.isNoneLine )
				{
					count ++;
				}

				return count;
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
				return Misc.UserTextStyleName(this.document.TextContext.StyleList.StyleMap.GetCaption(style));
			}
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


		protected StyleCategory					category;
		protected Text.TextStyle[]				list;
	}
}
