<bundle name="dialog_with_data">
  <data name="class">Window</data>
  <data name="WindowStyles">None</data>
  <data name="WindowType">Dialog</data>
  <data name="Size">364;210</data>
  <data name="Name">Root</data>
  <data name="Text">[res:strings#title.SettingsWindow]</data>
  <list name="widgets">
    <bundle>
      <data name="class">DataWidget</data>
      <data name="Representation">TextField</data>
      <data name="CaptionWidth">100</data>
      <data name="Anchor">TopLeft</data>
      <data name="AnchorMargins">8;86;16;166</data>
      <data name="Size">270;21</data>
      <data name="BindingInfo">&lt;bind path="UserName"/&gt;</data>
    </bundle>
    <bundle>
      <data name="class">DataWidget</data>
      <data name="Representation">NumericUpDown</data>
      <data name="CaptionWidth">100</data>
      <data name="Anchor">TopLeft</data>
      <data name="AnchorMargins">8;136;39;143</data>
      <data name="Size">220;21</data>
      <data name="BindingInfo">&lt;bind path="UserAge"/&gt;</data>
    </bundle>
    <bundle>
      <data name="class">DataWidget</data>
      <data name="Representation">RadioColumns</data>
      <data name="Anchor">TopLeft</data>
      <data name="AnchorMargins">8;8;62;170</data>
      <data name="Size">348;87</data>
      <data name="BindingInfo">&lt;bind path="c"/&gt;</data>
    </bundle>
    <bundle>
      <data name="class">Button</data>
      <data name="Anchor">BottomRight</data>
      <data name="AnchorMargins">188;96;171;16</data>
      <data name="Command">Ok</data>
      <data name="Text">[res:strings#label.OK]</data>
    </bundle>
    <bundle>
      <data name="class">Button</data>
      <data name="Anchor">BottomRight</data>
      <data name="AnchorMargins">276;8;171;16</data>
      <data name="Command">Cancel</data>
      <data name="Text">[res:strings#label.Cancel]</data>
    </bundle>
  </list>
</bundle>