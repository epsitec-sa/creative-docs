//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.Shortcut))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Shortcut permet de représenter un raccourci clavier.
	/// </summary>
	public sealed class Shortcut : DependencyObject, System.IEquatable<Shortcut>
	{
		public Shortcut()
		{
		}
		
		public Shortcut(KeyCode code)
		{
			this.KeyCode = code;
		}
		
		public Shortcut(char code, ModifierKeys modifier)
		{
			if ((code >= 'a') && (code <= 'z'))
			{
				this.KeyCode = (KeyCode)(KeyCode.AlphaA + code - 'a' + (int) modifier);
			}
			else if ((code >= 'A') && (code <= 'Z'))
			{
				this.KeyCode = (KeyCode)(KeyCode.AlphaA + code - 'A' + (int) modifier);
			}
			else if ((code >= '0') && (code <= '9'))
			{
				this.KeyCode = (KeyCode)(KeyCode.Digit0 + code - '0' + (int) modifier);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("No KeyCode mapping for shortcut char '{0}'", code));
			}
		}
		
		
		public static implicit operator Shortcut(KeyCode code)
		{
			return new Shortcut (code);
		}
		
		public static implicit operator Shortcut(Message message)
		{
			return Shortcut.FromMessage (message);
		}
		
		
		public static Shortcut FromMessage(Message message)
		{
			KeyCode key_code = message.KeyCodeOnly;
			
			if (message.Type == MessageType.KeyDown)
			{
				if ((key_code != KeyCode.None) &&
					(message.IsAltPressed))
				{
					//	OK. Utilise le code clavier :
					
					key_code |= KeyCode.ModifierAlt;
				}
				else
				{
					switch (key_code)
					{
						case KeyCode.ArrowDown:
						case KeyCode.ArrowLeft:
						case KeyCode.ArrowRight:
						case KeyCode.ArrowUp:
						case KeyCode.Back:
						case KeyCode.Clear:
						case KeyCode.Delete:
						case KeyCode.End:
						case KeyCode.Escape:
						case KeyCode.FuncF1:  case KeyCode.FuncF2:  case KeyCode.FuncF3:  case KeyCode.FuncF4:  case KeyCode.FuncF5:
						case KeyCode.FuncF6:  case KeyCode.FuncF7:  case KeyCode.FuncF8:  case KeyCode.FuncF9:
						case KeyCode.FuncF10: case KeyCode.FuncF11: case KeyCode.FuncF12: case KeyCode.FuncF13: case KeyCode.FuncF14:
						case KeyCode.FuncF15: case KeyCode.FuncF16: case KeyCode.FuncF17: case KeyCode.FuncF18: case KeyCode.FuncF19:
						case KeyCode.FuncF20: case KeyCode.FuncF21: case KeyCode.FuncF22: case KeyCode.FuncF23: case KeyCode.FuncF24:
						case KeyCode.Home:
						case KeyCode.Insert:
						case KeyCode.PageDown:
						case KeyCode.PageUp:
						case KeyCode.Pause:
							break;

						case KeyCode.Digit0: case KeyCode.Digit1: case KeyCode.Digit2: case KeyCode.Digit3: case KeyCode.Digit4:
						case KeyCode.Digit5: case KeyCode.Digit6: case KeyCode.Digit7: case KeyCode.Digit8: case KeyCode.Digit9:
							break;
						
						default:
							key_code = KeyCode.None;
							break;
					}
				}
				
				if ((key_code & KeyCode.KeyCodeMask) != KeyCode.None)
				{
					if (message.IsControlPressed)
					{
						key_code |= KeyCode.ModifierControl;
					}
					
					if (message.IsShiftPressed)
					{
						key_code |= KeyCode.ModifierShift;
					}
					
					return new Shortcut (key_code);
				}
			}
			else if (message.Type == MessageType.KeyPress)
			{
				if (message.IsControlPressed)
				{
					key_code |= KeyCode.ModifierControl;
				}
				
				if (message.IsShiftPressed)
				{
					key_code |= KeyCode.ModifierShift;
				}
				
				return new Shortcut (key_code);
			}
			
			return null;
		}
		
		
		public KeyCode					KeyCode
		{
			get
			{
				return this.key_code;
			}
			set
			{
				if (this.key_code != value)
				{
					this.key_code = value;
				}
			}
		}
		
		public KeyCode					KeyCodeOnly
		{
			get
			{
				return this.KeyCode & KeyCode.KeyCodeMask;
			}
		}
		
		public char						KeyChar
		{
			get
			{
				return (char) (this.KeyCode & KeyCode.KeyCodeMask);
			}
		}
		
		
		public bool						IsShiftPressed
		{
			get
			{
				return (this.KeyCode & KeyCode.ModifierShift) != 0;
			}
		}
		
		public bool						IsControlPressed
		{
			get
			{
				return (this.KeyCode & KeyCode.ModifierControl) != 0;
			}
		}
		
		public bool						IsAltPressed
		{
			get
			{
				return (this.KeyCode & KeyCode.ModifierAlt) != 0;
			}
		}
		
		
		public override string ToString()
		{
			return Message.GetKeyName (this.key_code);
		}

		public override bool Equals(object obj)
		{
			return this.Equals (obj as Shortcut);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		#region IEquatable<Shortcut> Members

		public bool Equals(Shortcut other)
		{
			if (object.ReferenceEquals (other, null))
			{
				return false;
			}

			return this.key_code == other.key_code;
		}

		#endregion
		
		public static bool operator==(Shortcut a, Shortcut b)
		{
			if (object.ReferenceEquals (a, b))
			{
				return true;
			}
			if (object.ReferenceEquals (a, null))
			{
				return false;
			}

			return a.Equals (b);
		}
		
		public static bool operator!=(Shortcut a, Shortcut b)
		{
			return !(a == b);
		}


		public static readonly DependencyProperty ShortcutsProperty = DependencyProperty.RegisterAttached ("Shortcuts", typeof (Collections.ShortcutCollection), typeof (Shortcut));
		
		private KeyCode					key_code;
	}
}
