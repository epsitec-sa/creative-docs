using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe RadioIcon est un IconButton pour une collection dont un seul
	/// bouton sera sélectionné.
	/// </summary>
	public class RadioIcon : IconButton
	{
		public RadioIcon()
		{
		}

		public RadioIcon(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		public int EnumValue
		{
			get
			{
				return this.enumValue;
			}

			set
			{
				this.enumValue = value;
			}
		}

		public bool EndOfLine
		{
			get
			{
				return this.endOfLine;
			}

			set
			{
				this.endOfLine = value;
			}
		}


		protected int				enumValue;
		protected bool				endOfLine;
	}
}
