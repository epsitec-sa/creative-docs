//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
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


		public int								RequiredHeight
		{
			get
			{
				switch (this.StackedControllerType)
				{
					case StackedControllerType.Label:
						return this.Label.GetTextHeight (this.Width);

					case StackedControllerType.Text:
					case StackedControllerType.ExportFilename:
					case StackedControllerType.ImportAccountsFilename:
					case StackedControllerType.Combo:
						return TextStackedController.height;

					case StackedControllerType.Int:
						return IntStackedController.height;

					case StackedControllerType.Decimal:
						return DecimalStackedController.height;

					case StackedControllerType.Date:
						return DateStackedController.height;

					case StackedControllerType.Radio:
						return this.labels.Count * RadioStackedController.radioHeight;

					case StackedControllerType.Bool:
						return BoolStackedController.checkHeight;

					case StackedControllerType.GroupGuid:
					case StackedControllerType.CategoryGuid:
					case StackedControllerType.PersonGuid:
						return this.Height;

					case StackedControllerType.Margins:
						return MarginsStackedController.height;

					case StackedControllerType.PageSize:
						return PageSizeStackedController.height;

					case StackedControllerType.PdfStyle:
						return PdfStyleStackedController.height;

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
					case StackedControllerType.Label:
					case StackedControllerType.Text:
					case StackedControllerType.ExportFilename:
					case StackedControllerType.ImportAccountsFilename:
					case StackedControllerType.GroupGuid:
					case StackedControllerType.CategoryGuid:
					case StackedControllerType.PersonGuid:
					case StackedControllerType.Combo:
					case StackedControllerType.Margins:
					case StackedControllerType.PageSize:
					case StackedControllerType.PdfStyle:
						return this.Width + 4;

					case StackedControllerType.Int:
						return IntStackedController.width + 38;  // 38 -> place pour les boutons -/+

					case StackedControllerType.Decimal:
						return DecimalStackedController.width + 10 + 50 + 4;  // place pour les unités

 					case StackedControllerType.Date:
						return DateStackedController.width;

					case StackedControllerType.Radio:
						return 30 + this.LabelsWidth;

					case StackedControllerType.Bool:
						return 30 + this.Label.GetTextWidth ();

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
					case StackedControllerType.ExportFilename:
					case StackedControllerType.ImportAccountsFilename:
					case StackedControllerType.Int:
					case StackedControllerType.Decimal:
					case StackedControllerType.Date:
					case StackedControllerType.GroupGuid:
					case StackedControllerType.CategoryGuid:
					case StackedControllerType.PersonGuid:
					case StackedControllerType.Combo:
					case StackedControllerType.Margins:
					case StackedControllerType.PageSize:
					case StackedControllerType.PdfStyle:
						return this.Label.GetTextWidth ();

					case StackedControllerType.Label:
					case StackedControllerType.Radio:
					case StackedControllerType.Bool:
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
				return this.labels.Select (x => x.GetTextWidth ()).Max ();
			}
		}


		public static AbstractStackedController CreateController(DataAccessor accessor, StackedControllerDescription description)
		{
			switch (description.StackedControllerType)
			{
				case StackedControllerType.Label:
					return new LabelStackedController (accessor);

				case StackedControllerType.Text:
					return new TextStackedController (accessor);

				case StackedControllerType.Int:
					return new IntStackedController (accessor);

				case StackedControllerType.Decimal:
					return new DecimalStackedController (accessor);

				case StackedControllerType.Date:
					return new DateStackedController (accessor);

				case StackedControllerType.Radio:
					return new RadioStackedController (accessor);

				case StackedControllerType.Combo:
					return new ComboStackedController (accessor);

				case StackedControllerType.Bool:
					return new BoolStackedController (accessor);

				case StackedControllerType.GroupGuid:
					return new GroupGuidStackedController (accessor);

				case StackedControllerType.CategoryGuid:
					return new CategoryGuidStackedController (accessor);

				case StackedControllerType.PersonGuid:
					return new PersonGuidStackedController (accessor);

				case StackedControllerType.ExportFilename:
					return new ExportFilenameStackedController (accessor);

				case StackedControllerType.ImportAccountsFilename:
					return new ImportAccountsFilenameStackedController (accessor);

				case StackedControllerType.Margins:
					return new MarginsStackedController (accessor);

				case StackedControllerType.PageSize:
					return new PageSizeStackedController (accessor);

				case StackedControllerType.PdfStyle:
					return new PdfStyleStackedController (accessor);

				default:
					throw new System.InvalidOperationException (string.Format ("Unsupported StackedControllerType {0}", description.StackedControllerType));
			}
		}


		private readonly List<string>			labels;
	}
}