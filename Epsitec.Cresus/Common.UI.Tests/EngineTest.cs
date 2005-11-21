using NUnit.Framework;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.UI
{
	[TestFixture] public class EngineTest
	{
		[SetUp] public void SetUp()
		{
			Common.Widgets.Widget.Initialise ();
			Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
			
			Support.Resources.SetupApplication ("Test");
		}
		
		[Test] public void CheckDataRecordAndFields()
		{
			Data.Record      record = EngineTest.CreateRecord ();
			Types.IDataGraph graph  = record;
			
			Assert.AreEqual (record, graph.Root);
			Assert.AreEqual (6, graph.Root.Count);
			
			Types.IDataValue v1 = graph.Navigate ("FontName")  as Types.IDataValue;
			Types.IDataValue v2 = graph.Navigate ("FontSize")  as Types.IDataValue;
			Types.IDataValue v3 = graph.Navigate ("UseHyphen") as Types.IDataValue;
			Types.IDataValue v4 = graph.Navigate ("FontStyle") as Types.IDataValue;
			Types.IDataValue v5 = graph.Navigate ("Quality")   as Types.IDataValue;
			Types.IDataValue v6 = graph.Navigate ("Optical")   as Types.IDataValue;
			
			Assert.IsNotNull (v1);
			Assert.IsNotNull (v2);
			Assert.IsNotNull (v3);
			Assert.IsNotNull (v4);
			Assert.IsNotNull (v5);
			Assert.IsNotNull (v6);
			
			Assert.AreEqual ("String",      v1.DataType.Name);
			Assert.AreEqual ("Decimal",     v2.DataType.Name);
			Assert.AreEqual ("Boolean",     v3.DataType.Name);
			Assert.AreEqual ("Integer",     v4.DataType.Name);
			Assert.AreEqual ("Enumeration", v5.DataType.Name);
			Assert.AreEqual ("Enumeration", v6.DataType.Name);
			
			Types.IEnum e1 = v5.DataType as Types.IEnum;
			Types.IEnum e2 = v6.DataType as Types.IEnum;
			
			Assert.IsFalse (e1.IsCustomizable);
			Assert.IsTrue  (e2.IsCustomizable);
			
			Assert.AreEqual (v1, record["FontName"]);
			Assert.AreEqual (v2, record["FontSize"]);
			Assert.AreEqual (v3, record["UseHyphen"]);
			Assert.AreEqual (v4, record["FontStyle"]);
			Assert.AreEqual (v5, record["Quality"]);
			Assert.AreEqual (v6, record["Optical"]);
			
			Assert.AreEqual (typeof (string),  v1.DataType.SystemType);
			Assert.AreEqual (typeof (decimal), v2.DataType.SystemType);
			Assert.AreEqual (typeof (bool),    v3.DataType.SystemType);
			Assert.AreEqual (typeof (int),     v4.DataType.SystemType);
			Assert.AreEqual (typeof (Quality), v5.DataType.SystemType);
			Assert.AreEqual (typeof (string) , v6.DataType.SystemType);
			
			Assert.AreEqual ("Times", v1.ReadValue ());
			Assert.AreEqual (12.0, (double)(decimal) v2.ReadValue ());
			Assert.AreEqual (false, v3.ReadValue ());
			Assert.AreEqual (1, (int) v4.ReadValue ());
			Assert.AreEqual (Quality.Default, v5.ReadValue ());
			Assert.AreEqual ("Default", v6.ReadValue ());
			
			v5.WriteValue (Quality.Fast);
			v6.WriteValue (Optical.Small.ToString ());
			
			Assert.AreEqual (Quality.Fast, v5.ReadValue ());
			Assert.AreEqual ("Small", v6.ReadValue ());
		}
		
		[Test] public void CheckConstraint()
		{
			Data.Record record = EngineTest.CreateRecord ();
			
			TextField       text    = new TextField ();
			TextFieldUpDown up_down = new TextFieldUpDown ();
			
			Engine.BindWidget (record, text,    @"<bind path=""FontName"" />");
			Engine.BindWidget (record, up_down, @"<bind path=""FontSize"" />");
			
			Assert.AreEqual ("Times", text.Text);
			Assert.AreEqual ("Times", record["FontName"].ReadValue ());
			Assert.AreEqual (12.0, (double) up_down.Value);
			Assert.AreEqual (12.0, (double) (decimal) record["FontSize"].ReadValue ());
			
			text.Text = "XYZ";		//	pas accepté par la contrainte XStringConstraint
			
			Assert.AreEqual ("Times", record["FontName"].ReadValue ());
			
			up_down.Text = "-5.5";	//	pas accepté
			
			Assert.AreEqual (12.0, (double) (decimal) record["FontSize"].ReadValue ());
			
			up_down.Value = -5.5M;	//	accepté, parce que Value contraint à [1..299]
			
			Assert.AreEqual (1.0, (double) (decimal) record["FontSize"].ReadValue ());
			
			text.Text = "A";		//	pas accepté par la contrainte XStringConstraint
			
			Assert.AreEqual ("Times", record["FontName"].ReadValue ());
			
			text.Text = "ABC";		//	modification OK
			
			Assert.AreEqual ("ABC", record["FontName"].ReadValue ());
		}
		
		[Test] public void CheckBindWidget()
		{
			Data.Record record = EngineTest.CreateRecord ();
			
			Widget          parent  = new Widget ();
			TextField       text    = new TextField (parent);
			TextFieldUpDown up_down = new TextFieldUpDown (parent);
			CheckButton     check_b = new CheckButton (parent);
			RadioButton     radio_1 = new RadioButton (parent, "Group A", 1);
			RadioButton     radio_2 = new RadioButton (parent, "Group A", 2);
			RadioButton     radio_3 = new RadioButton (parent, "Group A", 3);
			
			RadioButton     radio_4 = new RadioButton (parent, "Group B", (int) Quality.Default);
			RadioButton     radio_5 = new RadioButton (parent, "Group B", (int) Quality.Smooth);
			RadioButton     radio_6 = new RadioButton (parent, "Group B", (int) Quality.Fast);
			
			TextFieldCombo  combo_r = new TextFieldCombo (parent);
			TextFieldCombo  combo_w = new TextFieldCombo (parent);
			
			Assert.IsNotNull (combo_r.GetType ().GetInterface ("IReadOnly"), "TextFieldCombo does not implement IReadOnly.");
			
			combo_r.IsReadOnly = true;
			combo_w.IsReadOnly = false;
			
			Engine.BindWidget (record, text,    @"<bind path=""FontName"" />");
			Engine.BindWidget (record, up_down, @"<bind path=""FontSize"" />");
			Engine.BindWidget (record, check_b, @"<bind path=""UseHyphen"" />");
			Engine.BindWidget (record, radio_1, @"<bind path=""FontStyle"" />");
			Engine.BindWidget (record, radio_4, @"<bind path=""Quality"" />");
			Engine.BindWidget (record, combo_r, @"<bind path=""Quality"" />");
			Engine.BindWidget (record, combo_w, @"<bind path=""Optical"" />");
			
			Assert.AreEqual ("Times", text.Text);
			
			Assert.AreEqual ( 12.0, (double) up_down.Value);
			Assert.AreEqual (  1.0, (double) up_down.MinValue);
			Assert.AreEqual (299.0, (double) up_down.MaxValue);
			Assert.AreEqual (  0.1, (double) up_down.Resolution);
			
			Assert.AreEqual (false, check_b.IsActive);
			
			Assert.AreEqual (true,  radio_1.IsActive);
			Assert.AreEqual (false, radio_2.IsActive);
			Assert.AreEqual (false, radio_3.IsActive);
			
			Assert.AreEqual (true,  radio_4.IsActive);
			Assert.AreEqual (false, radio_5.IsActive);
			Assert.AreEqual (false, radio_6.IsActive);
			
			Assert.AreEqual (3, combo_r.Items.Count);
			Assert.AreEqual ("Default", combo_r.Items.GetName (0));
			Assert.AreEqual ("Smooth",  combo_r.Items.GetName (1));
			Assert.AreEqual ("Fast",    combo_r.Items.GetName (2));
			Assert.AreEqual (0, combo_r.Items.FindNameIndex ("Default"));
			Assert.AreEqual (1, combo_r.Items.FindNameIndex ("Smooth"));
			Assert.AreEqual (2, combo_r.Items.FindNameIndex ("Fast"));
			Assert.AreEqual (0, combo_r.SelectedIndex);
			Assert.AreEqual ("Default", combo_r.SelectedName);
			
			Assert.AreEqual (3, combo_w.Items.Count);
			Assert.AreEqual ("Default", combo_w.Items.GetName (0));
			Assert.AreEqual ("Small",   combo_w.Items.GetName (1));
			Assert.AreEqual ("Large",   combo_w.Items.GetName (2));
			Assert.AreEqual (0, combo_w.Items.FindNameIndex ("Default"));
			Assert.AreEqual (1, combo_w.Items.FindNameIndex ("Small"));
			Assert.AreEqual (2, combo_w.Items.FindNameIndex ("Large"));
			Assert.AreEqual (0, combo_w.SelectedIndex);
			Assert.AreEqual ("Default", combo_w.SelectedName);
			
			text.Text           = "Helvetica";
			up_down.Value       = 14M;
			check_b.ActiveState = ActiveState.Yes;
			radio_2.ActiveState = ActiveState.Yes;
			radio_5.ActiveState = ActiveState.Yes;
			combo_w.Text        = "Heading";
			
			Assert.AreEqual (false, radio_1.IsActive);
			Assert.AreEqual (true,  radio_2.IsActive);
			Assert.AreEqual (false, radio_3.IsActive);
			
			Assert.AreEqual (false, radio_4.IsActive);
			Assert.AreEqual (true,  radio_5.IsActive);
			Assert.AreEqual (false, radio_6.IsActive);
			
			Assert.AreEqual (1,        combo_r.SelectedIndex);
			Assert.AreEqual ("Smooth", combo_r.SelectedName);
			
			Assert.AreEqual (-1, combo_w.SelectedIndex);
			Assert.AreEqual ("", combo_w.SelectedName);
			
			Assert.AreEqual ("Helvetica",             record["FontName"].ReadValue ());
			Assert.AreEqual (14.0, (double) (decimal) record["FontSize"].ReadValue ());
			Assert.AreEqual (true,                    record["UseHyphen"].ReadValue ());
			Assert.AreEqual (2,                 (int) record["FontStyle"].ReadValue ());
			Assert.AreEqual (Quality.Smooth,          record["Quality"].ReadValue ());
			Assert.AreEqual ("{Heading}",             record["Optical"].ReadValue ());
			
			record["FontName"].WriteValue ("Courier");
			record["FontSize"].WriteValue (10M);
			record["UseHyphen"].WriteValue (false);
			record["FontStyle"].WriteValue (3);
			record["Quality"].WriteValue (Quality.Fast);
			record["Optical"].WriteValue ("{Subtitle}");
			
			Assert.AreEqual ("Courier", text.Text);
			Assert.AreEqual (10.0, (double) up_down.Value);
			Assert.AreEqual (false, check_b.IsActive);
			
			Assert.AreEqual (false, radio_1.IsActive);
			Assert.AreEqual (false, radio_2.IsActive);
			Assert.AreEqual (true,  radio_3.IsActive);
			
			Assert.AreEqual (false, radio_4.IsActive);
			Assert.AreEqual (false, radio_5.IsActive);
			Assert.AreEqual (true,  radio_6.IsActive);
			
			Assert.AreEqual (2, combo_r.SelectedIndex);
			Assert.AreEqual ("Fast", combo_r.SelectedName);
			
			Assert.AreEqual ("Subtitle", combo_w.Text);
			Assert.AreEqual ("",         combo_w.SelectedItem);
			Assert.AreEqual (-1,         combo_w.SelectedIndex);
			Assert.AreEqual ("",         combo_w.SelectedName);
			
			combo_r.SelectedName = "Smooth";
			combo_w.SelectedName = "Large";
			
			Assert.AreEqual (Quality.Smooth, record["Quality"].ReadValue ());
			
			Assert.AreEqual (false, radio_4.IsActive);
			Assert.AreEqual (true,  radio_5.IsActive);
			Assert.AreEqual (false, radio_6.IsActive);
			
			Assert.AreEqual ("Optique grandes fontes", combo_w.Text);
			Assert.AreEqual ("Optique grandes fontes", combo_w.SelectedItem);
			Assert.AreEqual (2,                        combo_w.SelectedIndex);
			Assert.AreEqual ("Large",                  combo_w.SelectedName);
			
			combo_r.SelectedName = "Default";
			
			Assert.AreEqual (Quality.Default, record["Quality"].ReadValue ());
			
			combo_r.SelectedName = "";
			
			Assert.AreEqual (Quality.Default, record["Quality"].ReadValue ());
		}
		
		[Test] public void CheckBindWidgetsFromBundle()
		{
			Data.Record record = EngineTest.CreateRecord ();
			
			Support.ResourceBundle bundle  = Support.Resources.GetBundle ("file:binding_form");
			Support.ObjectBundler  bundler = new Support.ObjectBundler (Support.Resources.DefaultManager);
			
			Assert.IsNotNull (bundle);
			
			Widget root = bundler.CreateFromBundle (bundle) as Widget;
			
			Assert.IsNotNull (root);
			
			Engine.BindWidgets (record, root);
			root.Window.Show ();
		}
		
		
		private static Data.Record CreateRecord()
		{
			Data.Record record = new Data.Record ();
			
			record.Add (new Data.Field ("FontName", "Times", null, new XStringConstraint ()));
			record.Add (new Data.Field ("FontSize", 12M, new Types.DecimalType (1.0M, 299.0M, 0.1M)));
			record.Add (new Data.Field ("UseHyphen", false));
			record.Add (new Data.Field ("FontStyle", 1));
			record.Add (new Data.Field ("Quality", Quality.Default));
			record.Add (new Data.Field ("Optical", Optical.Default.ToString (), new Types.OpenEnumType (typeof (Optical))));
			
			record["FontName"] .DefineCaption ("Nom de la fonte");
			record["FontSize"] .DefineCaption ("Taille de la fonte");
			record["UseHyphen"].DefineCaption ("Utilise la césure");
			record["FontStyle"].DefineCaption ("Style de fonte");
			record["Quality"]  .DefineCaption ("Qualité");
			
			Types.EnumType font_quality_enum = record["Quality"].DataType as Types.EnumType;
			Types.EnumType font_optical_enum = record["Optical"].DataType as Types.EnumType;
			
			font_quality_enum[Quality.Default].DefineCaption ("Qualité standard");
			font_quality_enum[Quality.Smooth] .DefineCaption ("Lissage de la police");
			font_quality_enum[Quality.Fast]   .DefineCaption ("Affichage rapide");
			
			font_optical_enum[Optical.Default].DefineCaption ("Optique standard");
			font_optical_enum[Optical.Large]  .DefineCaption ("Optique grandes fontes");
			font_optical_enum[Optical.Small]  .DefineCaption ("Optique petites fontes");
			
			return record;
		}
		
		private enum Quality
		{
			Default,
			Smooth,
			Fast
		}
		
		private enum Optical
		{
			Default	= 0,
			Small	= 5,
			Large	= 10
		}
		
		private class XStringConstraint : Types.IDataConstraint
		{
			public XStringConstraint()
			{
			}
			
			
			#region IDataConstraint Members
			public bool CheckConstraint(object value)
			{
				string text = value as string;
				
				if ((text != null) &&
					(text.Length > 1) &&
					(text[0] != 'X'))
				{
					return true;
				}
				
				return false;
			}
			#endregion
		}

	}
}
