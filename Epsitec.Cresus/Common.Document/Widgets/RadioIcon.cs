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


		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.KeyDown:
					if (this.ProcessKeyDown(message.KeyCode))
					{
						message.Consumer = this;
						return;
					}
					break;
			}
			
			base.ProcessMessage(message, pos);
		}

		protected virtual bool ProcessKeyDown(KeyCode key)
		{
			RadioIconGrid grid = this.Parent as RadioIconGrid;
			
			if ( grid == null )  return false;
			
			switch( key )
			{
				case KeyCode.ArrowUp:
				case KeyCode.ArrowDown:
				case KeyCode.ArrowLeft:
				case KeyCode.ArrowRight:
					return grid.Navigate(this, key);
				
				default:
					return false;
			}
		}

		
		protected int				enumValue;
		protected bool				endOfLine;
		protected int				column;
		protected int				row;
	}
}
