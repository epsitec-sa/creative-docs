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
		
		public int						KeyCode
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
//					char key_char  = System.Char.ToUpper ((char) (value & 0x0000ffff), System.Globalization.CultureInfo.CurrentCulture);
//					int  modifiers = value & 0x0fff0000;
					
					this.mode     = ShortcutMode.KeyCode;
					this.key_code = value; //key_char | modifiers;
				}
			}
		}
		
		public char						KeyChar
		{
			get
			{
				switch (this.mode)
				{
					case ShortcutMode.KeyCode:
						return (char) (this.KeyCode & 0x0000FFFF);
					case ShortcutMode.Mnemonic:
						return this.mnemonic_code;
				}
				
				return (char) 0;
			}
		}
		
		
		public override string ToString()
		{
			return System.String.Format ("{{Key={0}}}", this.key_code);
		}

		
		private ShortcutMode			mode;
		private char					mnemonic_code;
		private int						key_code;
	}
	
	public enum ShortcutMode
	{
		None,
		Mnemonic,
		KeyCode
	}
}
