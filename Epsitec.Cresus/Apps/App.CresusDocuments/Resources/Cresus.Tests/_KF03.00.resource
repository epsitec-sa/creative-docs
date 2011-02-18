<?xml version="1.0" encoding="utf-8"?>
<bundle name="_KF03" caption="Part.ContactAddressEdit" type="Panel" culture="fr" rank="3">
  <data name="Panel">
    <xml>
      <panel xmlns:s="http://www.epsitec.ch/XNS/storage-structure-1" xmlns:f="http://www.epsitec.ch/XNS/storage-fields-1">
        <s:storage s:root="_1" s:n_ext="2" s:n_typ="11" s:n_obj="20">
          <s:external s:name="_DataSource" />
          <s:external s:name="_ResourceManager" />
          <s:type id="_1" s:name="Epsitec.Common.UI.Panel" />
          <s:type id="_2" s:name="Epsitec.Common.Support.ResourceManager" />
          <s:type id="_3" s:name="Epsitec.Common.Types.DataObject" />
          <s:type id="_4" s:name="Epsitec.Common.Widgets.Layouts.LayoutEngine" />
          <s:type id="_5" s:name="Epsitec.Common.Widgets.Layouts.GridLayoutEngine" />
          <s:type id="_6" s:name="Epsitec.Common.Widgets.Layouts.RowDefinition" />
          <s:type id="_7" s:name="Epsitec.Common.Widgets.Layouts.ColumnDefinition" />
          <s:type id="_8" s:name="Epsitec.Common.UI.Placeholder" />
          <s:type id="_9" s:name="Epsitec.Common.UI.DataSourceMetadata" />
          <s:type id="_10" s:name="Epsitec.Common.UI.Panels.EditPanel" />
          <s:type id="_11" s:name="Epsitec.Common.UI.Panels.SearchPanel" />
          <s:object id="_1" s:type="_1" />
          <s:object id="_2" s:type="_5" />
          <s:object id="_3" s:type="_6" />
          <s:object id="_4" s:type="_6" />
          <s:object id="_5" s:type="_6" />
          <s:object id="_6" s:type="_6" />
          <s:object id="_7" s:type="_6" />
          <s:object id="_8" s:type="_6" />
          <s:object id="_9" s:type="_7" />
          <s:object id="_10" s:type="_7" />
          <s:object id="_11" s:type="_7" />
          <s:object id="_12" s:type="_8" />
          <s:object id="_13" s:type="_8" />
          <s:object id="_14" s:type="_8" />
          <s:object id="_15" s:type="_8" />
          <s:object id="_16" s:type="_8" />
          <s:object id="_17" s:type="_8" />
          <s:object id="_18" s:type="_9" />
          <s:object id="_19" s:type="_10" />
          <s:object id="_20" s:type="_11" />
          <s:data id="_1" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Grid" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="260" f:PreferredHeight="160" f:Anchor="BottomLeft" f:Padding="0;0;10;10" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:_1.BundleId="[KF03]" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:_4.LayoutEngine="{ObjRef _2}" f:Children="{Collection {ObjRef _12}, {ObjRef _13}, {ObjRef _14}, {ObjRef _15}, {ObjRef _16}, {ObjRef _17}}" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _18}" f:EditionPanel="{ObjRef _19}" f:SearchPanel="{ObjRef _20}" f:Text="" />
          <s:data id="_2" f:ColumnDefinitions="{Collection {ObjRef _3}, {ObjRef _4}, {ObjRef _5}, {ObjRef _6}, {ObjRef _7}, {ObjRef _8}}" f:RowDefinitions="{Collection {ObjRef _9}, {ObjRef _10}, {ObjRef _11}}" />
          <s:data id="_3" f:Height="Auto" f:MinHeight="20" f:BottomBorder="4" />
          <s:data id="_4" f:Height="Auto" f:MinHeight="20" f:MaxHeight="9999" f:BottomBorder="4" />
          <s:data id="_5" f:Height="Auto" f:MinHeight="20" f:MaxHeight="9999" f:BottomBorder="4" />
          <s:data id="_6" f:Height="Auto" f:MinHeight="20" f:MaxHeight="9999" f:BottomBorder="4" />
          <s:data id="_7" f:Height="Auto" f:MinHeight="20" f:MaxHeight="9999" f:BottomBorder="4" />
          <s:data id="_8" f:Height="Auto" f:MinHeight="20" f:MaxHeight="9999" f:BottomBorder="0" />
          <s:data id="_9" f:Width="Auto" f:MinWidth="20" />
          <s:data id="_10" f:Width="Auto" f:MinWidth="48" />
          <s:data id="_11" f:Width="*100" f:MinWidth="20" f:MaxWidth="9999" />
          <s:data id="_12" f:Value="{Binding Path=*.FirstName, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:Anchor="None" f:Dock="None" f:_5.Column="0" f:_5.Row="0" f:ContainerLayoutMode="HorizontalFlow" f:_5.ColumnSpan="3" />
          <s:data id="_13" f:Value="{Binding Path=*.LastName, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:Anchor="None" f:Dock="None" f:_5.Column="0" f:_5.Row="1" f:ContainerLayoutMode="HorizontalFlow" f:_5.ColumnSpan="3" f:TabIndex="1" />
          <s:data id="_14" f:Value="{Binding Path=*.Company, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:Anchor="None" f:Dock="None" f:_5.Column="0" f:_5.Row="2" f:ContainerLayoutMode="HorizontalFlow" f:_5.ColumnSpan="3" f:TabIndex="2" />
          <s:data id="_15" f:Value="{Binding Path=*.Address1, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:Anchor="None" f:Dock="None" f:_5.Column="0" f:_5.Row="3" f:ContainerLayoutMode="HorizontalFlow" f:_5.ColumnSpan="3" f:TabIndex="3" />
          <s:data id="_16" f:Value="{Binding Path=*.Zip, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:Anchor="None" f:Dock="None" f:_5.Column="0" f:_5.Row="4" f:ContainerLayoutMode="HorizontalFlow" f:TabIndex="4" />
          <s:data id="_17" f:Value="{Binding Path=*.City, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:Anchor="None" f:Dock="None" f:_5.Column="0" f:_5.Row="5" f:ContainerLayoutMode="HorizontalFlow" f:_5.ColumnSpan="3" f:TabIndex="5" />
          <s:data id="_18" f:Fields="{Collection {Text '*;[KF0E]'}}" />
          <s:data id="_19" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Grid" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="260" f:PreferredHeight="186" f:Anchor="BottomLeft" f:Dock="None" f:Margins="0;0;0;0" f:Padding="10;10;10;10" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _18}" f:EditionPanel="{ObjRef _19}" f:Owner="{ObjRef _1}" f:SearchPanel="{ObjRef _20}" f:Text="" />
          <s:data id="_20" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Grid" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="260" f:PreferredHeight="160" f:Anchor="BottomLeft" f:Dock="None" f:Margins="0;0;0;0" f:Padding="0;0;10;10" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _18}" f:EditionPanel="{ObjRef _19}" f:Owner="{ObjRef _1}" f:SearchPanel="{ObjRef _20}" f:Text="" />
        </s:storage>
      </panel>
    </xml>
  </data>
  <data name="DefaultSize">260;160</data>
  <data name="RootEntityId">
  </data>
</bundle>