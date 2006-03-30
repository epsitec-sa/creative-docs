using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe RadioIcon est un IconButton pour une collection dont un seul
	/// bouton sera sélectionné.
	/// </summary>
	public class RadioIcon : IconButton
	{
		public RadioIcon()
		{
			this.AutoToggle = true;
			this.AutoRadio  = true;
			this.ButtonStyle = ButtonStyle.ActivableIcon;
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

		public int Column
		{
			get
			{
				return this.column;
			}

			set
			{
				this.column = value;
			}
		}

		public int Row
		{
			get
			{
				return this.row;
			}

			set
			{
				this.row = value;
			}
		}


		protected int				enumValue;
		protected bool				endOfLine;
		protected int				rank = -1;
		protected int				column = -1;
		protected int				row = -1;
	}
}
