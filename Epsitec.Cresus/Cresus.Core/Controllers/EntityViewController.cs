//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers
{
	public abstract class EntityViewController : CoreViewController
	{
		public EntityViewController(string name, AbstractEntity entity, ViewControllerMode mode)
			: base (name)
		{
			this.entity = entity;
			this.mode = mode;
		}

		public AbstractEntity Entity
		{
			get
			{
				return this.entity;
			}
		}

		public ViewControllerMode Mode
		{
			get
			{
				return this.mode;
			}
		}

		public Orchestrators.DataViewOrchestrator Orchestrator
		{
			get;
			set;
		}

		public Widget Container
		{
			get
			{
				return this.container;
			}
		}


		public static EntityViewController CreateViewController(string name, AbstractEntity entity, ViewControllerMode mode, Orchestrators.DataViewOrchestrator orchestrator)
		{
			EntityViewController controller = EntityViewController.ResolveViewController (name, entity, mode);

			if (controller == null)
			{
				return null;
			}

			controller.Orchestrator = orchestrator;

			return controller;
		}
		
		private static EntityViewController ResolveViewController(string name, AbstractEntity entity, ViewControllerMode mode)
		{
			if (mode == ViewControllerMode.None)
			{
				return null;
			}

			if (entity is Entities.NaturalPersonEntity)
			{
				return new NaturalPersonViewController (name, entity, mode);
			}

			if (entity is Entities.LegalPersonEntity)
			{
				return new LegalPersonViewController (name, entity, mode);
			}

			//	Doit être avant les tests sur MailContactEntity, TelecomContactEntity et UriContactEntity !
			if (entity is Entities.AbstractContactEntity && mode == ViewControllerMode.RolesEdition)
			{
				return new RolesContactViewController (name, entity, mode);
			}

			if (entity is Entities.TelecomContactEntity && mode == ViewControllerMode.TelecomTypeEdition)
			{
				return new TelecomTypeViewController (name, entity, mode);
			}

			if (entity is Entities.UriContactEntity && mode == ViewControllerMode.UriSchemeEdition)
			{
				return new UriSchemeViewController (name, entity, mode);
			}

			//	Après...
			if (entity is Entities.MailContactEntity)
			{
				return new MailContactViewController (name, entity, mode);
			}

			if (entity is Entities.TelecomContactEntity)
			{
				return new TelecomContactViewController (name, entity, mode);
			}

			if (entity is Entities.UriContactEntity)
			{
				return new UriContactViewController (name, entity, mode);
			}

			// TODO: Compléter ici au fur et à mesure des besoins...

			return null;
		}


		#region Tiles creation
		protected static Widgets.GroupingTile CreateGroupingTile(Widget parent, string iconUri, string title, bool isEditing)
		{
			var group = new Widgets.GroupingTile
			{
				Parent = parent,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
				TopLeftIconUri = iconUri,
				Title = title,
				IsEditing = isEditing,
			};

			return group;
		}


		protected Widgets.SummaryTile CreateSummaryTile(Widgets.GroupingTile parent, EntitiesAccessors.AbstractAccessor accessor, bool enableCreateAndRemoveButton, ViewControllerMode childrenMode)
		{
			var tile = new Widgets.SummaryTile
			{
				Parent = parent.Container,
				Dock = DockStyle.Top,
				ArrowLocation = Direction.Right,
				ArrowEnabled = true,
				EnteredSensitivity = childrenMode != ViewControllerMode.None,
				EntitiesAccessor = accessor,
				Entity = accessor.Entity,
				Mode = this.Mode,
				ChildrenMode = childrenMode,
				EnableCreateAndRemoveButton = enableCreateAndRemoveButton,
				IsEditing = false,
				Summary = accessor.Summary,
			};

			tile.PreferredHeight = tile.ContentHeight;
			tile.Clicked += this.HandleTileClicked;
			tile.CreateEntity += this.HandleTileCreateEntity;
			tile.RemoveEntity += this.HandleTileRemoveEntity;

			return tile;
		}

		protected Widgets.EditionTile CreateEditionTile(Widgets.GroupingTile parent, EntitiesAccessors.AbstractAccessor accessor, ViewControllerMode childrenMode)
		{
			System.Diagnostics.Debug.Assert (this.container != null);

			var tile = new Widgets.EditionTile
			{
				Parent = parent.Container,
				Dock = DockStyle.Top,
				ArrowLocation = Direction.Right,
				ArrowEnabled = false,
				EnteredSensitivity = childrenMode != ViewControllerMode.None,
				EntitiesAccessor = accessor,
				Entity = accessor.Entity,
				Mode = this.Mode,
				ChildrenMode = childrenMode,
				IsEditing = true,
			};

			tile.Clicked += this.HandleTileClicked;

			return tile;
		}

		protected void CreateHeaderEditorTile()
		{
			System.Diagnostics.Debug.Assert (this.container != null);

#if true
			var tile = new FrameBox
			{
				Parent = this.container,
				Dock = DockStyle.Top,
			};

#if false
			var title = new StaticText
			{
				Parent = tile,
				Dock = DockStyle.Fill,
				ContentAlignment = ContentAlignment.MiddleCenter,
				Text = "Sous-titre",
			};
#endif

			var closeButton = new GlyphButton
			{
				Parent = tile,
				ButtonStyle = Common.Widgets.ButtonStyle.Normal,
				GlyphShape = GlyphShape.Close,
				Dock = DockStyle.Right,
				//?GlyphShape = GlyphShape.ArrowLeft,
				//?Dock = DockStyle.Left,
				PreferredSize = new Size (18, 18),
				Margins = new Margins (0, Widgets.ContainerTile.ArrowBreadth+2, 2, 2-1),
				//?Margins = new Margins (2, Widgets.TileContainer.ArrowBreadth+2, 2, 2-1),
			};

			closeButton.Clicked += this.HandleCloseButtonClicked;
#endif
		}

		protected void CreateFooterEditorTile()
		{
			System.Diagnostics.Debug.Assert (this.container != null);

#if false
			var tile = new Widgets.EditionTile
			{
				Parent = this.container,
				Dock = DockStyle.Top,
				ArrowLocation = Direction.Right,
				ArrowEnabled = false,
				IsEditing = true,
			};

			var closeButton = new Button
			{
				Parent = tile,
				Text = "Fermer",
				PreferredWidth = 75,
				Dock = DockStyle.Right,
				Margins = new Margins(0, Widgets.TileContainer.ArrowBreadth+10, 10, 10),
			};

			closeButton.Clicked += new EventHandler<MessageEventArgs> (this.HandleCloseButtonClicked);
#endif
		}
		#endregion



		#region Tiles content creation
		protected void CreateLinkButtons(Widget embedder)
		{
#if false
			//	TODO: Prototype non fonctionnel, à valider puis terminer
			var frameBox = new ClippingFrameBox
			{
				Parent = embedder,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 10, 20),
				TabIndex = ++this.tabIndex,
				ClippingMode = ClippingMode.ClipWidth,
			};

			var linkButton = new Button
			{
				Parent = frameBox,
				Text = "Lier avec...",
				PreferredWidth = 70,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 2, 0, 0),
				TabIndex = ++this.tabIndex,
			};

			var unlinkButton = new Button
			{
				Parent = frameBox,
				Text = "Délier",
				PreferredWidth = 70,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 10, 0, 0),
				TabIndex = ++this.tabIndex,
				Enable = false,
			};

			var label = new StaticText
			{
				Parent = frameBox,
				Text = "Utilisation unique",  // ou "Utilisé 10x"
				Dock = DockStyle.Fill,
			};
