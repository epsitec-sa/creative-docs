<?xml version="1.0" encoding="utf-8"?>
<bundle name="_KF" caption="Part.ContactAddress" type="Panel" culture="fr" rank="5">
  <data name="Panel">
    <xml>
      <panel xmlns:s="http://www.epsitec.ch/XNS/storage-structure-1" xmlns:f="http://www.epsitec.ch/XNS/storage-fields-1">
        <s:storage s:root="_1" s:n_ext="2" s:n_typ="13" s:n_obj="43">
          <s:external s:name="_DataSource" />
          <s:external s:name="_ResourceManager" />
          <s:type id="_1" s:name="Epsitec.Common.UI.Panel" />
          <s:type id="_2" s:name="Epsitec.Common.Support.ResourceManager" />
          <s:type id="_3" s:name="Epsitec.Common.Types.DataObject" />
          <s:type id="_4" s:name="Epsitec.Common.Widgets.FrameBox" />
          <s:type id="_5" s:name="Epsitec.Common.UI.Placeholder" />
          <s:type id="_6" s:name="Epsitec.Common.UI.DataSourceMetadata" />
          <s:type id="_7" s:name="Epsitec.Common.UI.Panels.EditPanel" />
          <s:type id="_8" s:name="Epsitec.Common.Widgets.Layouts.LayoutEngine" />
          <s:type id="_9" s:name="Epsitec.Common.Widgets.Layouts.GridLayoutEngine" />
          <s:type id="_10" s:name="Epsitec.Common.Widgets.Layouts.RowDefinition" />
          <s:type id="_11" s:name="Epsitec.Common.Widgets.Layouts.ColumnDefinition" />
          <s:type id="_12" s:name="Epsitec.Common.UI.Panels.SearchPanel" />
          <s:type id="_13" s:name="Epsitec.Common.Widgets.StaticText" />
          <s:object id="_1" s:type="_1" />
          <s:object id="_2" s:type="_4" />
          <s:object id="_3" s:type="_5" />
          <s:object id="_4" s:type="_5" />
          <s:object id="_5" s:type="_5" />
          <s:object id="_6" s:type="_5" />
          <s:object id="_7" s:type="_4" />
          <s:object id="_8" s:type="_5" />
          <s:object id="_9" s:type="_5" />
          <s:object id="_10" s:type="_6" />
          <s:object id="_11" s:type="_7" />
          <s:object id="_12" s:type="_9" />
          <s:object id="_13" s:type="_10" />
          <s:object id="_14" s:type="_10" />
          <s:object id="_15" s:type="_10" />
          <s:object id="_16" s:type="_10" />
          <s:object id="_17" s:type="_11" />
          <s:object id="_18" s:type="_11" />
          <s:object id="_19" s:type="_11" />
          <s:object id="_20" s:type="_11" />
          <s:object id="_21" s:type="_11" />
          <s:object id="_22" s:type="_11" />
          <s:object id="_23" s:type="_5" />
          <s:object id="_24" s:type="_5" />
          <s:object id="_25" s:type="_5" />
          <s:object id="_26" s:type="_5" />
          <s:object id="_27" s:type="_5" />
          <s:object id="_28" s:type="_5" />
          <s:object id="_29" s:type="_12" />
          <s:object id="_30" s:type="_13" />
          <s:object id="_31" s:type="_4" />
          <s:object id="_32" s:type="_9" />
          <s:object id="_33" s:type="_10" />
          <s:object id="_34" s:type="_10" />
          <s:object id="_35" s:type="_10" />
          <s:object id="_36" s:type="_10" />
          <s:object id="_37" s:type="_11" />
          <s:object id="_38" s:type="_11" />
          <s:object id="_39" s:type="_11" />
          <s:object id="_40" s:type="_5" />
          <s:object id="_41" s:type="_5" />
          <s:object id="_42" s:type="_5" />
          <s:object id="_43" s:type="_5" />
          <s:data id="_1" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="150" f:PreferredHeight="84" f:Anchor="BottomLeft" f:Padding="4;4;2;2" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:_1.BundleId="[KF]" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:Children="{Collection {ObjRef _2}, {ObjRef _5}, {ObjRef _6}, {ObjRef _7}}" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _10}" f:EditionPanel="{ObjRef _11}" f:SearchPanel="{ObjRef _29}" f:Text="" />
          <s:data id="_2" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="HorizontalFlow" f:PreferredWidth="80" f:Margins="0;0;0;0" f:Padding="0;0;0;0" f:Dock="StackBegin" f:Anchor="None" f:Children="{Collection {ObjRef _3}, {ObjRef _4}}" f:Text="" />
          <s:data id="_3" f:Value="{Binding Path=*.FirstName, Mode=OneWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:MinWidth="20" f:Margins="0;5;0;0" f:Dock="StackBegin" f:Anchor="None" f:ContainerLayoutMode="HorizontalFlow" f:PreferredWidth="0" />
          <s:data id="_4" f:Value="{Binding Path=*.LastName, Mode=OneWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:MinWidth="20" f:Margins="0;5;0;0" f:Dock="StackBegin" f:Anchor="None" f:ContainerLayoutMode="HorizontalFlow" f:PreferredWidth="40" f:TabIndex="1" />
          <s:data id="_5" f:Value="{Binding Path=*.Company, Mode=OneWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:Margins="0;0;0;0" f:Dock="StackBegin" f:Anchor="None" f:ContainerLayoutMode="HorizontalFlow" f:TabIndex="1" />
          <s:data id="_6" f:Value="{Binding Path=*.Address1, Mode=OneWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:Margins="0;0;0;0" f:Dock="StackBegin" f:Anchor="None" f:ContainerLayoutMode="HorizontalFlow" f:TabIndex="2" />
          <s:data id="_7" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="HorizontalFlow" f:PreferredWidth="80" f:Margins="0;0;0;0" f:Padding="0;0;0;0" f:Dock="StackBegin" f:Anchor="None" f:TabIndex="3" f:Children="{Collection {ObjRef _8}, {ObjRef _9}}" f:Text="" />
          <s:data id="_8" f:Value="{Binding Path=*.Zip, Mode=OneWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:Margins="0;5;0;0" f:Dock="StackBegin" f:Anchor="None" f:ContainerLayoutMode="HorizontalFlow" f:PreferredWidth="0" />
          <s:data id="_9" f:Value="{Binding Path=*.City, Mode=OneWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:Margins="0;0;0;0" f:Dock="StackBegin" f:Anchor="None" f:ContainerLayoutMode="HorizontalFlow" f:TabIndex="1" />
          <s:data id="_10" f:Fields="{Collection {Text '*;[KF0E]'}}" />
          <s:data id="_11" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Grid" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="314" f:PreferredHeight="108" f:Anchor="BottomLeft" f:Dock="None" f:Margins="0;0;0;0" f:Padding="10;10;10;10" f:_8.LayoutEngine="{ObjRef _12}" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:Children="{Collection {ObjRef _23}, {ObjRef _24}, {ObjRef _25}, {ObjRef _26}, {ObjRef _27}, {ObjRef _28}}" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _10}" f:EditionPanel="{ObjRef _11}" f:Owner="{ObjRef _1}" f:SearchPanel="{ObjRef _29}" f:Text="" />
          <s:data id="_12" f:ColumnDefinitions="{Collection {ObjRef _13}, {ObjRef _14}, {ObjRef _15}, {ObjRef _16}}" f:RowDefinitions="{Collection {ObjRef _17}, {ObjRef _18}, {ObjRef _19}, {ObjRef _20}, {ObjRef _21}, {ObjRef _22}}" />
          <s:data id="_13" f:Height="Auto" f:MinHeight="20" f:TopBorder="1" f:BottomBorder="1" />
          <s:data id="_14" f:Height="Auto" f:MinHeight="20" f:TopBorder="1" f:BottomBorder="1" />
          <s:data id="_15" f:Height="Auto" f:MinHeight="20" f:TopBorder="1" f:BottomBorder="1" />
          <s:data id="_16" f:Height="Auto" f:MinHeight="20" f:TopBorder="1" f:BottomBorder="1" />
          <s:data id="_17" f:Width="Auto" f:MinWidth="20" />
          <s:data id="_18" f:Width="*100" f:MinWidth="20" f:LeftBorder="0" />
          <s:data id="_19" f:Width="Auto" f:MinWidth="20" f:LeftBorder="4" />
          <s:data id="_20" f:Width="Auto" f:MinWidth="20" f:LeftBorder="0" />
          <s:data id="_21" f:Width="Auto" f:MinWidth="20" f:LeftBorder="0" />
          <s:data id="_22" f:Width="*100" f:MinWidth="20" />
          <s:data id="_23" f:Value="{Binding Path=*.FirstName, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:MinWidth="20" f:MinHeight="20" f:Anchor="None" f:Dock="None" f:_9.Column="0" f:_9.Row="0" f:ContainerLayoutMode="HorizontalFlow" f:_9.ColumnSpan="3" />
          <s:data id="_24" f:Value="{Binding Path=*.LastName, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:MinWidth="20" f:MinHeight="20" f:Anchor="None" f:Dock="None" f:_9.Column="3" f:_9.Row="0" f:ContainerLayoutMode="HorizontalFlow" f:_9.ColumnSpan="3" f:TabIndex="1" />
          <s:data id="_25" f:Value="{Binding Path=*.Company, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:MinWidth="20" f:MinHeight="20" f:Anchor="None" f:Dock="None" f:_9.Column="0" f:_9.Row="1" f:ContainerLayoutMode="HorizontalFlow" f:_9.ColumnSpan="6" f:TabIndex="2" />
          <s:data id="_26" f:Value="{Binding Path=*.Address1, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:MinWidth="20" f:MinHeight="20" f:Anchor="None" f:Dock="None" f:_9.Column="0" f:_9.Row="2" f:ContainerLayoutMode="HorizontalFlow" f:_9.ColumnSpan="6" f:TabIndex="3" />
          <s:data id="_27" f:Value="{Binding Path=*.Zip, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:MinWidth="20" f:MinHeight="20" f:Anchor="None" f:Dock="None" f:_9.Column="0" f:_9.Row="3" f:ContainerLayoutMode="HorizontalFlow" f:TabIndex="4" />
          <s:data id="_28" f:Value="{Binding Path=*.City, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:MinWidth="20" f:MinHeight="20" f:Anchor="None" f:Dock="None" f:_9.Column="2" f:_9.Row="3" f:ContainerLayoutMode="HorizontalFlow" f:_9.ColumnSpan="4" f:TabIndex="5" />
          <s:data id="_29" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="200" f:PreferredHeight="116" f:Anchor="BottomLeft" f:Dock="None" f:Margins="0;0;0;0" f:Padding="4;4;2;2" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:Children="{Collection {ObjRef _30}, {ObjRef _31}}" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _10}" f:EditionPanel="{ObjRef _11}" f:Owner="{ObjRef _1}" f:SearchPanel="{ObjRef _29}" f:Text="" />
          <s:data id="_30" f:MinWidth="10" f:TabNavigationMode="None" f:MinHeight="12" f:Margins="0;0;5;5" f:Dock="StackBegin" f:Anchor="None" f:PreferredHeight="12" f:CaptionId="[300LC]" />
          <s:data id="_31" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:ChildrenLayoutMode="Grid" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="200" f:Margins="0;0;5;5" f:PreferredHeight="80" f:Padding="0;0;0;0" f:DrawFullFrame="False" f:Dock="StackBegin" f:Anchor="None" f:_8.LayoutEngine="{ObjRef _32}" f:Children="{Collection {ObjRef _40}, {ObjRef _41}, {ObjRef _42}, {ObjRef _43}}" f:Text="" />
          <s:data id="_32" f:ColumnDefinitions="{Collection {ObjRef _33}, {ObjRef _34}, {ObjRef _35}, {ObjRef _36}}" f:RowDefinitions="{Collection {ObjRef _37}, {ObjRef _38}, {ObjRef _39}}" />
          <s:data id="_33" f:Height="Auto" f:MinHeight="20" />
          <s:data id="_34" f:Height="Auto" f:MinHeight="20" />
          <s:data id="_35" f:Height="Auto" f:MinHeight="20" />
          <s:data id="_36" f:Height="Auto" f:MinHeight="20" />
          <s:data id="_37" f:Width="Auto" f:MinWidth="20" />
          <s:data id="_38" f:Width="Auto" f:MinWidth="20" />
          <s:data id="_39" f:Width="*100" f:MinWidth="20" />
          <s:data id="_40" f:Value="{Binding Path=*.FirstName, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:Anchor="None" f:Dock="None" f:_9.Column="0" f:_9.Row="0" f:ContainerLayoutMode="HorizontalFlow" f:_9.ColumnSpan="3" />
          <s:data id="_41" f:Value="{Binding Path=*.LastName, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:Anchor="None" f:Dock="None" f:_9.Column="0" f:_9.Row="1" f:ContainerLayoutMode="HorizontalFlow" f:_9.ColumnSpan="3" />
          <s:data id="_42" f:Value="{Binding Path=*.Company, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:Anchor="None" f:Dock="None" f:_9.Column="0" f:_9.Row="2" f:ContainerLayoutMode="HorizontalFlow" f:_9.ColumnSpan="3" />
          <s:data id="_43" f:Value="{Binding Path=*.City, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:Anchor="None" f:Dock="None" f:_9.Column="0" f:_9.Row="3" f:ContainerLayoutMode="HorizontalFlow" f:_9.ColumnSpan="3" />
        </s:storage>
      </panel>
    </xml>
  </data>
  <data name="DefaultSize">150;84</data>
  <data name="RootEntityId"></data>
</bundle>