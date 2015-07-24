var me;
var formatedId;
var possisanLeft;
var possisanTop;
var newKay = "";
var meanuWidth;
var styles;
var spanStyles;
var substr;

jQuery('#ContentTreeInnerPanel').on('mouseenter', '.scContentTreeNode', function (event) {
	me = jQuery(this);
	formatedId = me.children().last().attr('id').substr(10).split("");
	possisanLef = me.position().left;
	possisanTop = me.position().top + 150;
	menanuWidth = me.width();

	getItemData(formatedId, possisanLef, possisanTop, menanuWidth);
}).on('mouseleave', '.scContentTreeNode', function (event) {
	newKay = "";

	jQuery(".tooltipInfo").remove();
});


function getItemData(itemId, left, top, width) {
	for (var i = 0; i < itemId.length; i++) {
		newKay += itemId[i];
		if (i == 7) { newKay += '-'; }
		if (i == 11) { newKay += '-'; }
		if (i == 15) { newKay += '-'; }
		if (i == 19) { newKay += '-'; }
	}

	// Setting multiple properties.
	left = left + width;
	styles = "top: " + top + "px;left: " + left + "px; padding: 10px; border: solid 1px #474747; border-radius: 10px; z-index: 999; width: auto !important; position: absolute; background: #F0F0F0;"
	spanStyles = "position: relative; margin-bottom: 4px; padding: 2px 0; display:block;";

	jQuery.get("http://chg.local/sitecore/api/ssc/item/" + newKay + "", function (data) {
		jQuery(".scFlexColumnContainer", parent.document.body).append("<div style='" + styles + "' class='tooltipInfo'><span style='" + spanStyles + "'><strong>Item ID : </strong>" + data.ItemID + "</span><span style='" + spanStyles + "'><strong>Item Name :  </strong>" + data.ItemName + "</span><span style='" + spanStyles + "'><strong>Item Path :  </strong>" + data.ItemPath + "</span><span style='spanStyles'> <strong>Template Name :  </strong>" + data.TemplateName + "</span><span style='" + spanStyles + "'> <strong>Template ID :  </strong>" + data.TemplateID + "</span></div>").show();
	});
}