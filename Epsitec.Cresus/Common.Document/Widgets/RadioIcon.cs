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

		public int Rank
		{
			get
			{
				return this.rank;
			}

			set
			{
				this.rank = value;
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

		protected override bool AboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			RadioIconGrid grid = this.Parent as RadioIconGrid;
			
			if ( grid == null ||
				 this.ActiveState == WidgetState.ActiveYes ||
				 mode != TabNavigationMode.ActivateOnTab )
			{
				return base.AboutToGetFocus(dir, mode, out focus);
			}
			
			//	Ce n'est pas notre bouton radio qui est allumé. TAB voudrait nous donner le
			//	focus, mais ce n'est pas adéquat; mieux vaut mettre le focus sur le frère qui
			//	est activé :
			
			RadioIcon icon = grid.SelectedRadioIcon;
			
			if ( icon == null )
			{
				return base.AboutToGetFocus(dir, mode, out focus);
			}
			else
			{
				return icon.AboutToGetFocus(dir, mode, out focus);
			}
		}
		
		protected override System.Collections.ArrayList FindTabWidgetList(TabNavigationMode mode)
		{
			if ( mode != TabNavigationMode.ActivateOnTab )
			{
				return base.FindTabWidgetList(mode);
			}
			
			//	On recherche les frères de ce widget, pour déterminer lequel devra être activé par la
			//	pression de la touche TAB. Pour bien faire, il faut supprimer les autres boutons radio
			//	qui appartiennent à notre groupe :
			
			System.Collections.ArrayList list = base.FindTabWidgetList (mode);
			System.Collections.ArrayList copy = new System.Collections.ArrayList ();
			
			foreach (Widget widget in list)
			{
				RadioIcon icon = widget as RadioIcon;
				
				if ( icon != null &&
					 icon != this )
				{
					//	Saute les boutons du même groupe. Ils ne sont pas accessibles par la
					//	touche TAB.
				}
				else
				{
					copy.Add (widget);
				}
			}
			
			return copy;
		}
		
		
		protected int				enumValue;
		protected bool				endOfLine;
		protected int				rank;
		protected int				column;
		protected int				row;
	}
}
