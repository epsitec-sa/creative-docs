//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Documents.Verbose;
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

			this.verboseDocumentOptions = VerboseDocumentOption.GetAll ().Where (x => x.Option != DocumentOption.None);
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
			this.CreateUseless (box);
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

		private void CreateUseless(Widget parent)
		{
			this.uselessFrame = new FrameBox
			{
				Parent = parent,
				DrawFullFrame = true,
				BackColor = Color.FromBrightness (1),
				PreferredHeight = this.documentCategoryController.errorHeight,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, -1, 0),
			};

			this.uselessText = new StaticText
			{
				Parent = this.uselessFrame,
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
				Margins = new Margins (5, 0, 0, 0),
			};
		}

		private void CreateCheckButtons()
		{
			this.UpdateData ();

			var parent = this.checkButtonsFrame.Viewport;
			parent.Children.Clear ();

			this.firstGroup = true;
			this.CreateGroup (parent, this.optionGroups.Where (x => x.Used != 0 && x.Used == x.Total), "Options parfaitement adaptées",  this.documentCategoryController.acceptedColor);
			this.CreateGroup (parent, this.optionGroups.Where (x => x.Used != 0 && x.Used <  x.Total), "Options partiellement adaptées", this.documentCategoryController.toleratedColor);
			this.CreateGroup (parent, this.optionGroups.Where (x => x.Used == 0                     ), "Options inadaptées",             this.documentCategoryController.rejectedColor);
		}

		private void CreateGroup(Widget parent, IEnumerable<OptionGroup> optionGroups, FormattedText title, Color color)
		{
			if (optionGroups.Any ())
			{
				var frame = this.CreateColorizedFrameBox (parent, color);
				this.CreateTitle (frame, title);

				this.lastButton = '-';

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
			if (this.lastButton == 'r' || (group.IsRadio && this.lastButton != '-'))
			{
				//	Met une petite séparation.
				new FrameBox
				{
					Parent = parent,
					PreferredHeight = 5,
					Dock = DockStyle.Top,
				};
			}

			this.lastButton = group.IsRadio ? 'r' : 'c';

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
					PreferredWidth = DocumentOptionsController.errorBulletWidth,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, this.documentCategoryController.lineHeight, 0),
				};

				var uselessFrame = new FrameBox
				{
					Parent = frame,
					PreferredWidth = DocumentOptionsController.errorBulletWidth,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, this.documentCategoryController.lineHeight, 0),
				};

				var leftFrame = new FrameBox
				{
					Parent = frame,
					Dock = DockStyle.Fill,
				};

				var rightFrame = new FrameBox
				{
					Parent = frame,
					PreferredWidth = DocumentOptionsController.ratioWidth,
					Dock = DockStyle.Right,
				};

				var ratio = new StaticText
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
						PreferredWidth = DocumentOptionsController.errorBulletWidth,
						PreferredHeight = this.documentCategoryController.lineHeight,
						Dock = DockStyle.Top,
					};

					group.OptionInformations[i].ErrorVisibility = errorFrame;
					group.OptionInformations[i].ErrorText = error;

					var useless = new StaticText
					{
						Parent = uselessFrame,
						ContentAlignment = ContentAlignment.MiddleLeft,
						PreferredWidth = DocumentOptionsController.errorBulletWidth,
						PreferredHeight = this.documentCategoryController.lineHeight,
						Dock = DockStyle.Top,
					};

					group.OptionInformations[i].UselessVisibility = uselessFrame;
					group.OptionInformations[i].UselessText = useless;

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
					PreferredWidth = DocumentOptionsController.errorBulletWidth,
					PreferredHeight = this.documentCategoryController.lineHeight,
					Dock = DockStyle.Left,
				};

				group.OptionInformations[0].ErrorVisibility = error;
				group.OptionInformations[0].ErrorText = error;

				var useless = new StaticText
				{
					Parent = frame,
					ContentAlignment = ContentAlignment.MiddleLeft,
					PreferredWidth = DocumentOptionsController.errorBulletWidth,
					PreferredHeight = this.documentCategoryController.lineHeight,
					Dock = DockStyle.Left,
				};

				group.OptionInformations[0].UselessVisibility = useless;
				group.OptionInformations[0].UselessText = useless;

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

				var ratio = new StaticText
				{
					Parent = frame,
					Text = group.Ratio,
					ContentAlignment = ContentAlignment.MiddleRight,
					PreferredWidth = DocumentOptionsController.ratioWidth,
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
			bool active = this.documentCategoryEntity.DocumentOptions.Contains (info.Entity);  // option cochée ?

			if (active)
			{
				//	Cherche s'il faut utiliser les puces.
				bool hasBullet = false;

				foreach (var verboseOption in this.verboseDocumentOptions)
				{
					if (info.PrintingOptionDictionary.Options.Contains (verboseOption.Option))
					{
						if (this.errorOptions.Contains (verboseOption.Option) ||
                            !this.RequiredDocumentOptionsContains (verboseOption.Option))
						{
							hasBullet = true;
						}
					}
				}

				//	Construit les lignes, une par option.
				var list = new List<string> ();

				int correctCount = 0;
				int errorCount   = 0;
				int uselessCount = 0;

				foreach (var verboseOption in this.verboseDocumentOptions)
				{
					if (info.PrintingOptionDictionary.Options.Contains (verboseOption.Option))
					{
						var description = info.PrintingOptionDictionary.GetOptionDescription (verboseOption, hasBullet: false, hiliteValue: true);

						if (this.errorOptions.Contains (verboseOption.Option))
						{
							description = description.ApplyFontColor (DocumentOptionsController.errorColor);

							if (hasBullet)
							{
								description = FormattedText.Concat (DocumentOptionsController.errorBullet, "  ", description);
							}

							errorCount++;
						}
						else if (!this.RequiredDocumentOptionsContains (verboseOption.Option))
						{
							description = description.ApplyFontColor (DocumentOptionsController.uselessColor);

							if (hasBullet)
							{
								description = FormattedText.Concat (DocumentOptionsController.uselessBullet, "  ", description);
							}

							uselessCount++;
						}
						else
						{
							if (hasBullet)
							{
								description = FormattedText.Concat (DocumentOptionsController.normalBullet, "  ", description);
							}

							correctCount++;
						}

						list.Add (description.ToString ());
					}
				}

				//	Génère les textes.
				FormattedText correctText = "";
				FormattedText errorText   = "";
				FormattedText uselessText = "";

				if (correctCount != 0)
				{
					if (correctCount == 1)
					{
						correctText = "Une option est définie correctement<br/>";
					}
					else
					{
						correctText = string.Format ("{0} options sont définies correctement<br/>", correctCount.ToString ());
					}

					if (hasBullet)
					{
						correctText = FormattedText.Concat (DocumentOptionsController.normalBullet, "  ", correctText);
					}
				}

				if (errorCount != 0)
				{
					if (errorCount == 1)
					{
						errorText = "Une option est définie plusieurs fois<br/>";
					}
					else
					{
						errorText = string.Format ("{0} options sont définies plusieurs fois<br/>", errorCount.ToString ());
					}

					if (hasBullet)
					{
						errorText = FormattedText.Concat (DocumentOptionsController.errorBullet, "  ", errorText);
					}

					errorText = errorText.ApplyFontColor (DocumentOptionsController.errorColor);
				}

				if (uselessCount != 0)
				{
					if (uselessCount == 1)
					{
						uselessText = "Une option est définie inutilement<br/>";
					}
					else
					{
						uselessText = string.Format ("{0} options sont définies inutilement<br/>", uselessCount.ToString ());
					}

					if (hasBullet)
					{
						uselessText = FormattedText.Concat (DocumentOptionsController.uselessBullet, "  ", uselessText);
					}

					uselessText = uselessText.ApplyFontColor (DocumentOptionsController.uselessColor);
				}

				//	Génère le texte final.
				var hline = new string ('_', 80);
				var separator = string.Concat ("<font size=\"40%\">", hline, "<br/> <br/></font>");

				return string.Concat (correctText, errorText, uselessText, separator, string.Join ("<br/>", list));
			}
			else  // option non cochée ?
			{
				if (info.Used == 0)
				{
					return "Ce bouton n'est pas adapté";
				}
				else if (info.Used == 1)
				{
					return "Une option sera ajoutée aux définitions";
				}
				else
				{
					return string.Format ("{0} options seront ajoutées aux définitions", info.Used.ToString ());
				}
			}
		}

		private FormattedText GetMissingTooltipDescription(List<DocumentOption> usedOptions)
		{
			var list = new List<DocumentOption> ();

			if (this.requiredDocumentOptions != null)
			{
				foreach (var option in this.requiredDocumentOptions)
				{
					if (!usedOptions.Contains (option))
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

			foreach (var verboseOption in this.verboseDocumentOptions)
			{
				if (options.Contains (verboseOption.Option))
				{
					var description = verboseOption.Description;
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

			var usedOptions    = new List<DocumentOption> ();
			var uselessOptions = new List<DocumentOption> ();

			int error   = 0;
			int missing = 0;
			int useless = 0;

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
						if (!uselessOptions.Contains (option))
						{
							uselessOptions.Add (option);
						}
					}
				}
			}

			int required = this.RequiredDocumentOptionsCount;

			if (usedOptions.Count < required)
			{
				missing = required - usedOptions.Count;
			}

			useless = uselessOptions.Count;

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
								text = DocumentOptionsController.errorBullet;
								break;
							}
						}

						info.ErrorText.FormattedText = text;
					}
				}

				if (info.UselessText != null)
				{
					if (useless == 0)
					{
						info.UselessVisibility.Visibility = false;
					}
					else
					{
						info.UselessVisibility.Visibility = true;

						FormattedText text = null;

						if (this.documentCategoryEntity.DocumentOptions.Contains (info.Entity))
						{
							foreach (var option in info.Options)
							{
								if (uselessOptions.Contains (option))
								{
									text = DocumentOptionsController.uselessBullet;
									break;
								}
							}
						}

						info.UselessText.FormattedText = text;
					}
				}

				if (info.Button != null)
				{
					this.SetTooltip (info.Button, info);
				}
			}

			FormattedText errorMessage   = null;
			FormattedText errorTooltip   = null;
			FormattedText uselessMessage = null;
			FormattedText uselessTooltip = null;
			FormattedText missingMessage = null;
			FormattedText missingTooltip = null;

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

				errorMessage = FormattedText.Concat (DocumentOptionsController.errorBullet, "  ", errorMessage.ApplyBold ());

				errorTooltip = this.GetTooltipDescription (this.errorOptions).ApplyFontColor (DocumentOptionsController.errorColor);
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

				missingMessage = FormattedText.Concat (DocumentOptionsController.missingBullet, "  ", missingMessage);

				missingTooltip = this.GetMissingTooltipDescription (usedOptions);
			}

			if (useless != 0)
			{
				if (useless == 1)
				{
					uselessMessage = "Il y a une option définie inutilement";
				}
				else
				{
					uselessMessage = string.Format ("Il y a {0} options définies inutilement", useless.ToString ());
				}

				uselessMessage = FormattedText.Concat (DocumentOptionsController.uselessBullet, "  ", uselessMessage);

				uselessTooltip = this.GetTooltipDescription (uselessOptions).ApplyFontColor (DocumentOptionsController.uselessColor);
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

			if (uselessMessage.IsNullOrEmpty)
			{
				this.uselessFrame.Visibility = false;
			}
			else
			{
				this.uselessFrame.Visibility = true;
				this.uselessText.FormattedText = uselessMessage;

				ToolTip.Default.SetToolTip (this.uselessFrame, uselessTooltip);
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

			var optionEntities = this.businessContext.GetAllEntities<DocumentOptionsEntity> ().OrderBy (x => x.Name);
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

			public DocumentOptionsEntity Entity
			{
				get
				{
					return this.optionInformations[0].Entity;
				}
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

			public string RatioDescription
			{
				get
				{
					if (this.Used == this.Total && this.Used != 0)
					{
						if (this.Used == 1)
						{
							return "L'option est utile pour ce type de document";
						}
						else
						{
							return string.Format ("Les {0} options sont toutes utiles pour ce type de document", this.Used.ToString ());
						}
					}
					else
					{
						int uselessCount = this.Total - this.Used;
						string uselessText = "";

						if (uselessCount <= 1)
						{
							uselessText = "Une option est définie inutilement (rouge)";
						}
						else
						{
							uselessText = string.Format ("{0} options sont définies inutilement (rouge)", uselessCount.ToString ());
						}

						if (this.Used == 0)
						{
							return string.Format ("Aucune option utile pour ce type de document, sur un total de {0}", this.Total.ToString ());
						}
						else if (this.Used == 1)
						{
							return string.Format ("Une option utile pour ce type de document, sur un total de {0} (noir ou bleu)<br/>{1}", this.Total.ToString (), uselessText);
						}
						else
						{
							return string.Format ("{0} options utiles pour ce type de document, sur un total de {1} (noir et bleu)<br/>{2}", this.Used.ToString (), this.Total.ToString (), uselessText);
						}
					}
				}
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

			public Widget UselessVisibility
			{
				get;
				set;
			}

			public StaticText UselessText
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


		private static readonly int		errorBulletWidth = 15;
		private static readonly int		ratioWidth       = 40;

		private static readonly Color	errorColor   = Color.FromRgb (230.0/255.0,   0.0/255.0,   0.0/255.0);  // rouge
		private static readonly Color	uselessColor = Color.FromRgb (153.0/255.0, 153.0/255.0, 153.0/255.0);  // gris

		private static FormattedText	normalBullet  = Misc.GetResourceIconImageTag ("DocumentOptions.Normal",  -2, new Size (13, 13));
		private static FormattedText	errorBullet   = Misc.GetResourceIconImageTag ("DocumentOptions.Error",   -2, new Size (13, 13));
		private static FormattedText	uselessBullet = Misc.GetResourceIconImageTag ("DocumentOptions.Useless", -2, new Size (13, 13));
		private static FormattedText	missingBullet = Misc.GetResourceIconImageTag ("DocumentOptions.Missing", -2, new Size (13, 13));

		private readonly IBusinessContext					businessContext;
		private readonly DocumentCategoryEntity				documentCategoryEntity;
		private readonly DocumentCategoryController			documentCategoryController;
		private readonly IEnumerable<VerboseDocumentOption>	verboseDocumentOptions;
		private readonly List<OptionInformation>			optionInformations;
		private readonly List<OptionGroup>					optionGroups;
		private readonly List<DocumentOption>				errorOptions;

		private Scrollable									checkButtonsFrame;
		private bool										firstGroup;
		private char										lastButton;
		private IEnumerable<DocumentOption>					requiredDocumentOptions;
		private FrameBox									errorFrame;
		private StaticText									errorText;
		private FrameBox									uselessFrame;
		private StaticText									uselessText;
		private FrameBox									missingFrame;
		private StaticText									missingText;
	}
}
