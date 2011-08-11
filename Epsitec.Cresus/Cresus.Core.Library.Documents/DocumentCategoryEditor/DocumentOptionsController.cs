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

namespace Epsitec.Cresus.Core.DocumentCategoryController
{
	public sealed class DocumentOptionsController
	{
		public DocumentOptionsController(DocumentCategoryController documentCategoryController)
		{
			this.documentCategoryController = documentCategoryController;
			this.businessContext            = this.documentCategoryController.BusinessContext;
			this.documentCategoryEntity     = this.documentCategoryController.DocumentCategoryEntity;

			this.optionInformations = new List<OptionInformation> ();
			this.optionGroups = new List<OptionGroup> ();
			this.errorOptions = new List<DocumentOption> ();
		}


		public void CreateUI(Widget parent)
		{
			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.checkButtonsFrame = new Scrollable
			{
				Parent = box,
				Dock = DockStyle.Fill,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode = ScrollableScrollerMode.Auto,
				PaintViewportFrame = true,
				Margins = new Margins (0, 0, 10, 0),
			};
			this.checkButtonsFrame.Viewport.IsAutoFitting = true;
			this.checkButtonsFrame.ViewportPadding = new Margins (-1);

			this.CreateCheckButtons ();

			this.CreateMissing (box);
			this.CreateOverflow (box);
			this.CreateError (box);
			this.UpdateErrorAndWarning ();
		}

		public void UpdateAfterDocumentTypeChanged()
		{
			this.CreateCheckButtons ();
			this.UpdateErrorAndWarning ();
		}


		private void CreateError(Widget parent)
		{
			this.errorFrame = new FrameBox
			{
				Parent = parent,
				DrawFullFrame = true,
				BackColor = Color.FromBrightness (1),
				PreferredHeight = this.documentCategoryController.errorHeight,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, -1, 0),
			};

			this.errorText = new StaticText
			{
				Parent = this.errorFrame,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleLeft,
				Dock = DockStyle.Fill,
				Margins = new Margins (5, 0, 0, 0),
			};
		}

		private void CreateOverflow(Widget parent)
		{
			this.overflowFrame = new FrameBox
			{
				Parent = parent,
				DrawFullFrame = true,
				BackColor = Color.FromBrightness (1),
				PreferredHeight = this.documentCategoryController.errorHeight,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, -1, 0),
			};

