// This class is a column that can be used in grid panels to show nullable
// boolean values with proper localization for null values.

Ext.require([
  'Epsitec.cresus.webcore.tools.Texts'
],
function() {
  Ext.define('Epsitec.cresus.webcore.tools.BooleanNullableColumn', {
    extend: 'Ext.grid.column.Boolean',
    alias: ['widget.booleannullablecolumn'],
    alternateClassName: ['Epsitec.BooleanNullableColumn'],

    /* Configuration */

    nullText: Epsitec.Texts.getNullItemText(),

    /* Methods */

    defaultRenderer: function(value) {
      if (value === null) {
        return this.nullText;
      }
      return this.callParent(arguments);
    }
  });
});
