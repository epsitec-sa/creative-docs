//	Copyright © 2003-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.Shortcut))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>Shortcut</c> class represents a keyboard shortcut.
	/// </summary>
	public sealed class Shortcut : DependencyObject, System.IEquatable<Shortcut>
	{
		public Shortcut()
		{
		}
		
		public Shortcut(KeyCode code)
		{
			this.keyCode = code;
		}
		
		public Shortcut(char code, ModifierKeys modifier)
		{
			if ((code >= 'a') && (code <= 'z'))
			{
				this.keyCode = (KeyCode) (KeyCode.AlphaA + code - 'a' + (int) modifier);
			}
			else if ((code >= 'A') && (code <= 'Z'))
			{
				this.keyCode = (KeyCode) (KeyCode.AlphaA + code - 'A' + (int) modifier);
			}
			else if ((code >= '0') && (code <= '9'))
			{
				this.keyCode = (KeyCode) (KeyCode.Digit0 + code - '0' + (int) modifier);
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


		/// <summary>
		/// Gets or sets the key code for this shortcut.
		/// </summary>
		/// <value>The key code.</value>
		public KeyCode KeyCode
		{
			get
			{
				return this.keyCode;
			}
			set
			{
				if (this.keyCode != value)
				{
					string oldValue = this.keyCode.ToString ();

					this.keyCode = value;

					string newValue = this.keyCode.ToString ();

					this.InvalidateProperty (Shortcut.KeyCodeProperty, oldValue, newValue);
				}
			}
		}

		/// <summary>
		/// Gets the key code stripped from its modifier keys.
		/// </summary>
		/// <value>The key code.</value>
		public KeyCode					KeyCodeOnly
		{
			get
			{
				return this.KeyCode & KeyCode.KeyCodeMask;
			}
		}

		/// <summary>
		/// Gets the key code as a <c>char</c>.
		/// </summary>
		/// <value>The key code.</value>
		public char						KeyChar
		{
			get
			{
				return (char) (this.KeyCode & KeyCode.KeyCodeMask);
			}
		}


		/// <summary>
		/// Gets a value indicating whether the shortcut includes shift.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this shortcut includes shift; otherwise, <c>false</c>.
		/// </value>
		public bool						IsShiftDefined
		{
			get
			{
				return (this.KeyCode & KeyCode.ModifierShift) != 0;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the shortcut includes control.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this shortcut includes control; otherwise, <c>false</c>.
		/// </value>
		public bool						IsControlDefined
		{
			get
			{
				return (this.KeyCode & KeyCode.ModifierControl) != 0;
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether the shortcut includes alt.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this shortcut includes alt; otherwise, <c>false</c>.
		/// </value>
		public bool						IsAltDefined
		{
			get
			{
				return (this.KeyCode & KeyCode.ModifierAlt) != 0;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this shortcut is empty.
		/// </summary>
		/// <value><c>true</c> if this shortcut is empty; otherwise, <c>false</c>.</value>
		public bool						IsEmpty
		{
			get
			{
				return this.keyCode == KeyCode.None;
			}
		}
		
		
		public override string ToString()
		{
			return Message.GetKeyName (this.keyCode);
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

			return this.keyCode == other.keyCode;
		}

		#endregion

		public static Shortcut FromMessage(Message message)
		{
			KeyCode keyCode = message.KeyCodeOnly;

			if (message.MessageType == MessageType.KeyDown)
			{
				if ((keyCode != KeyCode.None) &&
					(message.IsAltPressed))
				{
					//	OK. Utilise le code clavier :

					keyCode |= KeyCode.ModifierAlt;
				}
				else
				{
					switch (keyCode)
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
						case KeyCode.FuncF1:
						case KeyCode.FuncF2:
						case KeyCode.FuncF3:
						case KeyCode.FuncF4:
						case KeyCode.FuncF5:
						case KeyCode.FuncF6:
						case KeyCode.FuncF7:
						case KeyCode.FuncF8:
						case KeyCode.FuncF9:
						case KeyCode.FuncF10:
						case KeyCode.FuncF11:
						case KeyCode.FuncF12:
						case KeyCode.FuncF13:
						case KeyCode.FuncF14:
						case KeyCode.FuncF15:
						case KeyCode.FuncF16:
						case KeyCode.FuncF17:
						case KeyCode.FuncF18:
						case KeyCode.FuncF19:
						case KeyCode.FuncF20:
						case KeyCode.FuncF21:
						case KeyCode.FuncF22:
						case KeyCode.FuncF23:
						case KeyCode.FuncF24:
						case KeyCode.Home:
						case KeyCode.Insert:
						case KeyCode.PageDown:
						case KeyCode.PageUp:
						case KeyCode.Pause:
							break;

						case KeyCode.Digit0:
						case KeyCode.Digit1:
						case KeyCode.Digit2:
						case KeyCode.Digit3:
						case KeyCode.Digit4:
						case KeyCode.Digit5:
						case KeyCode.Digit6:
						case KeyCode.Digit7:
						case KeyCode.Digit8:
						case KeyCode.Digit9:
							break;

						case KeyCode.NumericAdd:
						case KeyCode.NumericSubstract:
						case KeyCode.NumericMultiply:
						case KeyCode.NumericDivide:
						case KeyCode.NumericDecimal:
						case KeyCode.NumLock:
							break;

						default:
							keyCode = KeyCode.None;
							break;
					}
				}

				if ((keyCode & KeyCode.KeyCodeMask) != KeyCode.None)
				{
					if (message.IsControlPressed)
					{
						keyCode |= KeyCode.ModifierControl;
					}

					if (message.IsShiftPressed)
					{
						keyCode |= KeyCode.ModifierShift;
					}

					return new Shortcut (keyCode);
				}
			}
			else if (message.MessageType == MessageType.KeyPress)
			{
				if (message.IsControlPressed)
				{
					keyCode |= KeyCode.ModifierControl;
				}

				if (message.IsShiftPressed)
				{
					keyCode |= KeyCode.ModifierShift;
				}

				return new Shortcut (keyCode);
			}
			else if (message.MessageType == MessageType.ApplicationCommand)
			{
				switch (message.ApplicationCommand)
				{
					case Message.ApplicationCommands.BrowserBackward:
						return new Shortcut (KeyCode.ModifierAlt | KeyCode.ArrowLeft);
					
					case Message.ApplicationCommands.BrowserForward:
						return new Shortcut (KeyCode.ModifierAlt | KeyCode.ArrowRight);
				}
			}

			return null;
		}
		
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


		/// <summary>
		/// Appends the main shortcut name to the text.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="shortcuts">The shortcuts.</param>
		/// <returns>The text with the shortcut.</returns>
		public static string AppendShortcutText(string text, IEnumerable<Shortcut> shortcuts)
		{
			if (shortcuts != null)
			{
				foreach (Shortcut shortcut in shortcuts)
				{
					if (!shortcut.IsEmpty)
					{
						return string.Format (Res.Strings.Shortcut.Template.TextWithShortcut.ToString (), text, shortcut);
					}
				}
			}
			
			return text;
		}

		
		public static Collections.ShortcutCollection GetShortcuts(DependencyObject obj)
		{
			return obj.GetValue (Shortcut.ShortcutsProperty) as Collections.ShortcutCollection;
		}

		public static bool HasShortcuts(DependencyObject obj)
		{
			Collections.ShortcutCollection shortcuts = obj.GetValueBase (Shortcut.ShortcutsProperty) as Collections.ShortcutCollection;

			return (shortcuts != null) && (shortcuts.Count > 0);
		}

		public static void SetShortcuts(DependencyObject obj, Collections.ShortcutCollection shortcuts)
		{
			if (shortcuts == null)
			{
				obj.ClearValue (Shortcut.ShortcutsProperty);
			}
			else
			{
				obj.SetValue (Shortcut.ShortcutsProperty, shortcuts);
			}
		}

		#region Dependency Property Support Methods

		private static object GetShortcutsValue(DependencyObject obj)
		{
			Collections.ShortcutCollection shortcuts = obj.GetValueBase (Shortcut.ShortcutsProperty) as Collections.ShortcutCollection;

			if (shortcuts == null)
			{
				shortcuts = new Collections.ShortcutCollection ();
				obj.SetLocalValue (Shortcut.ShortcutsProperty, shortcuts);
				obj.InvalidateProperty (Shortcut.ShortcutsProperty, null, shortcuts);
			}

			return shortcuts;
		}

		private static object GetKeyCodeValue(DependencyObject obj)
		{
			Shortcut that = (Shortcut) obj;
			return that.KeyCode.ToString ();
		}

		private static void SetKeyCodeValue(DependencyObject obj, object value)
		{
			Shortcut that = (Shortcut) obj;
			System.Enum keyCode;

			if (Types.InvariantConverter.Convert (value, typeof (KeyCode), out keyCode))
			{
				that.KeyCode = (KeyCode) keyCode;
			}
			else
			{
				that.KeyCode = KeyCode.None;
			}
		}
		
		#endregion

		public static readonly DependencyProperty ShortcutsProperty = DependencyProperty<Shortcut>.RegisterAttached ("Shortcuts", typeof (Collections.ShortcutCollection), new DependencyPropertyMetadata (Shortcut.GetShortcutsValue));
		public static readonly DependencyProperty KeyCodeProperty   = DependencyProperty<Shortcut>.Register (x => x.KeyCode, typeof (string), new DependencyPropertyMetadata (Shortcut.GetKeyCodeValue, Shortcut.SetKeyCodeValue));
		
		private KeyCode			keyCode;
	}
}
