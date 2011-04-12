//	Copyright © 2006-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets.Helpers;

using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>RadioIconGrid</c> class represents and manages a grid of icons
	/// where only one item can be selected at a given time.
	/// </summary>
	public class RadioIconGrid : AbstractGroup
	{
		public RadioIconGrid()
		{
			this.list = new List<RadioIcon> ();
			this.selectedValue = -1;
			this.enableEndOfLine = true;

			this.SetupController ();

			if (this.controller != null)
			{
				this.controller.Changed += this.HandleRadioChanged;
			}
		}

		public RadioIconGrid(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		protected virtual void SetupController()
		{
			this.controller = GroupController.GetGroupController (this, "GridGroup");
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (RadioIcon icon in this.list)
				{
					this.DetachRadioIcon (icon);
				}
				
				this.list.Clear ();

				if (this.controller != null)
				{
					this.controller.Changed -= this.HandleRadioChanged;
					this.controller = null;
				}
			}

			base.Dispose (disposing);
		}

		public void AddRadioIcon(string iconUri, string tooltip, int enumValue, bool endOfLine)
		{
			RadioIcon icon = new RadioIcon (this);
			
			icon.IconUri          = iconUri;
			icon.EnumValue         = enumValue;
			icon.EndOfLine         = endOfLine;
			icon.TabIndex          = this.list.Count + 1;
			icon.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			icon.Group             = this.controller == null ? null : this.controller.Group;
			
			ToolTip.Default.SetToolTip (icon, tooltip);
			
			this.list.Add (icon);

			this.AttachRadioIcon (icon);
		}

		protected virtual void AttachRadioIcon(RadioIcon icon)
		{
		}
		
		protected virtual void DetachRadioIcon(RadioIcon icon)
		{
		}


		public RadioIcon SelectedRadioIcon
		{
			get
			{
				foreach (RadioIcon icon in this.list)
				{
					if (this.IsIconSelected (icon))
					{
						return icon;
					}
				}

				return null;
			}
		}

		protected virtual bool IsIconSelected(RadioIcon icon)
		{
			if (icon.EnumValue == this.selectedValue)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public virtual int SelectedValue
		{
			get
			{
				return this.selectedValue;
			}

			set
			{
				if (this.selectedValue != value)
				{
					this.selectedValue = value;

					foreach (RadioIcon icon in this.list)
					{
						if (this.IsIconSelected (icon))
						{
							icon.ActiveState = ActiveState.Yes;
						}
						else
						{
							icon.ActiveState = ActiveState.No;
						}
					}

					this.OnSelectionChanged ();
				}
			}
		}

		public bool EnableEndOfLine
		{
			get
			{
				return this.enableEndOfLine;
			}

			set
			{
				this.enableEndOfLine = value;
			}
		}

		public override Drawing.Size GetBestFitSize()
		{
			if (this.list == null || this.list.Count == 0)
			{
				return Drawing.Size.Zero;
			}
			else
			{
				double dx = this.list[0].PreferredWidth;
				double dy = this.list[0].PreferredHeight;
				return new Drawing.Size(dx*this.list.Count, dy);
			}
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();

			Rectangle bounds = this.Client.Bounds;
			Point     corner = this.Client.Bounds.TopLeft;
			int       column = 0;
			int       row    = 0;

			foreach (RadioIcon icon in this.list)
			{
				System.Diagnostics.Debug.Assert (icon != null);

				Rectangle rect = new Rectangle (corner.X, corner.Y-icon.PreferredHeight, icon.PreferredWidth, icon.PreferredHeight);
				icon.SetManualBounds (rect);
				icon.Column = column;
				icon.Row    = row;
				icon.Index  = row * 1000 + column;
				icon.Visibility = bounds.Contains (rect);

				corner.X += icon.PreferredWidth;
				column++;

				if ((corner.X > this.Client.Bounds.Right-icon.PreferredWidth) ||
					(icon.EndOfLine && this.enableEndOfLine))
				{
					corner.X  = this.Client.Bounds.Left;
					corner.Y -= icon.PreferredHeight;
					column    = 0;
					row++;
				}
			}
		}


		private void HandleRadioChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.controller == sender);

			RadioIcon icon = this.controller.FindActiveWidget () as RadioIcon;

			if ((icon != null) &&
				(!this.IsIconSelected (icon)))
			{
				this.selectedValue = icon.EnumValue;
				this.OnSelectionChanged ();
			}
		}

		protected virtual void OnSelectionChanged()
		{
			var handler = this.GetUserEventHandler ("SelectionChanged");
			
			if (handler != null)
			{
				handler (this);
			}
		}


		public event EventHandler SelectionChanged
		{
			add
			{
				this.AddUserEventHandler ("SelectionChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("SelectionChanged", value);
			}
		}


		protected readonly List<RadioIcon>		list;
		protected GroupController				controller;
		private int								selectedValue;
		private bool							enableEndOfLine;
	}
}
