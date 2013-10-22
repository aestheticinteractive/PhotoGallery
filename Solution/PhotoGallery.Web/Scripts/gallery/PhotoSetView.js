/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
function PhotoSetView(pPhotoSet, pSelector) {
	this.photoSet = pPhotoSet;
	this.selector = pSelector;
	this.photoSet.events.listen("filterChange", this, this.updateView);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetView.prototype.updateView = function() {
	//var hold = $(this.selector);
	$(this.selector+" .border").hide();

	if ( !isTouch() ) {
		$(this.selector+" .thumb")
			.mouseenter(function() {
				$(this).find(".border").show();
			})
			.mouseleave(function() {
				$(this).find(".border").hide();
			});
	}
};
