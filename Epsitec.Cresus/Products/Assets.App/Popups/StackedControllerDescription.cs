//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class StackedControllerDescription
	{
		public StackedControllerDescription()
		{
			this.labels = new List<string> ();
		}


		public StackedControllerType			StackedControllerType;
		public int								Width;
		public int								BottomMargin;

		public string							Label
		{
			get
			{
				if (this.labels.Any ())
				{
					return this.labels[0];
				}
				else
				{
					return null;
				}
			}
			set
			{
				this.labels.Clear ();

				if (value.Contains ("<br/>"))
				{
					var lines = value.Split (new string[] {"<br/>"}, System.StringSplitOptions.RemoveEmptyEntries);
					this.labels.AddRange (lines);
				}
				else
				{
					this.labels.Add (value);
				}
			}
		}

		public List<string>						Labels
		{
			get
			{
				return this.labels;
			}
		}


		public int								RequiredHeight
		{
			get
			{
				switch (this.StackedControllerType)
				{
					case StackedControllerType.Text:
						return TextStackedController.height;

					case StackedControllerType.Int:
						return IntStackedController.height;

					case StackedControllerType.Date:
						return DateStackedController.height;

					case StackedControllerType.Radio:
						return this.labels.Count * RadioStackedController.radioHeight;

					default:
						throw new System.InvalidOperationException (string.Format ("Unsupported StackedControllerType {0}", this.StackedControllerType));
				}
			}
		}

		public int								RequiredControllerWidth
		{
			get
			{
				switch (this.StackedControllerType)
				{
					case StackedControllerType.Text:
						return this.Width;

					case StackedControllerType.Int:
						return IntStackedController.width;

					case StackedControllerType.Date:
						return DateStackedController.width;

					case StackedControllerType.Radio:
						return 20 + this.LabelsWidth;

					default:
						throw new System.InvalidOperationException (string.Format ("Unsupported StackedControllerType {0}", this.StackedControllerType));
				}
			}
		}

		public int								RequiredLabelsWidth
		{
			get
			{
				switch (this.StackedControllerType)
				{
					case StackedControllerType.Text:
					case StackedControllerType.Int:
					case StackedControllerType.Date:
						return this.LabelsWidth;

					case StackedControllerType.Radio:
						return 0;

					default:
						throw new System.InvalidOperationException (string.Format ("Unsupported StackedControllerType {0}", this.StackedControllerType));
				}
			}
		}

		private int								LabelsWidth
		{
			get
			{
				int width = 0;

				foreach (var label in this.labels)
				{
					width = System.Math.Max (width, label.GetTextWidth ());
				}

				return width;
			}
		}


		public static AbstractStackedController CreateController(DataAccessor accessor, StackedControllerDescription description)
		{
			switch (description.StackedControllerType)
			{
				case StackedControllerType.Text:
					return new TextStackedController (accessor);

				case StackedControllerType.Int:
					return new IntStackedController (accessor);

				case StackedControllerType.Date:
					return new DateStackedController (accessor);

				case StackedControllerType.Radio:
					return new RadioStackedController (accessor);

				default:
					throw new System.InvalidOperationException (string.Format ("Unsupported StackedControllerType {0}", description.StackedControllerType));
			}
		}


		private readonly List<string>			labels;
	}
}