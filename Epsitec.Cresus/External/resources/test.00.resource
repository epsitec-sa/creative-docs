<bundle name="test">
  <data name="class">Window</data>
  <data name="WindowStyles">HasCloseButton, HasHelpButton</data>
  <data name="WindowType">Dialog</data>
  <data name="Size">400;300</data>
  <data name="Name">Root</data>
  <data name="Text">[res:strings#title.SettingsWindow]</data>
  <list name="widgets">
    <bundle>
      <data name="class">Button</data>
      <data name="Anchor">BottomRight</data>
      <data name="AnchorMargins">312;8;261;16</data>
      <data name="Text">[res:strings#label.Cancel]</data>
    </bundle>
    <bundle>
      <data name="class">Button</data>
      <data name="Anchor">BottomRight</data>
      <data name="AnchorMargins">224;96;261;16</data>
      <data name="Text">[res:strings#label.OK]</data>
    </bundle>
    <bundle>
      <data name="class">StaticTextSmall</data>
      <data name="Anchor">BottomLeft</data>
      <data name="AnchorMargins">8;276;266.92873046875;21.07126953125</data>
      <data name="Size">116;12</data>
      <data name="Text">[res:strings#label.Copyright]</data>
    </bundle>
    <bundle>
      <data name="class">GroupBox</data>
      <data name="Anchor">TopLeft</data>
      <data name="AnchorMargins">8;8;16;45</data>
      <data name="Size">384;239</data>
      <data name="Text">[res:strings#group.Options]</data>
    </bundle>
  </list>
</bundle>