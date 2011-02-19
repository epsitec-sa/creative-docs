<?xml version="1.0" encoding="utf-8"?>
<bundle name="_KF06" caption="TestAvecHeritage" type="Panel" culture="fr" rank="6">
  <data name="Panel">
    <xml>
      <panel xmlns:s="http://www.epsitec.ch/XNS/storage-structure-1" xmlns:f="http://www.epsitec.ch/XNS/storage-fields-1">
        <s:storage s:root="_1" s:n_ext="2" s:n_typ="7" s:n_obj="5">
          <s:external s:name="_DataSource" />
          <s:external s:name="_ResourceManager" />
          <s:type id="_1" s:name="Epsitec.Common.UI.Panel" />
          <s:type id="_2" s:name="Epsitec.Common.Support.ResourceManager" />
          <s:type id="_3" s:name="Epsitec.Common.Types.DataObject" />
          <s:type id="_4" s:name="Epsitec.Common.UI.PanelPlaceholder" />
          <s:type id="_5" s:name="Epsitec.Common.UI.DataSourceMetadata" />
          <s:type id="_6" s:name="Epsitec.Common.UI.Panels.EditPanel" />
          <s:type id="_7" s:name="Epsitec.Common.UI.Panels.SearchPanel" />
          <s:object id="_1" s:type="_1" />
          <s:object id="_2" s:type="_4" />
          <s:object id="_3" s:type="_5" />
          <s:object id="_4" s:type="_6" />
          <s:object id="_5" s:type="_7" />
          <s:data id="_1" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="200" f:PreferredHeight="200" f:Anchor="BottomLeft" f:Padding="20;20;20;20" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:_1.BundleId="[KF06]" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:Children="{Collection {ObjRef _2}}" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _3}" f:EditionPanel="{ObjRef _4}" f:SearchPanel="{ObjRef _5}" f:Text="" />
          <s:data id="_2" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:Text="&lt;i&gt;Panel&lt;/i&gt;" f:PreferredWidth="200" f:Margins="5;5;5;5" f:PreferredHeight="100" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:Dock="StackBegin" f:Anchor="None" f:PanelId="[KF03]" />
          <s:data id="_3" f:Fields="{Collection {Text '*;[KF0E]'}}" />
          <s:data id="_4" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="200" f:PreferredHeight="200" f:Anchor="BottomLeft" f:Dock="None" f:Margins="0;0;0;0" f:Padding="20;20;20;20" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _3}" f:EditionPanel="{ObjRef _4}" f:Owner="{ObjRef _1}" f:SearchPanel="{ObjRef _5}" f:Text="" />
          <s:data id="_5" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="200" f:PreferredHeight="200" f:Anchor="BottomLeft" f:Dock="None" f:Margins="0;0;0;0" f:Padding="20;20;20;20" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _3}" f:EditionPanel="{ObjRef _4}" f:Owner="{ObjRef _1}" f:SearchPanel="{ObjRef _5}" f:Text="" />
        </s:storage>
      </panel>
    </xml>
  </data>
  <data name="DefaultSize">200;200</data>
  <data name="RootEntityId"></data>
</bundle>