using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'éditer un Caption.Type.
	/// </summary>
	public class TypeEditorBoolean : AbstractTypeEditor
	{
		public TypeEditorBoolean(Module module)
		{
			this.module = module;

			this.groupDefault = new ResetBox(this);
			this.groupDefault.IsPatch = this.module.IsPatch;
			this.groupDefault.Dock = DockStyle.StackBegin;
			this.groupDefault.Margins = new Margins(0, 0, 0, 0);
			this.groupDefault.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);

			this.checkDefault = new CheckButton(this.groupDefault.GroupBox);
			this.checkDefault.AutoToggle = false;
			this.checkDefault.Text = Res.Strings.Viewers.Types.Boolean.Default;
			this.checkDefault.Dock = DockStyle.Fill;
			this.checkDefault.Clicked += new MessageEventHandler(this.HandleCheckClicked);
		}

		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.groupDefault.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);
				this.checkDefault.Clicked -= new MessageEventHandler(this.HandleCheckClicked);
			}
			
			base.Dispose(disposing);
		}


		public override string GetSummary()
		{
			//	Retourne le texte du résumé.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			this.PutSummaryInitialise();
			object value;

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceBaseType.DefaultValue);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				bool def = (bool) value;
				this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Default, def ? Res.Strings.Viewers.Types.Boolean.True : Res.Strings.Viewers.Types.Boolean.False);
			}

			return builder.ToString();
		}


		public override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
			this.ignoreChange = true;
			object value;
			bool usesOriginalData;

			CultureMapSource source = this.module.AccessTypes.GetCultureMapSource(this.cultureMap);

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceBaseType.DefaultValue, out usesOriginalData);
			this.ColorizeResetBox(this.groupDefault, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.checkDefault.ActiveState = ActiveState.No;
			}
			else
			{
				bool def = (bool) value;
				this.checkDefault.ActiveState = def ? ActiveState.Yes : ActiveState.No;
			}

			this.ignoreChange = false;
		}


		private void HandleCheckClicked(object sender, MessageEventArgs e)
		{
			if (this.ignoreChange)
			{
				return;
			}

			if (sender == this.checkDefault)
			{
				bool def = false;
				object value = this.structuredData.GetValue(Support.Res.Fields.ResourceBaseType.DefaultValue);
				if (!UndefinedValue.IsUndefinedValue(value))
				{
					def = (bool) value;
				}

				this.structuredData.SetValue(Support.Res.Fields.ResourceBaseType.DefaultValue, !def);
			}

			this.OnContentChanged();
			this.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
		}

		private void HandleResetButtonClicked(object sender, MessageEventArgs e)
		{
			AbstractButton button = sender as AbstractButton;

			if (button == this.groupDefault.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceBaseType.DefaultValue);
			}

			this.OnContentChanged();
			this.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
		}


		protected ResetBox						groupDefault;
		protected CheckButton					checkDefault;
	}
}