			this.overflowText = new StaticText
			{
				Parent = this.overflowFrame,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleLeft,
				Dock = DockStyle.Fill,
				Margins = new Margins (5, 0, 0, 0),
			};
		}

		private void CreateMissing(Widget parent)
		{
			this.missingFrame = new FrameBox
			{
				Parent = parent,
				DrawFullFrame = true,
				BackColor = Color.FromBrightness (1),
				PreferredHeight = this.documentCategoryController.errorHeight,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, -1, 0),
			};

			this.missingText = new StaticText
			{
				Parent = this.missingFrame,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleLeft,
				Dock = DockStyle.Fill,
				Margins = new Margins (5+12, 0, 0, 0),
			};
		}

		private void CreateCheckButtons()
		{
			this.UpdateData ();

			var parent = this.checkButtonsFrame.Viewport;
			parent.Children.Clear ();

			this.firstGroup = true;
			this.CreateGroup (parent, this.optionGroups.Where (x => x.Used != 0 && x.Used == x.Total), "Options parfaitement adaptées",  this.documentCategoryController.acceptedColor);   // vert clair
			this.CreateGroup (parent, this.optionGroups.Where (x => x.Used != 0 && x.Used <  x.Total), "Options partiellement adaptées", this.documentCategoryController.toleratedColor);  // orange clair
			this.CreateGroup (parent, this.optionGroups.Where (x => x.Used == 0                     ), "Options inadaptées",             this.documentCategoryController.rejectedColor);   // rouge clair
		}

		private void CreateGroup(Widget parent, IEnumerable<OptionGroup> optionGroups, FormattedText title, Color color)
		{
			if (optionGroups.Any ())
			{
				var frame = this.CreateColorizedFrameBox (parent, color);
				this.CreateTitle (frame, title);

				foreach (var group in optionGroups)
				{
					this.CreateCheckButton (frame, group);
				}
			}
		}

		private FrameBox CreateColorizedFrameBox(Widget parent, Color color)
		{
			var box = new FrameBox
			{
				Parent = parent,
				DrawFullFrame = true,
				BackColor = color,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, this.firstGroup ? 0 : -1, 0),
				Padding = new Margins (5),
			};

			this.firstGroup = false;

			return box;
		}

		private void CreateTitle(Widget parent, FormattedText title)
		{
			new StaticText
			{
				Parent = parent,
				FormattedText = FormattedText.Concat (title, " :"),
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
			};
		}

		private void CreateCheckButton(Widget parent, OptionGroup group)
		{
			int index = this.optionGroups.IndexOf (group);

			if (group.IsRadio)
			{
				var frame = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Top,
				};

				var errorFrame = new FrameBox
				{
					Parent = frame,
					PreferredWidth = 10,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, this.documentCategoryController.lineHeight-2, 0),
				};

				var overflowFrame = new FrameBox
				{
					Parent = frame,
					PreferredWidth = 10,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, this.documentCategoryController.lineHeight-2, 0),
				};

				var leftFrame = new FrameBox
				{
					Parent = frame,
					Dock = DockStyle.Fill,
				};

				var rightFrame = new FrameBox
				{
					Parent = frame,
					PreferredWidth = 40,
					Dock = DockStyle.Right,
				};

				new StaticText
				{
					Parent = rightFrame,
					Text = group.Ratio,
					ContentAlignment = ContentAlignment.MiddleRight,
					Dock = DockStyle.Fill,
				};

				var first = new RadioButton
				{
					Parent = leftFrame,
					FormattedText = "Aucune option",
					Name = string.Concat (index.ToString (), ".-1"),
					Group = group.Name,
					PreferredHeight = this.documentCategoryController.lineHeight,
					Dock = DockStyle.Top,
				};

				var firstState = ActiveState.Yes;

				for (int i = 0; i < group.OptionInformations.Count; i++)
				{
					var error = new StaticText
					{
						Parent = errorFrame,
						ContentAlignment = ContentAlignment.MiddleLeft,
						PreferredWidth = 10,
						PreferredHeight = this.documentCategoryController.lineHeight,
						Dock = DockStyle.Top,
					};

					group.OptionInformations[i].ErrorVisibility = errorFrame;
					group.OptionInformations[i].ErrorText = error;

					var overflow = new StaticText
					{
						Parent = overflowFrame,
						ContentAlignment = ContentAlignment.MiddleLeft,
						PreferredWidth = 10,
						PreferredHeight = this.documentCategoryController.lineHeight,
						Dock = DockStyle.Top,
					};

					group.OptionInformations[i].OverflowVisibility = overflowFrame;
					group.OptionInformations[i].OverflowText = overflow;

					var entity = group.OptionInformations[i].Entity;
					bool check = this.documentCategoryEntity.DocumentOptions.Contains (entity);

					if (check)
					{
						firstState = ActiveState.No;
					}

					var button = new RadioButton
					{
						Parent = leftFrame,
						FormattedText = entity.Name,
						Name = string.Concat (index.ToString (), ".", i.ToString ()),
						Group = group.Name,
						ActiveState = check ? ActiveState.Yes : ActiveState.No,
						PreferredHeight = this.documentCategoryController.lineHeight,
						Dock = DockStyle.Top,
						Margins = new Margins (0, 0, 0, (i == group.OptionInformations.Count-1) ? 5 : 0),
					};

					group.OptionInformations[i].Button = button;

					button.ActiveStateChanged += delegate
					{
						this.ButtonClicked (button);
					};
				}

				first.ActiveState = firstState;

				first.ActiveStateChanged += delegate
				{
					this.ButtonClicked (first);
				};
			}
			else
			{
				var frame = new FrameBox
				{
					Parent = parent,
					PreferredHeight = this.documentCategoryController.lineHeight,
					Dock = DockStyle.Top,
				};

				var error = new StaticText
				{
					Parent = frame,
					ContentAlignment = ContentAlignment.MiddleLeft,
					PreferredWidth = 10,
					PreferredHeight = this.documentCategoryController.lineHeight-3,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, 0, 3),
				};

				group.OptionInformations[0].ErrorVisibility = error;
				group.OptionInformations[0].ErrorText = error;

				var overflow = new StaticText
				{
					Parent = frame,
					ContentAlignment = ContentAlignment.MiddleLeft,
					PreferredWidth = 10,
					PreferredHeight = this.documentCategoryController.lineHeight-3,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, 0, 3),
				};

				group.OptionInformations[0].OverflowVisibility = overflow;
				group.OptionInformations[0].OverflowText = overflow;

				var entity = group.OptionInformations[0].Entity;
				bool check = this.documentCategoryEntity.DocumentOptions.Contains (entity);

				var button = new CheckButton
				{
					Parent = frame,
					FormattedText = entity.Name,
					Name = index.ToString (),
					ActiveState = check ? ActiveState.Yes : ActiveState.No,
					Dock = DockStyle.Fill,
				};

				group.OptionInformations[0].Button = button;

				button.ActiveStateChanged += delegate
				{
					this.ButtonClicked (button);
				};

				new StaticText
				{
					Parent = frame,
					Text = group.Ratio,
					ContentAlignment = ContentAlignment.MiddleRight,
					PreferredWidth = 40,
					Dock = DockStyle.Right,
				};
			}
		}

		private void SetTooltip(Widget button, OptionInformation info)
		{
			var description = this.GetTooltipDescription (info);

			if (!description.IsNullOrEmpty)
			{
				ToolTip.Default.SetToolTip (button, description);
			}
		}

		private FormattedText GetTooltipDescription(OptionInformation info)
		{
			//	Retourne le contenu pour un tooltip, avec les valeurs.
			var list = new List<string> ();

			foreach (var option in Documents.Verbose.VerboseDocumentOption.GetAll ().Where (x => x.Option != DocumentOption.None))
			{
				if (info.PrintingOptionDictionary.Options.Contains (option.Option))
				{
					var description = info.PrintingOptionDictionary.GetOptionDescription (option, hasBullet: false, hiliteValue: true);

					if (this.errorOptions.Contains (option.Option))
					{
						description = description.ApplyFontColor (Color.FromName ("Red"));
					}

					if (!this.RequiredDocumentOptionsContains (option.Option))
					{
						description = description.ApplyFontColor (Color.FromName ("Blue"));
					}

					list.Add (description.ToString ());
				}
			}

			return string.Join ("<br/>", list);
		}

		private FormattedText GetDeltaTooltipDescription(List<DocumentOption> usedOptions, bool exist)
		{
			var list = new List<DocumentOption> ();

			if (this.requiredDocumentOptions != null)
			{
				foreach (var option in this.requiredDocumentOptions)
				{
					if (usedOptions.Contains (option) == exist)
					{
						list.Add (option);
					}
				}
			}

			return this.GetTooltipDescription (list);
		}

		private FormattedText GetTooltipDescription(List<DocumentOption> options)
		{
			//	Retourne le contenu pour un tooltip, sans les valeurs.
			var list = new List<string> ();

			foreach (var option in Documents.Verbose.VerboseDocumentOption.GetAll ().Where (x => x.Option != DocumentOption.None))
			{
				if (options.Contains (option.Option))
				{
					var description = option.Description;
					list.Add (description.ToString ());
				}
			}

			return string.Join ("<br/>", list);
		}


		private void ButtonClicked(AbstractButton button)
		{
			var parts = button.Name.Split ('.');

			if (parts.Length == 1 && button is CheckButton)
			{
				int index = int.Parse (parts[0]);
				var entity = this.optionGroups[index].OptionInformations[0].Entity;

				if (button.ActiveState == ActiveState.Yes)
				{
					if (!this.documentCategoryEntity.DocumentOptions.Contains (entity))
					{
						this.documentCategoryEntity.DocumentOptions.Add (entity);
					}
				}
				else
				{
					if (this.documentCategoryEntity.DocumentOptions.Contains (entity))
					{
						this.documentCategoryEntity.DocumentOptions.Remove (entity);
					}
				}
			}

			if (parts.Length == 2 && button is RadioButton)
			{
				if (button.ActiveState == ActiveState.Yes)
				{
					int index = int.Parse (parts[0]);
					int group = int.Parse (parts[1]);

					for (int i = 0; i < this.optionGroups[index].OptionInformations.Count; i++)
					{
						var entity = this.optionGroups[index].OptionInformations[i].Entity;

						if (this.documentCategoryEntity.DocumentOptions.Contains (entity))
						{
							this.documentCategoryEntity.DocumentOptions.Remove (entity);
						}
					}

					if (group != -1)
					{
						var entity = this.optionGroups[index].OptionInformations[group].Entity;
						this.documentCategoryEntity.DocumentOptions.Add (entity);
					}
				}
			}

			this.UpdateErrorAndWarning ();
		}


		private void UpdateErrorAndWarning()
		{
			this.errorOptions.Clear ();

			var usedOptions     = new List<DocumentOption> ();
			var overflowOptions = new List<DocumentOption> ();

			int error    = 0;
			int missing  = 0;
			int overflow = 0;

			FormattedText errorBullet    = new FormattedText ("●").ApplyFontColor (Color.FromName ("Red")).ApplyFontSize (14);
			FormattedText overflowBullet = new FormattedText ("●").ApplyFontColor (Color.FromName ("Blue")).ApplyFontSize (14);

			foreach (var documentOptionEntity in this.documentCategoryEntity.DocumentOptions)
			{
				var info = this.optionInformations.Where (x => x.Entity == documentOptionEntity).FirstOrDefault ();

				foreach (var option in info.Options)
				{
					if (this.RequiredDocumentOptionsContains (option))
					{
						if (usedOptions.Contains (option))
						{
							if (!this.errorOptions.Contains (option))
							{
								this.errorOptions.Add (option);
								error++;
							}
						}
						else
						{
							usedOptions.Add (option);
						}
					}
					else
					{
						if (!overflowOptions.Contains (option))
						{
							overflowOptions.Add (option);
						}
					}
				}
			}

			int required = this.RequiredDocumentOptionsCount;

			if (usedOptions.Count < required)
			{
				missing = required - usedOptions.Count;
			}

			overflow = overflowOptions.Count;

			foreach (var info in this.optionInformations)
			{
				if (info.ErrorText != null)
				{
					if (error == 0)
					{
						info.ErrorVisibility.Visibility = false;
					}
					else
					{
						info.ErrorVisibility.Visibility = true;

						FormattedText text = null;

						foreach (var option in info.Options)
						{
							if (this.errorOptions.Contains (option))
							{
								text = errorBullet;
								break;
							}
						}

						info.ErrorText.FormattedText = text;
					}
				}

				if (info.OverflowText != null)
				{
					if (overflow == 0)
					{
						info.OverflowVisibility.Visibility = false;
					}
					else
					{
						info.OverflowVisibility.Visibility = true;

						FormattedText text = null;

						if (this.documentCategoryEntity.DocumentOptions.Contains (info.Entity))
						{
							foreach (var option in info.Options)
							{
								if (overflowOptions.Contains (option))
								{
									text = overflowBullet;
									break;
								}
							}
						}

						info.OverflowText.FormattedText = text;
					}
				}

				if (info.Button != null)
				{
					this.SetTooltip (info.Button, info);
				}
			}

			FormattedText errorMessage    = null;
			FormattedText errorTooltip    = null;
			FormattedText overflowMessage = null;
			FormattedText overflowTooltip = null;
			FormattedText missingMessage  = null;
			FormattedText missingTooltip  = null;

			if (error != 0)
			{
				if (error == 1)
				{
					errorMessage = "Il y a une option définie plusieurs fois";
				}
				else
				{
					errorMessage = string.Format ("Il y a {0} options définies plusieurs fois", error.ToString ());
				}

				errorMessage = FormattedText.Concat (errorBullet, " ", errorMessage.ApplyBold ());

				errorTooltip = this.GetTooltipDescription (this.errorOptions);
			}

			if (missing != 0)
			{
				if (missing == 1)
				{
					missingMessage = "Il y a une option indéfinie";
				}
				else
				{
					missingMessage = string.Format ("Il y a {0} options indéfinies", missing.ToString ());
				}

				missingTooltip = this.GetDeltaTooltipDescription (usedOptions, false);
			}

			if (overflow != 0)
			{
				if (overflow == 1)
				{
					overflowMessage = "Il y a une option définie inutilement";
				}
				else
				{
					overflowMessage = string.Format ("Il y a {0} options définies inutilement", overflow.ToString ());
				}

				overflowMessage = FormattedText.Concat (overflowBullet, " ", overflowMessage);

				overflowTooltip = this.GetTooltipDescription (overflowOptions);
			}

			if (this.documentCategoryEntity.DocumentOptions.Count == 0)
			{
				missingMessage = "Aucune option d'impression n'est choisie";
			}

			if (errorMessage.IsNullOrEmpty)
			{
				this.errorFrame.Visibility = false;
			}
			else
			{
				this.errorFrame.Visibility = true;
				this.errorText.FormattedText = errorMessage;

				ToolTip.Default.SetToolTip (this.errorFrame, errorTooltip);
			}

			if (overflowMessage.IsNullOrEmpty)
			{
				this.overflowFrame.Visibility = false;
			}
			else
			{
				this.overflowFrame.Visibility = true;
				this.overflowText.FormattedText = overflowMessage;

				ToolTip.Default.SetToolTip (this.overflowFrame, overflowTooltip);
			}

			if (missingMessage.IsNullOrEmpty)
			{
				this.missingFrame.Visibility = false;
			}
			else
			{
				this.missingFrame.Visibility = true;
				this.missingText.FormattedText = missingMessage;

				ToolTip.Default.SetToolTip (this.missingFrame, missingTooltip);
			}
		}


		private void UpdateData()
		{
			this.UpdateOptionInformations ();
			this.UpdateOptionGroup ();
		}

		private void UpdateOptionGroup()
		{
			this.optionGroups.Clear ();

			foreach (var info in this.optionInformations)
			{
				if (!info.IsRadio)
				{
					var group = new OptionGroup (info);

					foreach (var friend in this.optionInformations)
					{
						if (friend != info && !friend.IsRadio && friend.HasSameOptions (info))
						{
							info.IsRadio = true;
							friend.IsRadio = true;
							group.OptionInformations.Add (friend);
						}
					}

					this.optionGroups.Add (group);
				}
			}
		}

		private void UpdateOptionInformations()
		{
			this.requiredDocumentOptions = Epsitec.Cresus.Core.Documents.External.CresusCore.GetRequiredDocumentOptionsByDocumentType (this.documentCategoryEntity.DocumentType);
			this.optionInformations.Clear ();

			var optionEntities = this.businessContext.GetAllEntities<DocumentOptionsEntity> ();
			foreach (var optionEntity in optionEntities)
			{
				this.optionInformations.Add (this.GetOptionInformation (optionEntity));
			}
		}

		private OptionInformation GetOptionInformation(DocumentOptionsEntity optionEntity)
		{
			var printingOptionDictionary = optionEntity.GetOptions ();
			var options = printingOptionDictionary.Options.Where (x => x != DocumentOption.None);

			int count = 0;
			int total = 0;

			if (this.requiredDocumentOptions != null)
			{
				foreach (var option in options)
				{
					if (this.RequiredDocumentOptionsContains (option))
					{
						count++;
					}

					total++;
				}
			}

			return new OptionInformation (optionEntity, printingOptionDictionary, options, count, total);
		}


		private bool RequiredDocumentOptionsContains(DocumentOption option)
		{
			return this.requiredDocumentOptions != null && this.requiredDocumentOptions.Contains (option);
		}

		private int RequiredDocumentOptionsCount
		{
			get
			{
				return (this.requiredDocumentOptions == null) ? 0 : this.requiredDocumentOptions.Count ();
			}
		}


		private class OptionGroup
		{
			public OptionGroup(OptionInformation optionInformation)
			{
				this.optionInformations = new List<OptionInformation> ();
				this.optionInformations.Add (optionInformation);
			}

			public List<OptionInformation> OptionInformations
			{
				get
				{
					return this.optionInformations;
				}
			}

			public string Ratio
			{
				get
				{
					return string.Concat (this.Used.ToString (), "/", this.Total.ToString ());
				}
			}

			public int Used
			{
				get
				{
					return this.optionInformations[0].Used;
				}
			}

			public int Total
			{
				get
				{
					return this.optionInformations[0].Total;
				}
			}

			public string Name
			{
				get
				{
					return this.optionInformations[0].Entity.Name.ToString ();
				}
			}

			public bool IsRadio
			{
				get
				{
					return this.optionInformations.Count > 1;
				}
			}

			private readonly List<OptionInformation> optionInformations;
		}

		private class OptionInformation
		{
			public OptionInformation(DocumentOptionsEntity entity, PrintingOptionDictionary printingOptionDictionary, IEnumerable<DocumentOption> options, int used, int total)
			{
				this.Entity                   = entity;
				this.PrintingOptionDictionary = printingOptionDictionary;
				this.options                  = options;
				this.Used                     = used;
				this.Total                    = total;
			}

			public DocumentOptionsEntity Entity
			{
				get;
				private set;
			}

			public PrintingOptionDictionary PrintingOptionDictionary
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

			public bool IsRadio
			{
				get;
				set;
			}

			public Widget ErrorVisibility
			{
				get;
				set;
			}

			public StaticText ErrorText
			{
				get;
				set;
			}

			public Widget OverflowVisibility
			{
				get;
				set;
			}

			public StaticText OverflowText
			{
				get;
				set;
			}

			public AbstractButton Button
			{
				get;
				set;
			}

			public IEnumerable<DocumentOption> Options
			{
				get
				{
					return this.options;
				}
			}

			public bool HasSameOptions(OptionInformation that)
			{
				if (this.options.Count () != that.options.Count ())
				{
					return false;
				}

				foreach (var option in this.options)
				{
					if (!that.options.Contains (option))
					{
						return false;
					}
				}

				return true;
			}

			private readonly IEnumerable<DocumentOption> options;
		}


		private readonly IBusinessContext					businessContext;
		private readonly DocumentCategoryEntity				documentCategoryEntity;
		private readonly DocumentCategoryController			documentCategoryController;
		private readonly List<OptionInformation>			optionInformations;
		private readonly List<OptionGroup>					optionGroups;
		private readonly List<DocumentOption>				errorOptions;

		private Scrollable									checkButtonsFrame;
		private bool										firstGroup;
		private IEnumerable<DocumentOption>					requiredDocumentOptions;
		private FrameBox									errorFrame;
		private StaticText									errorText;
		private FrameBox									overflowFrame;
		private StaticText									overflowText;
		private FrameBox									missingFrame;
		private StaticText									missingText;
	}
}
