//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.UI;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

[assembly: Controller (typeof (Epsitec.Common.UI.Controllers.EnumController))]

namespace Epsitec.Common.UI.Controllers
{
	public class EnumController : AbstractController
	{
		public EnumController(string parameter)
		{
			switch (parameter)
			{
				case "Combo":
					this.helper = new ComboHelper (this);
					break;

				case "Icons":
					this.helper = new IconsHelper (this);
					break;
				
				case "Radio":
					break;
				
				default:
					break;
			}

			if (this.helper == null)
			{
				this.helper = new ComboHelper (this);
			}
		}

		public override object GetActualValue()
		{
			return this.helper.GetSelectedName ();
		}

		protected override Widgets.Layouts.IGridPermeable GetGridPermeableLayoutHelper()
		{
			return this.helper;
		}

		protected override void CreateUserInterface(INamedType namedType, Caption caption)
		{
			this.Placeholder.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			IEnumType enumType = namedType as IEnumType;

			this.helper.CreateUserInterface (enumType, caption);
		}

		protected override void PrepareUserInterfaceDisposal()
		{
			base.PrepareUserInterfaceDisposal ();

			this.helper.Dispose ();
			this.helper = null;
		}

		protected override void RefreshUserInterface(object oldValue, object newValue)
		{
			if ((newValue != UndefinedValue.Value) &&
				(newValue != InvalidValue.Value) &&
				(newValue != null))
			{
				this.helper.SetSelectedName (this.ConvertFromValue (newValue));
			}
		}

		private void HandleComboTextChanged(object sender)
		{
			this.OnActualValueChanged ();
		}

		private string ConvertFromValue(object newValue)
		{
			return newValue.ToString ();
		}
		
		private enum Mode
		{
			None, Combo, Icons, Radio
		}

		private abstract class Helper : System.IDisposable, Widgets.Layouts.IGridPermeable
		{
			public Helper(EnumController host)
			{
				this.host = host;
			}
			
			public abstract void CreateUserInterface(IEnumType enumType, Caption caption);
			public abstract string GetSelectedName();
			
			protected EnumController host;

			#region IDisposable Members

			public abstract void Dispose();

			#endregion

			public abstract void SetSelectedName(string name);

			#region IGridPermeable Members

			public abstract IEnumerable<Widgets.Layouts.PermeableCell> GetChildren(int column, int row, int columnSpan, int rowSpan);
			public abstract bool UpdateGridSpan(ref int columnSpan, ref int rowSpan);

			#endregion
		}

		private class ComboHelper : Helper
		{
			public ComboHelper(EnumController host)
				: base (host)
			{
			}

			public override string GetSelectedName()
			{
				return this.combo.SelectedName;
			}

			public override void SetSelectedName(string name)
			{
				this.combo.SelectedName = name;
			}
			
			public override void Dispose()
			{
				this.combo.TextChanged -= this.host.HandleComboTextChanged;				
			}

			public override void CreateUserInterface(IEnumType enumType, Caption caption)
			{
				this.label = new StaticText ();
				this.combo = new TextFieldCombo ();

				this.host.AddWidget (this.label, WidgetType.Label);
				this.host.AddWidget (this.combo, WidgetType.Input);

				this.label.HorizontalAlignment = HorizontalAlignment.Right;
				this.label.VerticalAlignment = VerticalAlignment.BaseLine;
				this.label.ContentAlignment = Drawing.ContentAlignment.MiddleRight;
				this.label.Dock = DockStyle.Stacked;

				if (caption.HasLabels)
				{
					if (caption.Id.IsValid)
					{
						this.label.CaptionId = caption.Id;
					}
					else
					{
						this.label.Text = Collection.Extract<string> (caption.Labels, 0);
					}
					this.label.PreferredWidth = this.label.GetBestFitSize ().Width;
					this.label.Margins = new Drawing.Margins (4, 4, 0, 0);
				}

				this.combo.HorizontalAlignment = HorizontalAlignment.Stretch;
				this.combo.VerticalAlignment = VerticalAlignment.BaseLine;
				this.combo.TextChanged += this.host.HandleComboTextChanged;
				this.combo.PreferredWidth = 40;

				this.combo.TabIndex = 1;
				this.combo.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.combo.Dock = DockStyle.Stacked;

				if (enumType != null)
				{
					Support.ResourceManager manager = Widgets.Helpers.VisualTree.GetResourceManager (this.host.Placeholder);
					
					foreach (IEnumValue enumValue in enumType.Values)
					{
						if (!enumValue.IsHidden)
						{
							Caption evCaption = Support.CaptionCache.Instance.GetCaption (manager, enumValue.CaptionId);
							string evCaptionText = null;

							if ((evCaption != null) &&
								(evCaption.Labels.Count > 0))
							{
								evCaptionText = Collection.Extract (evCaption.Labels, 0);
							}

							this.combo.Items.Add (enumValue.Name, evCaptionText ?? enumValue.Name);
						}
					}

					this.combo.IsReadOnly = enumType.IsCustomizable ? false : true;
				}
			}

