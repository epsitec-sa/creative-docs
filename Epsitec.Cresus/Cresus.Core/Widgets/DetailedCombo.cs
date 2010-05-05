//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	public class DetailedCombo : Widget
	{
		public DetailedCombo()
		{
		}

		public DetailedCombo(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		/// <summary>
		/// Autorise ou non les sélections multiples, ce qui se traduit par des CheckButtons ou des RadioButtons.
		/// Dans l'implémentation actuelle, le setter doit être appelé avant le setter de ComboInitializer !
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [allow multiple selection]; otherwise, <c>false</c>.
		/// </value>
		public bool AllowMultipleSelection
		{
			get;
			set;
		}

		public Accessors.ComboInitializer ComboInitializer
		{
			get
			{
				return this.comboInitializer;
			}
			set
			{
				this.comboInitializer = value;
				this.CreateUI ();
			}
		}


		public void CreateUI()
		{
			this.Children.Clear ();

			int tabIndex = 1;

			foreach (var pair in this.comboInitializer.Content)
			{
				AbstractButton button;

				if (this.AllowMultipleSelection)
				{
					button = new CheckButton
					{
						Parent = this,
						Name = pair.Key,
						Text = pair.Value,
						Dock = DockStyle.Top,
						TabIndex = tabIndex++,
					};
				}
				else
				{
					button = new RadioButton
					{
						Parent = this,
						Name = pair.Key,
						Text = pair.Value,
						Dock = DockStyle.Top,
						TabIndex = tabIndex++,
					};
				}

				button.ActiveStateChanged += new EventHandler (this.HandleButtonActiveStateChanged);
			}
		}


		private void TextToButtons()
		{
			string text = this.comboInitializer.ConvertEditionToInternal (this.Text);
			var words = Misc.Split (text.Replace (",", " "), " ");

			foreach (AbstractButton button in this.Children)
			{
				button.ActiveState = (words.Contains (button.Name)) ? Common.Widgets.ActiveState.Yes : Common.Widgets.ActiveState.No;
			}
		}

		private void ButtonsToText()
		{
			var words = new List<string> ();

			foreach (AbstractButton button in this.Children)
			{
				if (button.IsActive)
				{
					words.Add (button.Text);
				}
			}

			this.ignoreChange = true;
			this.Text = Misc.Join (words, ", ");
			this.ignoreChange = false;
		}


		protected override void OnTextChanged()
		{
			base.OnTextChanged ();

			if (!this.ignoreChange)
			{
				this.TextToButtons ();
			}
		}


		private void HandleButtonActiveStateChanged(object sender)
		{
			this.ButtonsToText ();
		}


		private Accessors.ComboInitializer comboInitializer;
		private bool ignoreChange;
	}
}
