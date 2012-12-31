Ext.require([
  'Epsitec.cresus.webcore.entityUi.SetColumn',
  'Epsitec.cresus.webcore.entityUi.TileColumn',
  'Epsitec.cresus.webcore.field.EntityCollectionField',
  'Epsitec.cresus.webcore.field.EntityReferenceField',
  'Epsitec.cresus.webcore.field.EnumerationField',
  'Epsitec.cresus.webcore.tile.CollectionSummaryTile',
  'Epsitec.cresus.webcore.tile.EditionTile',
  'Epsitec.cresus.webcore.tile.EmptySummaryTile',
  'Epsitec.cresus.webcore.tile.GroupedSummaryTile',
  'Epsitec.cresus.webcore.tile.GroupedSummaryTileItem',
  'Epsitec.cresus.webcore.tile.SummaryTile',
  'Epsitec.cresus.webcore.tools.Texts'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityUi.BrickWallParser', {
    alternateClassName: ['Epsitec.BrickWallParser'],

    statics: {
      parseColumn: function(column) {
        switch (column.type) {
          case 'set':
            return this.parseSetColumn(column);

          case 'tile':
            return this.parseTileColumn(column);

          default:
            throw 'invalid column type: ' + column.type;
        }
      },

      parseSetColumn: function(column) {
        var c = this.parseEntityColumn(column);
        c.typeName = 'Epsitec.SetColumn';
        c.title = column.title;
        c.iconCls = column.icon;
        c.displayDatabase = column.displayDatabase;
        c.pickDatabase = column.pickDatabase;
        return c;
      },

      parseTileColumn: function(column) {
        var c = this.parseEntityColumn(column);
        c.typeName = 'Epsitec.TileColumn';
        c.items = this.parseTiles(column.tiles);
        return c;
      },

      parseEntityColumn: function(column) {
        return {
          entityId: column.entityId,
          viewMode: column.viewMode,
          viewId: column.viewId
        };
      },

      parseTiles: function(tiles) {
        return tiles.map(this.parseTile, this);
      },

      parseTile: function(tile) {
        switch (tile.type) {
          case 'summary':
            return this.parseSummaryTile(tile);

          case 'groupedSummary':
            return this.parseGroupedSummaryTile(tile);

          case 'collectionSummary':
            return this.parseCollectionSummaryTile(tile);

          case 'emptySummary':
            return this.parseEmptySummaryTile(tile);

          case 'edition':
            return this.parseEditionTile(tile);

          case 'action':
            return this.parseActionTile(tile);

          default:
            throw 'invalid tile type: ' + tile.type;
        }
      },

      parseSummaryTile: function(tile) {
        var t = this.parseBaseActionTile(tile);
        t.xtype = 'epsitec.summarytile';
        t.html = tile.text;
        t.isRoot = tile.isRoot;
        t.subViewMode = tile.subViewMode;
        t.subViewId = tile.subViewId;
        t.autoCreatorId = tile.autoCreatorId;
        return t;
      },

      parseGroupedSummaryTile: function(tile) {
        return {
          xtype: 'epsitec.groupedsummarytile',
          title: tile.title,
          iconCls: tile.icon,
          subViewMode: tile.subViewMode,
          subViewId: tile.subViewId,
          hideRemoveButton: tile.hideRemoveButton,
          hideAddButton: tile.hideAddButton,
          propertyAccessorId: tile.propertyAccessorId,
          items: this.parseGroupedItems(tile.items)
        };
      },

      parseGroupedItems: function(items) {
        return items.map(this.parseGroupedItem, this);
      },

      parseGroupedItem: function(item) {
        return {
          xtype: 'epsitec.groupedsummarytileitem',
          entityId: item.entityId,
          html: item.text
        };
      },

      parseCollectionSummaryTile: function(tile) {
        var t = this.parseSummaryTile(tile);
        t.xtype = 'epsitec.collectionsummarytile';
        t.hideRemoveButton = tile.hideRemoveButton;
        t.hideAddButton = tile.hideAddButton;
        t.propertyAccessorId = tile.propertyAccessorId;
        return t;
      },

      parseEmptySummaryTile: function(tile) {
        var t = this.parseBaseTile(tile);
        t.xtype = 'epsitec.emptysummarytile';
        t.propertyAccessorId = tile.propertyAccessorId;
        return t;
      },

      parseEditionTile: function(tile) {
        var t = this.parseBaseActionTile(tile);
        t.xtype = 'epsitec.editiontile';
        t.items = this.parseBricks(tile.bricks);
        return t;
      },

      parseActionTile: function(tile) {
        var t = this.parseBaseTile(tile);
        t.text = tile.text;
        t.fields = this.parseBricks(tile.fields);
        return t;
      },

      parseBaseActionTile: function(tile) {
        var t = this.parseBaseTile(tile);
        t.actions = tile.actions;
        return t;
      },

      parseBaseTile: function(tile) {
        return {
          title: tile.title,
          iconCls: tile.icon,
          entityId: tile.entityId
        };
      },

      parseBricks: function(bricks) {
        return bricks.map(this.parseBrick, this);
      },

      parseBrick: function(brick) {
        switch (brick.type) {
          case 'booleanField':
            return this.parseBooleanField(brick);

          case 'dateField':
            return this.parseDateField(brick);

          case 'decimalField':
            return this.parseDecimalField(brick);

          case 'entityCollectionField':
            return this.parseEntityCollectionField(brick);

          case 'entityReferenceField':
            return this.parseEntityReferenceField(brick);

          case 'enumerationField':
            return this.parseEnumerationField(brick);

          case 'globalWarning':
            return this.parseGlobalWarning(brick);

          case 'horizontalGroup':
            return this.parseHorizontalGroup(brick);

          case 'integerField':
            return this.parseIntegerField(brick);

          case 'separator':
            return this.parseSeparator(brick);

          case 'specialField':
            return this.parseSpecialField(brick);

          case 'textAreaField':
            return this.parseTextAreaField(brick);

          case 'textField':
            return this.parseTextField(brick);

          default:
            throw 'invalid brick type: ' + brick.type;
        }
      },

      parseField: function(brick) {
        return {
          fieldLabel: brick.title,
          name: brick.name,
          value: brick.value,
          readOnly: brick.readOnly,
          labelSeparator: null,
          allowBlank: brick.allowBlank
        };
      },

      parseBooleanField: function(brick) {
        var field = this.parseField(brick);

        field.xtype = 'checkboxfield';
        field.inputValue = true;
        field.uncheckedValue = false;

        field.checked = field.value;
        delete field.value;

        // This is the way of setting the label to the side of the checkbox and
        // to align the checkbox on the right side of the form. It's kind of
        // ugly but extjs doesn't allow us to do better here, such has having an
        // expandable label.
        field.labelAlign = 'left';
        field.labelWidth = 320;

        return field;
      },

      parseDateField: function(brick) {
        var field = this.parseField(brick);
        field.xtype = 'datefield';
        field.format = 'd.m.Y';
        return field;
      },

      parseDecimalField: function(brick) {
        var field = this.parseField(brick);
        field.xtype = 'numberfield';
        return field;
      },

      parseEntityCollectionField: function(brick) {
        var field = this.parseField(brick);
        field.xtype = 'epsitec.entitycollectionfield';
        field.databaseName = brick.databaseName;
        return field;
      },

      parseEntityReferenceField: function(brick) {
        var field = this.parseField(brick);
        field.xtype = 'epsitec.entityreferencefield';
        field.databaseName = brick.databaseName;
        return field;
      },

      parseEnumerationField: function(brick) {
        var field = this.parseField(brick);
        field.xtype = 'epsitec.enumerationfield';
        field.enumerationName = brick.enumerationName;

        return field;
      },

      parseGlobalWarning: function(brick) {
        return {
          xtype: 'displayfield',
          value: Epsitec.Texts.getGlobalWarning(),
          cls: 'global-warning'
        };
      },

      parseHorizontalGroup: function(brick) {
        var group, columnWidth;

        group = {
          xtype: 'fieldset',
          layout: 'column',
          padding: '0 5 5 5',
          title: brick.title,
          items: this.parseBricks(brick.bricks)
        };

        columnWidth = 1 / group.items.length;

        Ext.Array.forEach(group.items, function(b, i) {
          b.columnWidth = columnWidth;
          if (i < group.items.length - 1) {
            b.margin = '0 5 0 0';
          }
          delete b.fieldLabel;
        });

        return group;
      },

      parseIntegerField: function(brick) {
        var field = this.parseField(brick);
        field.xtype = 'numberfield';
        field.allowDecimals = false;
        return field;
      },

      parseSeparator: function(brick) {
        return {
          xtype: 'box',
          border: true,
          autoEl: {
            tag: 'hr'
          },
          style: {
            borderColor: 'grey',
            borderStyle: 'solid'
          }
        };
      },

      parseSpecialField: function(brick) {
        var field = {
          xtype: brick.fieldName,
          entityId: brick.entityId,
          controllerName: brick.controllerName,
          fieldConfig: this.parseField(brick)
        };

        field.fieldLabel = field.fieldConfig.fieldLabel;
        delete field.fieldConfig.fieldLabel;

        field.labelSeparator = field.fieldConfig.labelSeparator;
        delete field.fieldConfig.labelSeparator;

        return field;
      },

      parseTextAreaField: function(brick) {
        var field = this.parseField(brick);
        field.xtype = 'textareafield';
        return field;
      },

      parseTextField: function(brick) {
        var field = this.parseField(brick);
        field.xtype = 'textfield';

        if (brick.isPassword === true) {
          field.inputType = 'password';
        }

        return field;
      }
    }
  });
});
