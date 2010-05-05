//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	public class HintEditor : TextFieldEx, Common.Widgets.Collections.IStringCollectionHost, Common.Support.Data.INamedStringSelection
	{
		public HintEditor()
		{
			this.TextDisplayMode = TextFieldDisplayMode.ActiveHint;
			this.DefocusAction = Common.Widgets.DefocusAction.AcceptEdition;

			this.items = new Common.Widgets.Collections.StringCollection (this);
			this.selectedRow = -1;
		}

		public HintEditor(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		#region IStringCollectionHost Members

		public void NotifyStringCollectionChanged()
		{
		}

		public Common.Widgets.Collections.StringCollection Items
		{
			get
			{
				return this.items;
			}
		}

		#endregion

		#region INamedStringSelection Members

		public string SelectedName
		{
			get
			{
				if (this.selectedRow == -1)
				{
					return null;
				}

				return this.items.GetName (this.selectedRow);
			}
			set
			{
				if (this.SelectedName != value)
				{
					int index = -1;

					if (value != null)
					{
						index = this.items.FindIndexByName (value);

						if (index < 0)
						{
							throw new System.ArgumentException (string.Format ("No element named '{0}' in list", value));
						}
					}

					this.SelectedIndex = index;
				}
			}
		}

		#endregion

		#region IStringSelection Members

		public int SelectedIndex
		{
			get
			{
				return this.selectedRow;
			}
			set
			{
				if (value != -1)
				{
					value = System.Math.Max (value, 0);
					value = System.Math.Min (value, this.items.Count-1);
				}
				if (value != this.selectedRow)
				{
					this.selectedRow = value;
					this.OnSelectedIndexChanged ();
				}
			}
		}

		public string SelectedItem
		{
			get
			{
				int index = this.SelectedIndex;
				
				if (index < 0)
				{
					return null;
				}

				return this.Items[index];
			}
			set
			{
				this.SelectedIndex = this.Items.IndexOf (value);
			}
		}

		public event EventHandler  SelectedIndexChanged
		{
			add
			{
				this.AddUserEventHandler("SelectedIndexChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("SelectedIndexChanged", value);
			}
		}

		#endregion



		private void HintSearching(string typed)
		{
			typed = Misc.RemoveAccentsToLower (typed);

			if (!string.IsNullOrEmpty (typed))
			{
				foreach (var item in this.items)
				{
					string name = item.ToString ();

					if (Misc.RemoveAccentsToLower (name).Contains (typed))
					{
						this.SelectedItem = name;
						this.HintText = name;
						this.SetError (false);
						return;
					}
				}
			}

			this.HintText = null;
			this.SetError (!string.IsNullOrEmpty (this.Text));
		}


		protected virtual void OnSelectedIndexChanged()
		{
			//	Génère un événement pour dire que la sélection dans la liste a changé.
			EventHandler handler = (EventHandler) this.GetUserEventHandler ("SelectedIndexChanged");
			if (handler != null)
			{
				handler (this);
			}
		}

		protected override bool AboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			// TODO: Ne fonctionne toujours pas.
			this.SelectAll ();
			return base.AboutToGetFocus (dir, mode, out focus);
		}

		protected override void OnTextChanged()
		{
			base.OnTextChanged ();

			this.HintSearching (this.Text);
		}

		protected override void OnEditionAccepted()
		{
			base.OnEditionAccepted ();

			this.Text = this.HintText;
		}


		private readonly Common.Widgets.Collections.StringCollection items;
		private int selectedRow;
	}
}
