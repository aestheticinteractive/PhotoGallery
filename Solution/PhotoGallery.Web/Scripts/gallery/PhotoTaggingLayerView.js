/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function PhotoTaggingLayerView(pPhotoSet, pSelector, pSearchUrl, pAddUrl) {
	this.photoSet = pPhotoSet;
	this.selector = pSelector;
	this.tagSearchUrl = pSearchUrl;
	this.addUrl = pAddUrl;
}

/*--------------------------------------------------------------------------------------------*/
PhotoTaggingLayerView.prototype.initView = function() {
	var resizeClosure = function(pScope) {
		return function(pEvent) {
			pScope.onResize(pEvent);
		};
	};
	
	var keyClosure = function(pScope) {
		return function() {
			pScope.onSearchKeyup();
		};
	};
	
	var keyTimerClosure = function(pScope) {
		return function() {
			clearTimeout(pScope.timer);
			pScope.timer = setTimeout(keyClosure(pScope), 500);
		};
	};

	var clickClosure = function(pScope) {
		return function(pEvent) {
			pScope.onClick(pEvent);
		};
	};

	var cancelClosure = function(pScope) {
		return function() {
			pScope.onCancel(false);
		};
	};

	$(window).resize(resizeClosure(this));
	$(this.selector).keyup(keyTimerClosure(this));
	$(this.selector).click(null, clickClosure(this));
	$('#TagSearchCancel').click(null, cancelClosure(this));
	$('#TagModeBar').hide();
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.isVisible = function() {
	return $(this.selector).is(':visible');
};

/*--------------------------------------------------------------------------------------------*/
PhotoTaggingLayerView.prototype.toggleView = function() {
	$(this.selector).toggle();
	$('#PhotoViewBar').toggle();
	$('#TagModeBar').toggle();
	this.onCancel(true);
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
PhotoTaggingLayerView.prototype.onCancel = function(pNotFromClick) {
	this.cancelClick = !pNotFromClick;
	this.showSearch = false;

	$(this.selector+' .spot').hide();
	$(this.selector+' .inputPanel').hide();
	$('#TagSearch').val('');
	this.onSearchKeyup();
};

/*--------------------------------------------------------------------------------------------*/
PhotoTaggingLayerView.prototype.onClick = function(pEvent) {
	if ( this.showSearch || this.cancelClick ) {
		this.cancelClick = false;
		return;
	}

	$(this.selector+' .spot').show();
	$(this.selector+' .inputPanel').show();
	$('#TagSearch').focus();
	this.showSearch = true;

	var pho = $('#PhotoViewer .photo');
	var pos = pho.offset();
	var x = Math.max(pos.left, Math.min(pos.left+pho.width(), pEvent.pageX));
	var y = Math.max(pos.top, Math.min(pos.top+pho.height(), pEvent.pageY));
	this.spotRelX = (x-pos.left)/pho.width();
	this.spotRelY = (y-pos.top)/pho.height();

	this.onResize();
};

/*--------------------------------------------------------------------------------------------*/
PhotoTaggingLayerView.prototype.onResize = function() {
	if ( !this.showSearch ) {
		return;
	}

	var spot = $(this.selector+' .spot');
	var panel = $(this.selector+' .inputPanel');
	var pho = $('#PhotoViewer .photo');
	var pos = pho.offset();
	var winW = $(window).width();
	var winH = $(window).height();
	var x = pos.left+pho.width()*this.spotRelX;
	var y = pos.top+pho.height()*this.spotRelY;
	var boxW = 300;
	var boxH = 300;
	var boxX = x;
	var boxY = y+40;
	var pad = 10;

	if ( boxX-boxW/2-pad < 0 ) {
		boxX = boxW/2+pad;
	}
	else if ( boxX+boxW/2+pad > winW ) {
		boxX = winW-boxW/2-pad;
	}

	if ( boxY+boxH+pad > winH ) {
		boxY = winH-boxH-pad;
	}

	spot.css('left', x+'px').css('top', y+'px');
	panel.css('left', boxX+'px').css('top', boxY+'px');
};
