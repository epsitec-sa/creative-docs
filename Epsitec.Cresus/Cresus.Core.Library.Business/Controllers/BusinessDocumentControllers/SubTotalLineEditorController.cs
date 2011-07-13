//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class SubTotalLineEditorController : AbstractLineEditorController
	{
		public SubTotalLineEditorController(AccessData accessData)
			: base (accessData)
		{
		}

		protected override void CreateUI(UIBuilder builder)
		{
			var box = new FrameBox
			{
				Parent = this.tileContainer,
				Dock = DockStyle.Fill,
				Padding = new Margins (10),
				TabIndex = this.NextTabIndex,
			};

			var line1 = new FrameBox
			{
				Parent = box,
				Dock = DockStyle.Top,
				PreferredHeight = 20,
				Margins = new Margins (0, 0, 0, 20),
				TabIndex = this.NextTabIndex,
			};

			var line2 = new FrameBox
			{
				Parent = box,
				Dock = DockStyle.Top,
				PreferredHeight = 20,
				Margins = new Margins (0, 0, 0, 5),
				TabIndex = this.NextTabIndex,
			};

			var line3 = new FrameBox
			{
				Parent = box,
				Dock = DockStyle.Top,
				PreferredHeight = 20,
				Margins = new Margins (0, 0, 0, 5),
				TabIndex = this.NextTabIndex,
			};

			var line4 = new FrameBox
			{
				Parent = box,
				Dock = DockStyle.Top,
				PreferredHeight = 20,
				Margins = new Margins (0, 0, 0, 5),
				TabIndex = this.NextTabIndex,
			};

			var line5 = new FrameBox
			{
				Parent = box,
				Dock = DockStyle.Top,
				PreferredHeight = 20,
				Margins = new Margins (0, 0, 0, 5),
				TabIndex = this.NextTabIndex,
			};

			//	Rabais.
			{
				var field = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.DiscountText, x => this.DiscountText = x));
				this.PlaceLabelAndField (line1, 200, 100, "Rabais en % ou en francs", field);
			}

			//	Textes.
			{
				var field = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Entity.TextForPrimaryPrice, x => this.Entity.TextForPrimaryPrice = x));
				this.PlaceLabelAndField (line2, 200, 400, "Texte pour sous-total avant rabais", field);
			}
			{
				var field = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Entity.TextForDiscount, x => this.Entity.TextForDiscount = x));
				this.PlaceLabelAndField (line3, 200, 400, "Texte pour le rabais", field);
			}
			{
				var field = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Entity.TextForResultingPrice, x => this.Entity.TextForResultingPrice = x));
				this.PlaceLabelAndField (line4, 200, 400, "Texte pour sous-total après rabais", field);
			}
		}

		public override FormattedText TitleTile
		{
			get
			{
				return "Sous-total";
			}
		}


		private string DiscountText
		{
			get
			{
				System.Diagnostics.Debug.Assert (this.Entity.Discount != null);

				if (this.Entity.Discount.DiscountRate.HasValue && this.Entity.Discount.DiscountRate.Value != 0)
				{
					return Misc.PercentToString (this.Entity.Discount.DiscountRate);
				}

				if (this.Entity.Discount.Value.HasValue && this.Entity.Discount.Value.Value != 0)
				{
					return Misc.PriceToString (this.Entity.Discount.Value);
				}

				return null;
			}
			set
			{
				System.Diagnostics.Debug.Assert (this.Entity.Discount != null);

				if (string.IsNullOrEmpty (value))
				{
					this.Entity.Discount.DiscountRate = null;
					this.Entity.Discount.Value = null;
				}
				else
				{
					if (value.Contains ("%"))
					{
						value = value.Replace ("%", "");

						decimal d;
						if (decimal.TryParse (value, out d))
						{
							this.Entity.Discount.DiscountRate = d/100;
							this.Entity.Discount.Value = null;
						}
					}
					else
					{
						decimal d;
						if (decimal.TryParse (value, out d))
						{
							this.Entity.Discount.DiscountRate = null;
							this.Entity.Discount.Value = d;
						}
					}
				}
			}
		}


		private SubTotalDocumentItemEntity Entity
		{
			get
			{
				return this.entity as SubTotalDocumentItemEntity;
			}
		}
	}
}
