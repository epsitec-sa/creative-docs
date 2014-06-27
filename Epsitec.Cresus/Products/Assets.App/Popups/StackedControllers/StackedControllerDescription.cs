//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups.StackedControllers
{
	public class StackedControllerDescription
	{
		public StackedControllerDescription()
		{
			this.labels = new List<string> ();
		}


		public string							Label;
		public StackedControllerType			StackedControllerType;
		public DateRangeCategory				DateRangeCategory;
		public DecimalFormat					DecimalFormat;
		public int								Width;
		public int								Height;
		public int								BottomMargin;

		public string							MultiLabels
		{
			//	Spécifie une liste de labels séparés par "<br/>".
			set
			{
				this.labels.Clear ();

				if (value.Contains ("<br/>"))
				{
					var lines = value.Split (new string[] { "<br/>" }, System.StringSplitOptions.RemoveEmptyEntries);
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


		public static AbstractStackedController CreateController(DataAccessor accessor, StackedControllerDescription description)
		{
			switch (description.StackedControllerType)
			{
				case StackedControllerType.Label:
					return new LabelStackedController (accessor, description);

				case StackedControllerType.Text:
					return new TextStackedController (accessor, description);

				case StackedControllerType.Int:
					return new IntStackedController (accessor, description);

				case StackedControllerType.Decimal:
					return new DecimalStackedController (accessor, description);

				case StackedControllerType.Date:
					return new DateStackedController (accessor, description);

				case StackedControllerType.Radio:
					return new RadioStackedController (accessor, description);

				case StackedControllerType.Combo:
					return new ComboStackedController (accessor, description);

				case StackedControllerType.Bool:
					return new BoolStackedController (accessor, description);

				case StackedControllerType.GroupGuid:
					return new GroupGuidStackedController (accessor, description);

				case StackedControllerType.CategoryGuid:
					return new CategoryGuidStackedController (accessor, description);

				case StackedControllerType.PersonGuid:
					return new PersonGuidStackedController (accessor, description);

				case StackedControllerType.ExportFilename:
					return new ExportFilenameStackedController (accessor, description);

				case StackedControllerType.ImportAccountsFilename:
					return new ImportAccountsFilenameStackedController (accessor, description);

				case StackedControllerType.Margins:
					return new MarginsStackedController (accessor, description);

				case StackedControllerType.PageSize:
					return new PageSizeStackedController (accessor, description);

				case StackedControllerType.PdfStyle:
					return new PdfStyleStackedController (accessor, description);

				default:
					throw new System.InvalidOperationException (string.Format ("Unsupported StackedControllerType {0}", description.StackedControllerType));
			}
		}


		private readonly List<string>			labels;
	}
}