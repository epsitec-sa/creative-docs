<bundle>
 <class>Window</class>
 <name>my_window</name>
 <text>Ressource: démo</text>
 <size>240;280</size>
 <widgets>
  <list>
   <bundle>
    <class>CheckButton</class>
    <text>Bouton A</text>
    <name>checkA</name>
    <cmd>on</cmd>
    <size>200;*</size>
    <pos>10;210</pos>
   </bundle>
   <bundle>
    <class>GroupBox</class>
    <anchor>Left, Right, Top, Bottom</anchor>
    <text>Choix cornélien</text>
    <size>215;85</size>
    <pos>10;60</pos>
    <dock_h>no</dock_h>
    <dock_m>5;5;20;5</dock_m>
    <widgets>
     <list>
      <bundle>
       <class>RadioButton</class>
       <text>Bouton B</text>
       <dock>Fill</dock>
      </bundle>
      <bundle>
       <class>RadioButton</class>
       <text>Bouton C</text>
       <dock>Fill</dock>
      </bundle>
      <bundle>
       <class>RadioButton</class>
       <text>Bouton D</text>
       <dock>Fill</dock>
      </bundle>
     </list>
    </widgets>
   </bundle>
   <bundle>
    <ref t='button.cancel'/>
    <cmd>on</cmd>
    <pos>10;10</pos>
   </bundle>
  </list>
 </widgets>
</bundle>
