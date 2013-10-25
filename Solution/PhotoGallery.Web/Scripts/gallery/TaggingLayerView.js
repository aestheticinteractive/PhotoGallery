/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function TaggingLayerView(pPhotoSet, pSelector, pPhotoSelector, pSearchUrl, pAddUrl) {
	this.photoSet = pPhotoSet;
	this.selector = pSelector;
	this.photoSelector = pPhotoSelector;
	this.tagSearchUrl = pSearchUrl;
	this.addUrl = pAddUrl;
}

/*--------------------------------------------------------------------------------------------*/
TaggingLayerView.prototype.buildView = function() {
	var resizeClosure = function(pScope) {
		return function(pEvent) {
			pScope.onResize(pEvent);
		};
	};
	
	var keyClosure = function(pScope) {
		return function(pEvent) {
			pScope.onKeyup(pEvent);
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

	var clicker = $('<div>')
		.attr('class', 'clicker')
		.click(clickClosure(this));

	this.spot = $('<div>')
		.attr('class', 'spot')
		.append($('<div>')
			.attr('class', 'spotInner')
		);
		
	this.closeBtn = $('<a>')
		.attr('class', 'closeBtn')
		.html('Done')
		.click(cancelClosure(this));

	this.searchDiv = $('<div>')
		.attr('class', 'liveSearch');

	this.layer = $(this.selector)
		.append(clicker)
		.append(this.spot)
		.append(this.closeBtn)
		.append(this.searchDiv);

	var lst = new LiveSearchTags(this.tagSearchUrl);
	lst.events.listen('closed', this, this.onSearchClose);

	this.searchView = new LiveSearchView(lst, this.selector+' .liveSearch');
	this.searchView.buildView('Enter tag text...', true);

	$(window).resize(resizeClosure(this));
	$(document).keyup(keyClosure(this));
};

/*--------------------------------------------------------------------------------------------*/
TaggingLayerView.prototype.isVisible = function() {
	return this.layer.is(':visible');
};

/*--------------------------------------------------------------------------------------------*/
TaggingLayerView.prototype.toggleView = function() {
	this.layer.toggle();
	this.onCancel(true);
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
TaggingLayerView.prototype.onCancel = function(pNotFromClick) {
	this.cancelClick = !pNotFromClick;
	this.spot.hide();
	this.searchView.hide();
};

/*--------------------------------------------------------------------------------------------*/
TaggingLayerView.prototype.onKeyup = function(pEvent) {
	if ( !this.isVisible() ) {
		return;
	}

	if ( pEvent.which == ESCAPE_KEY ) {
		if ( this.searchView.isVisible() ) {
			this.searchView.onClose();
		}
		else {
			this.toggleView();
		}
	}
};

/*--------------------------------------------------------------------------------------------*/
TaggingLayerView.prototype.onClick = function(pEvent) {
	if ( this.searchView.isVisible() || this.cancelClick ) {
		this.cancelClick = false;
		return;
	}

	var pho = $(this.photoSelector);
	var pos = pho.offset();
	var x = Math.max(pos.left, Math.min(pos.left+pho.width(), pEvent.pageX));
	var y = Math.max(pos.top, Math.min(pos.top+pho.height(), pEvent.pageY));
	this.spotRelX = (x-pos.left)/pho.width();
	this.spotRelY = (y-pos.top)/pho.height();
	
	this.spot.show();
	this.searchView.show();
	this.closeBtn.hide();
	this.onResize();
};

/*--------------------------------------------------------------------------------------------*/
TaggingLayerView.prototype.onSearchClose = function() {
	this.spot.hide();
	this.closeBtn.show();
};

/*--------------------------------------------------------------------------------------------*/
TaggingLayerView.prototype.onResize = function() {
	if ( !this.searchView.isVisible() ) {
		return;
	}

	var pho = $(this.photoSelector);
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

	this.spot
		.css('left', x+'px')
		.css('top', y+'px');

	this.searchDiv
		.css('left', boxX+'px')
		.css('top', boxY+'px');
};
