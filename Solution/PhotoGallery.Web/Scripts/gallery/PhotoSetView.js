/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
function PhotoSetView(pPhotoSet, pSelector) {
	this.photoSet = pPhotoSet;
	this.selector = pSelector;
	this.photoSet.events.listen("filterChanged", this, this.updateView);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetView.prototype.initView = function() {
	$(this.selector+" .border").hide();

	if ( isTouch() ) {
		return;
	}

	var onClickClosure = function(pScope) {
		return function() {
			var id = $(this).attr('data-id');
			pScope.showPhoto(id);
		};
	};

	$(this.selector+" .thumb")
		.click(onClickClosure(this.photoSet))
		.mouseenter(function() {
			$(this).find(".border").show();
		})
		.mouseleave(function() {
			$(this).find(".border").hide();
		});
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetView.prototype.updateView = function() {
	var ps = this.photoSet;

	$(this.selector+" .thumb").each(function() {
		var id = $(this).attr('data-id');
		
		if ( ps.isPhotoAvailable(id) ) {
			$(this).show();
		}
		else {
			$(this).hide();
		}
	});
};
