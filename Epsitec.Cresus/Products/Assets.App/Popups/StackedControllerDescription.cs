//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;
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
				this.labels.Add (value);
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
					case StackedControllerType.Date:
						return DateController.controllerHeight;

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
					case StackedControllerType.Date:
						return DateController.controllerWidth;

					default:
						throw new System.InvalidOperationException (string.Format ("Unsupported StackedControllerType {0}", this.StackedControllerType));
				}
			}
		}

		public int								RequiredLabelsWidth
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
				case StackedControllerType.Date:
					return new DateStackedController (accessor);

				default:
					throw new System.InvalidOperationException (string.Format ("Unsupported StackedControllerType {0}", description.StackedControllerType));
			}
		}


		private readonly List<string>			labels;
	}
}