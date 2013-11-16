/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function TaggingLayerView(pTaggingLayer, pSelector, pPhotoSelector) {
	this.taggingLayer = pTaggingLayer;
	this.selector = pSelector;
	this.photoSelector = pPhotoSelector;

	this.taggingLayer.events.listen('addTagFailed', this, this.onAddTagFail);
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

	var closeClosure = function(pScope) {
		return function() {
			pScope.hide();
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
		.click(closeClosure(this));

	this.searchDiv = $('<div>')
		.attr('class', 'liveSearch');

	this.layer = $(this.selector)
		.append(clicker)
		.append(this.spot)
		.append(this.closeBtn)
		.append(this.searchDiv);

	var ls = this.taggingLayer.liveSearch;
	ls.events.listen('itemSelected', this, this.onSearchSelect);
	ls.events.listen('closed', this, this.onSearchClose);

	this.searchView = new LiveSearchView(ls, this.selector+' .liveSearch');
	this.searchView.buildView('Enter tag text...', true);

	$(window).resize(resizeClosure(this));
	$(document).keyup(keyClosure(this));
};

/*--------------------------------------------------------------------------------------------*/
TaggingLayerView.prototype.isVisible = function() {
	return this.layer.is(':visible');
};

/*--------------------------------------------------------------------------------------------*/
TaggingLayerView.prototype.show = function() {
	this.layer.show();
	this.onCancel(true);
};

/*--------------------------------------------------------------------------------------------*/
TaggingLayerView.prototype.hide = function() {
	this.layer.hide();
	this.taggingLayer.onClose();
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

	if ( this.searchView.isVisible() ) {
		if ( pEvent.which == ESCAPE_KEY ) {
			this.searchView.onClose();
		}

		return;
	}
	
	switch ( pEvent.which ) {
		case ESCAPE_KEY:
			this.hide();
			break;

		case LEFT_ARROW:
			this.taggingLayer.onPrev();
			break;

		case RIGHT_ARROW:
			this.taggingLayer.onNext();
			break;
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
	var sx = (x-pos.left)/pho.width();
	var sy = (y-pos.top)/pho.height();
	this.taggingLayer.setSpotPos(sx, sy);

	console.log("XY: "+x+', '+y+' / '+sx+', '+sy+' / '+pos.left+', '+pos.top);
	console.log(" *** "+pho.css('margin-top')+', '+pho.css('padding-top')+', '+pho.css('border-top'));
	
	//good2 --- XY: 587, 112 / 0.3982725527831094, 0.3227665706051873 / 379.5, 0
	//bad22 --- XY: 587, 180 / 0.3982725527831094, 0.31988472622478387 / 379.5, 69


	//bad2  --- XY: 610, 228 / 0.44241842610364684, 0.45821325648414984 / 379.5, 69

	this.spot.show();
	this.searchView.show();
	this.closeBtn.hide();
	this.onResize();
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
	var sp = this.taggingLayer.getSpotPos();
	var x = pos.left+pho.width()*sp.x;
	var y = pos.top+pho.height()*sp.y;
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

/*--------------------------------------------------------------------------------------------*/
TaggingLayerView.prototype.onSearchSelect = function() {
	this.searchView.onClose();
};

/*--------------------------------------------------------------------------------------------*/
TaggingLayerView.prototype.onSearchClose = function() {
	this.spot.hide();
	this.closeBtn.show();
};

/*--------------------------------------------------------------------------------------------*/
TaggingLayerView.prototype.onAddTagFail = function() {
	alert('Your attempt to tag this photo failed. Please make sure you are logged into Fabric.');
};
