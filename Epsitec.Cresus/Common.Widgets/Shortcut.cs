namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Shortcut permet de représenter un raccourci clavier.
	/// </summary>
	public class Shortcut
	{
		public Shortcut()
		{
		}
		
		public Shortcut(KeyCode code)
		{
			this.KeyCode = code;
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
						
						default:
							key_code = KeyCode.None;
							break;
					}
				}
				
				if ((key_code & KeyCode.KeyCodeMask) != KeyCode.None)
				{
					if (message.IsCtrlPressed)
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
				if (message.IsCtrlPressed)
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
		
		
		public bool Match(Shortcut shortcut)
		{
			switch (this.mode)
			{
				case ShortcutMode.Mnemonic:
					if (shortcut.KeyChar == this.mnemonic_code)
					{
						return true;
					}
					break;
				
				case ShortcutMode.KeyCode:
					if (shortcut.KeyCode == this.key_code)
					{
						return true;
					}
					break;
			}
			
			return false;
		}
		
		
		public ShortcutMode				Mode
		{
			get { return this.mode; }
		}

		public char						Mnemonic
		{
			get
			{
				if (this.mode == ShortcutMode.Mnemonic)
				{
					return this.mnemonic_code;
				}
				return (char) 0;
			}
			set
			{
				if (this.mnemonic_code != value)
				{
					this.mode          = ShortcutMode.Mnemonic;
					this.mnemonic_code = value;
				}
			}
		}
		
		public KeyCode					KeyCode
		{
			get
			{
				if (this.Mode == ShortcutMode.KeyCode)
				{
					return this.key_code;
				}
				
				return 0;
			}
			set
			{
				if (this.key_code != value)
				{
					this.mode     = ShortcutMode.KeyCode;
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
				switch (this.mode)
				{
					case ShortcutMode.KeyCode:
						return (char) (this.KeyCode & KeyCode.KeyCodeMask);
					case ShortcutMode.Mnemonic:
						return this.mnemonic_code;
				}
				
				return (char) 0;
			}
		}
		
		
		public bool						IsShiftPressed
		{
			get
			{
				return (this.KeyCode & KeyCode.ModifierShift) != 0;
			}
		}
		
		public bool						IsCtrlPressed
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
			System.Text.StringBuilder extra = new System.Text.StringBuilder ();
			
			if (this.IsShiftPressed) extra.Append ("+SHIFT");
			if (this.IsCtrlPressed)  extra.Append ("+CTRL");
			if (this.IsAltPressed)   extra.Append ("+ALT");
			
			return System.String.Format ("[{0}{1}]", this.KeyCodeOnly.ToString (), extra.ToString ());
		}

		public override bool Equals(object obj)
		{
			Shortcut s = obj as Shortcut;
			
			if (s == null)
			{
				return false;
			}
			
			return (this.mode == s.mode) && (this.key_code == s.key_code);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
		
		
		public static bool operator==(Shortcut a, Shortcut b)
		{
			return System.Object.Equals (a, b);
		}
		
		public static bool operator!=(Shortcut a, Shortcut b)
		{
			return ! System.Object.Equals (a, b);
		}
		
		
		private ShortcutMode			mode;
		private char					mnemonic_code;
		private KeyCode					key_code;
	}
	
	public enum ShortcutMode
	{
		None,
		Mnemonic,
		KeyCode
	}
}
