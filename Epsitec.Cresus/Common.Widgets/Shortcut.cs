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
