<bundle name="ServerManager.MainWindow" type="Interface.DialogWindow">
  <data name="class">Window</data>
  <data name="Size">249;159</data>
  <data name="Name">Application</data>
  <data name="Text">Gestionnaire Crésus Serveur</data>
  <list name="widgets">
    <bundle>
      <data name="class">Button</data>
      <data name="Anchor">TopLeft</data>
      <data name="AnchorMargins">8;267;91;45</data>
      <data name="Command">StartService</data>
      <data name="Text">Démarrer</data>
    </bundle>
    <bundle>
      <data name="class">Button</data>
      <data name="Anchor">BottomLeft</data>
      <data name="AnchorMargins">8;267;120;16</data>
      <data name="Command">StopService</data>
      <data name="Text">Arrêter</data>
    </bundle>
    <bundle>
      <data name="class">StaticTextLarge</data>
      <data name="Anchor">TopLeft</data>
      <data name="AnchorMargins">8;8;16;126</data>
      <data name="Size">339;17</data>
      <data name="Text">Etat du service</data>
    </bundle>
    <bundle>
      <data name="class">DataWidget</data>
      <data name="Representation">StatusLed</data>
      <data name="Anchor">TopLeft</data>
      <data name="AnchorMargins">41;106;58;88</data>
      <data name="Size">102;13</data>
      <data name="BindingInfo">&lt;bind path="IsRunning"/&gt;</data>
    </bundle>
  </list>
</bundle>