#endif
		}

		protected void CreateTextField(Widget embedder, int width, string initialValue, System.Action<string> callback, System.Func<string, bool> validator)
		{
			var textField = new TextField
			{
				Parent = embedder,
				Text = initialValue,
				Dock = (width == 0) ? DockStyle.Fill : DockStyle.Left,
				PreferredWidth = width,
				Margins = new Margins (0, (width == 0) ? 10:2, 0, 0),
				TabIndex = ++this.tabIndex,
			};

			this.CreateTextFieldHandler (textField, callback, validator);
		}

		protected void CreateTextField(Widget embedder, int width, string label, string initialValue, System.Action<string> callback, System.Func<string, bool> validator)
		{
			var staticText = new StaticText
			{
				Parent = embedder,
				Text = string.Concat (label, " :"),
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			TextField textField;

			if (width == 0)  // occupe toute la largeur ?
			{
				textField = new TextField
				{
					Parent = embedder,
					Text = initialValue,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 10, 0, 5),
					TabIndex = ++this.tabIndex,
				};
			}
			else  // largeur partielle fixe ?
			{
				var box = new FrameBox
				{
					Parent = embedder,
					Dock = DockStyle.Top,
					TabIndex = ++this.tabIndex,
				};

				textField = new TextField
				{
					Parent = box,
					Text = initialValue,
					Dock = DockStyle.Left,
					PreferredWidth = width,
					Margins = new Margins (0, 10, 0, 5),
					TabIndex = ++this.tabIndex,
				};
			}

			this.CreateTextFieldHandler (textField, callback, validator);
		}

		protected void CreateTextFieldMulti(Widget embedder, int height, string label, string initialValue, System.Action<string> callback, System.Func<string, bool> validator)
		{
			var staticText = new StaticText
			{
				Parent = embedder,
				Text = string.Concat (label, " :"),
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			var textField = new TextFieldMulti
			{
				Parent = embedder,
				Text = initialValue,
				PreferredHeight = height,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 5),
				TabIndex = ++this.tabIndex,
			};

			this.CreateTextFieldHandler (textField, callback, validator);
		}

		private void CreateTextFieldHandler(AbstractTextField textField, System.Action<string> callback, System.Func<string, bool> validator)
		{
			textField.TextChanged +=
				delegate (object sender)
				{
					if (validator == null || validator (textField.Text))
					{
						callback (textField.Text);
						textField.SetError (false);
					}
					else
					{
						textField.SetError (true);
					}
				};
		}


		protected void CreateHintEditor(Widget embedder, int width1, string label, string initialValue1, string initialValue2, System.Action<string> callback1, System.Action<string> callback2, EntitiesAccessors.BidirectionnalConverter converter)
		{
			var staticText = new StaticText
			{
				Parent = embedder,
				Text = string.Concat (label, " :"),
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			var hint = new Widgets.HintEditor
			{
				Parent = embedder,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 5),
				TabIndex = ++this.tabIndex,
			};

			converter.InitializeHintEditor (hint);
			hint.Text = converter.GetFormatedText (initialValue1, initialValue2);  // après InitializeHintEditor !

			hint.EditionAccepted +=
				delegate (object sender)
				{
					int sel = hint.SelectedIndex;
					if (sel != -1)
					{
						string text1, text2;
						converter.Get (sel, out text1, out text2);
						callback1 (text1);
						callback2 (text2);
					}
				};
		}


		protected void CreateCombo(Widget embedder, int width, string label, EntitiesAccessors.ComboInitializer initializer, bool readOnly, bool allowMultipleSelection, bool detailed, string initialValue, System.Action<string> callback, System.Func<string, bool> validator)
		{
			var staticText = new StaticText
			{
				Parent = embedder,
				Text = string.Concat (label, " :"),
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			if (detailed)
			{
				var combo = new Widgets.DetailedCombo
				{
					Parent = embedder,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 5, 10),
					AllowMultipleSelection = allowMultipleSelection,
					ComboInitializer = initializer,
					Text = initializer.ConvertInternalToEdition (initialValue),
					TabIndex = ++this.tabIndex,
				};

				this.CreateComboHandler (combo, callback, validator, initializer.ConvertEditionToInternal);
			}
			else
			{
				Widgets.SuperCombo combo;

				if (width == 0)  // occupe toute la largeur ?
				{
					combo = new Widgets.SuperCombo
					{
						Parent = embedder,
						IsReadOnly = readOnly,
						AllowMultipleSelection = allowMultipleSelection,
						//?Text = initializer.ConvertInternalToEdition (initialValue),
						Dock = DockStyle.Top,
						Margins = new Margins (0, 10, 0, 5),
						TabIndex = ++this.tabIndex,
					};
				}
				else  // largeur partielle fixe ?
				{
					var box = new FrameBox
					{
						Parent = embedder,
						Dock = DockStyle.Top,
						TabIndex = ++this.tabIndex,
					};

					combo = new Widgets.SuperCombo
					{
						Parent = box,
						IsReadOnly = readOnly,
						AllowMultipleSelection = allowMultipleSelection,
						//?Text = initializer.ConvertInternalToEdition (initialValue),
						Dock = DockStyle.Left,
						PreferredWidth = width,
						Margins = new Margins (0, 10, 0, 5),
						TabIndex = ++this.tabIndex,
					};
				}

				initializer.InitializeCombo (combo);
				combo.AddSelection (Enumerable.Range (0, 1));

				this.CreateComboHandler (combo, callback, validator, initializer.ConvertEditionToInternal);
			}
		}

		private void CreateComboHandler(Widget widget, System.Action<string> callback, System.Func<string, bool> validator, System.Func<string, string> converter)
		{
			widget.TextChanged +=
				delegate (object sender)
				{
					string text = converter (widget.Text);

					if (validator == null || validator (text))
					{
						callback (text);
						widget.SetError (false);
					}
					else
					{
						widget.SetError (true);
					}
				};
		}


		protected void CreateMargin(Widget embedder, bool horizontalSeparator)
		{
			if (horizontalSeparator)
			{
				var separator = new Separator
				{
					Parent = embedder,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 10, 10, 10),
					PreferredHeight = 2,
				};
			}
			else
			{
				var frame = new FrameBox
				{
					Parent = embedder,
					Dock = DockStyle.Top,
					PreferredHeight = 10,
				};
			}
		}
		#endregion


		private void CreateEntity(Widgets.AbstractTile tile)
		{
			EntitiesAccessors.AbstractAccessor accessor = tile.EntitiesAccessor;
			AbstractEntity newEntity = accessor.Create ();
			CoreViewController controller = EntityViewController.CreateViewController ("ViewController", newEntity, tile.ChildrenMode, this.Orchestrator);

			this.Orchestrator.RebuildView ();
			this.Orchestrator.ShowSubView (this, controller);
		}

		private void RemoveEntity(Widgets.AbstractTile tile)
		{
			EntitiesAccessors.AbstractAccessor accessor = tile.EntitiesAccessor;

			Common.Dialogs.DialogResult result = Common.Dialogs.MessageDialog.ShowQuestion (accessor.RemoveQuestion, tile.Window);

			if (result == Common.Dialogs.DialogResult.Yes)
			{
				accessor.Remove ();

				this.Orchestrator.RebuildView ();
			}
		}

		private void CloseTile()
		{
			this.Orchestrator.CloseView (this);
		}


		private void HandleTileClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsqu'une tuile quelconque est cliquée.
			var tile = sender as Widgets.AbstractTile;

			if (tile.IsSelected)
			{
				//	If the tile was selected, deselect it by closing its sub-view:
				tile.CloseSubView (this.Orchestrator);
			}
			else
			{
				tile.OpenSubView (this.Orchestrator, this);
			}
		}

		private void HandleTileCreateEntity(object sender)
		{
			//	Appelé lorsque le bouton "+" d'une tuile est cliqué.
			var tile = sender as Widgets.AbstractTile;
			this.CreateEntity (tile);
		}

		private void HandleTileRemoveEntity(object sender)
		{
			//	Appelé lorsque le bouton "-" d'une tuile est cliqué.
			var tile = sender as Widgets.AbstractTile;
			this.RemoveEntity (tile);
		}

		private void HandleCloseButtonClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton "Fermer" d'une tuile est cliqué.
			this.CloseTile ();
		}


		private readonly AbstractEntity entity;
		private readonly ViewControllerMode mode;
		protected Widget container;
		private int tabIndex;
	}
}
