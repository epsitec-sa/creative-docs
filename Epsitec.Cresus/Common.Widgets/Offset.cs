//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Offset permet de modifier un offset (x;y).
	/// </summary>
	public class Offset : AbstractGroup
	{
		public Offset()
		{
			this.AutoEngage = true;
			this.AutoRepeat = true;
			this.InternalState |= InternalState.Engageable;

			this.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;

			Widget upperBand = new Widget(this);
			upperBand.Dock = DockStyle.StackFill;
			upperBand.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			Widget middleBand = new Widget(this);
			middleBand.Dock = DockStyle.StackFill;
			middleBand.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			Widget lowerBand = new Widget(this);
			lowerBand.Dock = DockStyle.StackFill;
			lowerBand.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			Widget foo;

			//	Bande supérieure.
			foo = new Widget(upperBand);
			foo.Dock = DockStyle.StackFill;

			this.buttonUp = new GlyphButton(upperBand);
			this.buttonUp.GlyphShape = GlyphShape.ArrowUp;
			this.buttonUp.Dock = DockStyle.StackFill;
			this.buttonUp.Engaged += new EventHandler(this.HandleButton);
			this.buttonUp.StillEngaged += new EventHandler(this.HandleButton);

			foo = new Widget(upperBand);
			foo.Dock = DockStyle.StackFill;

			//	Bande médiane.
			this.buttonLeft = new GlyphButton(middleBand);
			this.buttonLeft.GlyphShape = GlyphShape.ArrowLeft;
			this.buttonLeft.Dock = DockStyle.StackFill;
			this.buttonLeft.Engaged += new EventHandler(this.HandleButton);
			this.buttonLeft.StillEngaged += new EventHandler(this.HandleButton);

			this.buttonCenter = new GlyphButton(middleBand);
			this.buttonCenter.GlyphShape = GlyphShape.Close;
			this.buttonCenter.Dock = DockStyle.StackFill;
			this.buttonCenter.Engaged += new EventHandler(this.HandleButton);
			this.buttonCenter.StillEngaged += new EventHandler(this.HandleButton);

			this.buttonRight = new GlyphButton(middleBand);
			this.buttonRight.GlyphShape = GlyphShape.ArrowRight;
			this.buttonRight.Dock = DockStyle.StackFill;
			this.buttonRight.Engaged += new EventHandler(this.HandleButton);
			this.buttonRight.StillEngaged += new EventHandler(this.HandleButton);

			//	Bande inférieure.
			foo = new Widget(lowerBand);
			foo.Dock = DockStyle.StackFill;

			this.buttonDown = new GlyphButton(lowerBand);
			this.buttonDown.GlyphShape = GlyphShape.ArrowDown;
			this.buttonDown.Dock = DockStyle.StackFill;
			this.buttonDown.Engaged += new EventHandler(this.HandleButton);
			this.buttonDown.StillEngaged += new EventHandler(this.HandleButton);

			foo = new Widget(lowerBand);
			foo.Dock = DockStyle.StackFill;
		}

		public Offset(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.buttonLeft.Engaged -= new EventHandler(this.HandleButton);
				this.buttonLeft.StillEngaged -= new EventHandler(this.HandleButton);

				this.buttonRight.Engaged -= new EventHandler(this.HandleButton);
				this.buttonRight.StillEngaged -= new EventHandler(this.HandleButton);

				this.buttonDown.Engaged -= new EventHandler(this.HandleButton);
				this.buttonDown.StillEngaged -= new EventHandler(this.HandleButton);

				this.buttonUp.Engaged -= new EventHandler(this.HandleButton);
				this.buttonUp.StillEngaged -= new EventHandler(this.HandleButton);

				this.buttonCenter.Engaged -= new EventHandler(this.HandleButton);
				this.buttonCenter.StillEngaged -= new EventHandler(this.HandleButton);
			}

			base.Dispose(disposing);
		}


		public Point OffsetValue
		{
			get
			{
				return this.offset;
			}
			set
			{
				if (this.offset != value)
				{
					this.offset = value;
					this.OnOffsetValueChanged();
				}
			}
		}


		private void HandleButton(object sender)
		{
			//	Bouton pressé ou maintenu pressé.
			GlyphButton button = sender as GlyphButton;

			Point move = Point.Zero;

			if (button == this.buttonLeft)
			{
				move.X = -this.step;
			}

			if (button == this.buttonRight)
			{
				move.X = this.step;
			}

			if (button == this.buttonDown)
			{
				move.Y = -this.step;
			}

			if (button == this.buttonUp)
			{
				move.Y = this.step;
			}

			this.OffsetValue += move;
		}


		#region Events handler
		protected virtual void OnOffsetValueChanged()
		{
			//	Génère un événement pour dire que l'offset a changé.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("OffsetValueChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Support.EventHandler OffsetValueChanged
		{
			add
			{
				this.AddUserEventHandler("OffsetValueChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("OffsetValueChanged", value);
			}
		}
		#endregion


		protected Point						offset;
		protected double					step = 1;

		protected GlyphButton				buttonLeft;
		protected GlyphButton				buttonRight;
		protected GlyphButton				buttonDown;
		protected GlyphButton				buttonUp;
		protected GlyphButton				buttonCenter;
	}
}
