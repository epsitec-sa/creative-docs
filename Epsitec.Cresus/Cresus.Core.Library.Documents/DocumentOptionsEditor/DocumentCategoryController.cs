//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.PlugIns;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.DocumentOptionsEditor
{
	public sealed class DocumentCategoryController : System.IDisposable
	{
		public DocumentCategoryController(IBusinessContext businessContext, DocumentCategoryEntity documentCategoryEntity)
		{
			System.Diagnostics.Debug.Assert (businessContext != null);
			System.Diagnostics.Debug.Assert (documentCategoryEntity.IsNotNull ());

			this.businessContext        = businessContext;
			this.documentCategoryEntity = documentCategoryEntity;

			this.optionInformations = new List<OptionInformation> ();
			
			this.businessContext.SavingChanges += this.HandleBusinessContextSavingChanges;
		}



		public void CreateUI(Widget parent)
		{
			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, 0),
			};

			this.CreateDocumentType (box);

			this.checkButtonsFrame = new FrameBox
			{
				Parent = box,
				Dock = DockStyle.Fill,
			};

			this.CreateCheckButtons (this.checkButtonsFrame);
		}

		public void CreateDocumentType(Widget parent)
		{
			this.CreateTitle (parent, "Type du document", false);

			var combo = new TextFieldCombo
			{
				Parent = parent,
				IsReadOnly = true,
				Dock = DockStyle.Top,
			};

			var types = EnumKeyValues.FromEnum<DocumentType> ();
			foreach (var type in types)
			{
				if (type.Key != DocumentType.None   &&
					type.Key != DocumentType.Unknown)
				{
					combo.Items.Add (type.Key.ToString (), type.Values[0]);

					if (type.Key == this.documentCategoryEntity.DocumentType)
					{
						combo.SelectedItemIndex = combo.Items.Count-1;
					}
				}
			}

			combo.SelectedItemChanged += delegate
			{
				string key = combo.Items.GetKey (combo.SelectedItemIndex);
				this.documentCategoryEntity.DocumentType = (DocumentType) System.Enum.Parse (typeof (DocumentType), key);

				this.CreateCheckButtons (this.checkButtonsFrame);
			};
		}

		public void CreateCheckButtons(Widget parent)
		{
			parent.Children.Clear ();

			this.UpdateOptionInformations ();

			//	Premier choix.
			{
				var extract = this.optionInformations.Where (x => x.Used != 0 && x.Used == x.Total);
				if (extract.Any ())
				{
					this.CreateTitle (parent, "Options parfaitement adaptées", true);

					foreach (var optionInformation in extract)
					{
						this.CreateCheckButton (parent, optionInformation);
					}
				}
			}

			//	Deuxième choix.
			{
				var extract = this.optionInformations.Where (x => x.Used != 0 && x.Used < x.Total);
				if (extract.Any ())
				{
					this.CreateTitle (parent, "Options partiellement adaptées", true);

					foreach (var optionInformation in extract)
					{
						this.CreateCheckButton (parent, optionInformation);
					}
				}
			}

			//	Dernier choix.
			{
				var extract = this.optionInformations.Where (x => x.Used == 0);
				if (extract.Any ())
				{
					this.CreateTitle (parent, "Options pas adaptées", true);

					foreach (var optionInformation in extract)
					{
						this.CreateCheckButton (parent, optionInformation);
					}
				}
			}
		}

		private void CreateTitle(Widget parent, FormattedText title, bool hasTopMargin)
		{
			new StaticText
			{
				Parent = parent,
				FormattedText = FormattedText.Concat (title, " :"),
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, hasTopMargin ? 10 : 0, 2),
			};
		}

		private CheckButton CreateCheckButton(Widget parent, OptionInformation optionInformation)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				PreferredHeight = 15,
				Dock = DockStyle.Top,
			};

			var button = new CheckButton
			{
				Parent = frame,
				FormattedText = optionInformation.Entity.Name,
				Dock = DockStyle.Fill,
			};

			new StaticText
			{
				Parent = frame,
				Text = string.Concat (optionInformation.Used.ToString (), "/", optionInformation.Total.ToString ()),
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth = 40,
				Dock = DockStyle.Right,
			};

			return button;
		}


		private void UpdateOptionInformations()
		{
			this.documentOptions = Epsitec.Cresus.Core.Documents.External.CresusCore.GetRequiredDocumentOptionsByDocumentType (this.documentCategoryEntity.DocumentType);
			this.optionInformations.Clear ();

			var optionEntities = this.businessContext.GetAllEntities<DocumentOptionsEntity> ();
			foreach (var optionEntity in optionEntities)
			{
				this.optionInformations.Add (this.GetOptionInformation (optionEntity));
			}
		}

		private OptionInformation GetOptionInformation(DocumentOptionsEntity optionEntity)
		{
			int count = 0;
			int total = 0;

			if (this.documentOptions != null)
			{
				PrintingOptionDictionary printingOptionDictionary = optionEntity.GetOptions ();

				foreach (var option in printingOptionDictionary.Options)
				{
					if (this.documentOptions.Contains (option))
					{
						count++;
					}

					total++;
				}
			}

			return new OptionInformation (optionEntity, count, total);
		}

		private class OptionInformation
		{
			public OptionInformation(DocumentOptionsEntity entity, int used, int total)
			{
				this.Entity = entity;
				this.Used   = used;
				this.Total  = total;
			}

			public DocumentOptionsEntity Entity
			{
				get;
				private set;
			}

			public int Used
			{
				get;
				private set;
			}

			public int Total
			{
				get;
				private set;
			}
		}




		private void HandleBusinessContextSavingChanges(object sender, CancelEventArgs e)
		{
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.businessContext.SavingChanges -= this.HandleBusinessContextSavingChanges;
		}

		#endregion


		private readonly IBusinessContext					businessContext;
		private readonly DocumentCategoryEntity				documentCategoryEntity;
		private readonly List<OptionInformation>			optionInformations;

		private FrameBox									checkButtonsFrame;
		private IEnumerable<DocumentOption>					documentOptions;
	}
}
