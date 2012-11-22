Ext.require([
  'Epsitec.cresus.webcore.CollectionSummaryTile',
  'Epsitec.cresus.webcore.EditionTile',
  'Epsitec.cresus.webcore.EmptySummaryTile',
  'Epsitec.cresus.webcore.EntityCollectionField',
  'Epsitec.cresus.webcore.EntityReferenceField',
  'Epsitec.cresus.webcore.EnumerationField',
  'Epsitec.cresus.webcore.SummaryTile',
  'Epsitec.cresus.webcore.Texts'
],
function() {
  Ext.define('Epsitec.cresus.webcore.BrickWallParser', {
    alternateClassName: ['Epsitec.BrickWallParser'],

    statics: {
      parseEntityColumn: function(column) {
        return {
          entityId: column.entityId,
          viewMode: column.viewMode,
          viewId: column.viewId,
          items: this.parseTiles(column.tiles)
        };
      },

      parseTiles: function(tiles) {
        return tiles.map(this.parseTile, this);
      },

      parseTile: function(tile) {
        switch (tile.type) {
          case 'summary':
            return this.parseSummaryTile(tile);

          case 'collectionSummary':
            return this.parseCollectionSummaryTile(tile);

          case 'emptySummary':
            return this.parseEmptySummaryTile(tile);

          case 'edition':
            return this.parseEditionTile(tile);

          default:
            throw 'invalid tile type: ' + tile.type;
        }
      },

      parseSummaryTile: function(tile) {
        var t = this.parseActionTile(tile);
        t.xtype = 'epsitec.summarytile';
        t.html = tile.text;
        t.isRoot = tile.isRoot;
        t.subViewMode = tile.subViewMode;
        t.subViewId = tile.subViewId;
        t.autoCreatorId = tile.autoCreatorId;
        return t;
      },

      parseCollectionSummaryTile: function(tile) {
        var t = this.parseSummaryTile(tile);
        t.xtype = 'epsitec.collectionsummarytile';
        t.hideRemoveButton = tile.hideRemoveButton;
        t.hideAddButton = tile.hideAddButton;
        t.propertyAccessorId = tile.propertyAccessorId;
        t.entityType = tile.entityType;
        return t;
      },

      parseEmptySummaryTile: function(tile) {
        return {
          xtype: 'epsitec.emptysummarytile',
          propertyAccessorId: tile.propertyAccessorId,
          entityType: tile.entityType
        };
      },

      parseEditionTile: function(tile) {
        var t = this.parseBaseTile(tile);
        t.xtype = 'epsitec.editiontile';
        t.items = this.parseBricks(tile.bricks);
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
          readOnly: brick.readOnly,
          labelSeparator: null,
          allowBlank: brick.allowBlank
        };
      },

      parseBooleanField: function(brick) {
        var field = this.parseField(brick);

        field.xtype = 'checkboxfield';
        field.checked = brick.value;
        field.inputValue = true;
        field.uncheckedValue = false;

        return field;
      },

      parseDateField: function(brick) {
        var field = this.parseField(brick);

        field.xtype = 'datefield';
        field.format = 'd.m.Y';
        field.value = brick.value;

        return field;
      },

      parseDecimalField: function(brick) {
        var field = this.parseField(brick);

        field.xtype = 'numberfield';
        field.value = brick.value;

        return field;
      },

      parseEntityCollectionField: function(brick) {
        var field = this.parseField(brick);

        field.xtype = 'epsitec.entitycollectionfield';
        field.values = brick.value;
        field.entityName = brick.entityName;

        return field;
      },

      parseEntityReferenceField: function(brick) {
        var field = this.parseField(brick);

        field.xtype = 'epsitec.entityreferencefield';
        field.value = brick.value;
        field.entityName = brick.entityName;

        return field;
      },

      parseEnumerationField: function(brick) {
        var field = this.parseField(brick);

        field.xtype = 'epsitec.enumerationfield';
        field.value = brick.value;
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
        field.value = brick.value;

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

      parseTextAreaField: function(brick) {
        var field = this.parseField(brick);

        field.xtype = 'textareafield';
        field.value = brick.value;

        return field;
      },

      parseTextField: function(brick) {
        var field = this.parseField(brick);

        field.xtype = 'textfield';
        field.value = brick.value;

        if (brick.isPassword === true) {
          field.inputType = 'password';
        }

        return field;
      }
    }
  });
});