			public override IEnumerable<Widgets.Layouts.PermeableCell> GetChildren(int column, int row, int columnSpan, int rowSpan)
			{
				yield return new Widgets.Layouts.PermeableCell (this.label, column+0, row+0, 1, 1);
				yield return new Widgets.Layouts.PermeableCell (this.combo, column+1, row+0, columnSpan-1, 1);
			}

			public override bool UpdateGridSpan(ref int columnSpan, ref int rowSpan)
			{
				columnSpan = System.Math.Max (columnSpan, 2);
				rowSpan    = System.Math.Max (rowSpan, 1);

				return true;
			}

			private TextFieldCombo combo;
			private StaticText label;
		}

		private class IconsHelper : Helper
		{
			public IconsHelper(EnumController host)
				: base (host)
			{
			}

			public override string GetSelectedName()
			{
				if (this.enumType.IsDefinedAsFlags)
				{
					int flags = this.combo.SelectedValue;
					IEnumerable<IEnumValue> values = EnumType.ConvertEnumValuesFromFlags (this.enumType, flags);
					return EnumType.ConvertToString (values);
				}
				else
				{
					int rank = this.combo.SelectedValue;
					IEnumValue value = this.enumType[rank];

					if (value != null)
					{
						return value.Name;
					}
					else
					{
						return null;
					}
				}
			}

			public override void SetSelectedName(string name)
			{
				if (this.enumType.IsDefinedAsFlags)
				{
					IEnumerable<IEnumValue> values = EnumType.ConvertFromString (this.enumType, name);
					int flags = EnumType.ConvertEnumValuesToFlags (values);
					this.combo.SelectedValue = flags;
				}
				else
				{
					IEnumValue value = this.enumType[name];
					if (value != null)
					{
						this.combo.SelectedValue = value.Rank;
					}
					else
					{
						this.combo.SelectedValue = -1;
					}
				}
			}

			public override void Dispose()
			{
				this.combo.SelectionChanged -= this.host.HandleComboTextChanged;
			}

			public override void CreateUserInterface(IEnumType enumType, Caption caption)
			{
				this.enumType = enumType;
				
				this.label = new StaticText ();
				this.combo = enumType.IsDefinedAsFlags ? new CheckIconGrid () : new RadioIconGrid ();

				this.host.AddWidget (this.label, WidgetType.Label);
				this.host.AddWidget (this.combo, WidgetType.Input);

				this.label.HorizontalAlignment = HorizontalAlignment.Right;
				this.label.VerticalAlignment = VerticalAlignment.Center;
				this.label.ContentAlignment = Drawing.ContentAlignment.MiddleRight;
				this.label.Dock = DockStyle.Stacked;

				if (caption.HasLabels)
				{
					if (caption.Id.IsValid)
					{
						this.label.CaptionId = caption.Id;
					}
					else
					{
						this.label.Text = Collection.Extract<string> (caption.Labels, 0);
					}
					this.label.PreferredWidth = this.label.GetBestFitSize ().Width;
					this.label.Margins = new Drawing.Margins (4, 4, 0, 0);
				}

				this.combo.HorizontalAlignment = HorizontalAlignment.Stretch;
				this.combo.VerticalAlignment = VerticalAlignment.Center;
				this.combo.SelectionChanged += this.host.HandleComboTextChanged;
				this.combo.PreferredWidth = 40;
				this.combo.PreferredHeight = 22;

				this.combo.TabIndex = 1;
				this.combo.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.combo.Dock = DockStyle.Stacked;

				if (enumType != null)
				{
					Support.ResourceManager manager = Widgets.Helpers.VisualTree.GetResourceManager (this.host.Placeholder);

					foreach (IEnumValue enumValue in enumType.Values)
					{
						if (!enumValue.IsHidden)
						{
							Caption evCaption = Support.CaptionCache.Instance.GetCaption (manager, enumValue.CaptionId);
							string evCaptionText = null;
							string evCaptionDesc = null;

							if ((evCaption != null) &&
								(evCaption.Labels.Count > 0))
							{
								evCaptionText = Collection.Extract (evCaption.Labels, 0);
							}

							if (evCaption != null)
							{
								evCaptionDesc = evCaption.Description;
							}

							string icon = evCaption.Icon;
							string text = evCaptionText ?? enumValue.Name;
							string tip  = evCaptionDesc ?? text;

							int value = this.enumType.IsDefinedAsFlags ? EnumType.ConvertToInt (enumValue.Value) : enumValue.Rank;

							this.combo.AddRadioIcon (icon, tip, value, false);
						}
					}
				}
			}

			public override IEnumerable<Widgets.Layouts.PermeableCell> GetChildren(int column, int row, int columnSpan, int rowSpan)
			{
				yield return new Widgets.Layouts.PermeableCell (this.label, column+0, row+0, 1, 1);
				yield return new Widgets.Layouts.PermeableCell (this.combo, column+1, row+0, columnSpan-1, 1);
			}

			public override bool UpdateGridSpan(ref int columnSpan, ref int rowSpan)
			{
				columnSpan = System.Math.Max (columnSpan, 2);
				rowSpan    = System.Math.Max (rowSpan, 1);

				return true;
			}

			private RadioIconGrid combo;
			private StaticText label;
			private IEnumType enumType;
		}

		private Helper helper;
	}
}
