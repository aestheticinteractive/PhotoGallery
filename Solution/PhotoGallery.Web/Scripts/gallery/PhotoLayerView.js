/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function PhotoLayerView(pPhotoSet, pSiteSelector) {
	this.photoSet = pPhotoSet;
	this.siteSelector = pSiteSelector;
	this.photoSet.events.listen("photoChanged", this, this.onPhoto);
}

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.buildView = function() {
	var keyClosure = function(pScope, pUp) {
		return function(pEvent) {
			if ( pUp ) {
				pScope.onKeyUp(pEvent);
			}
			else {
				pScope.onKeyDown(pEvent);
			}
		};
	};
	
	var resizeClosure = function(pScope) {
		return function(pEvent) {
			pScope.onResize(pEvent);
		};
	};
	
	var navClosure = function(pScope, pDir) {
		return function() {
			if ( pDir == 1 ) {
				pScope.photoSet.showNextPhoto();
			}
			else if ( pDir == -1 ) {
				pScope.photoSet.showPrevPhoto();
			}
			else {
				pScope.photoSet.hideCurrentPhoto();
			}
		};
	};

	$(document)
		.keydown(keyClosure(this, false))
		.keyup(keyClosure(this, true));

	$(window).resize(resizeClosure(this));

	this.origBg = $('body').css('background-color');
	this.origOy = $('body').css('overflow-y');

	////

	this.layerPad = $('<div>')
		.attr('id', 'PhotoLayerPadding');

	this.layer = $('<div>')
		.attr('id', 'PhotoLayer');

	this.photo = $('<img>')
		.attr('class', 'photo')
		.click(null, navClosure(this, 1));

	this.details = $('<p>')
		.attr('class', 'details');

	this.tagModeBtn = $('<a>')
		.attr('class', 'tagModeBtn')
		.attr('title', 'Enter Tag Mode');

	this.prevBtn = $('<a>')
		.attr('class', 'prevBtn')
		.attr('title', 'Previous Photo')
		.click(null, navClosure(this, -1));

	this.nextBtn = $('<a>')
		.attr('class', 'nextBtn')
		.attr('title', 'Next Photo')
		.click(null, navClosure(this, 1));

	this.closeBtn = $('<a>')
		.attr('class', 'closeBtn')
		.attr('title', 'Close Photo')
		.click(null, navClosure(this, 0));

	this.buttons = $('<div>')
		.attr('class', 'buttons')
		.append(this.tagModeBtn)
		.append(this.prevBtn)
		.append(this.nextBtn)
		.append(this.closeBtn);
		
	if ( !isTouch() ) {
		this.buttons.find('a')
			.fadeTo(0, 0.4)
			.mouseenter(function() { $(this).fadeTo(0, 1.0); })
			.mouseleave(function() { $(this).fadeTo(0, 0.4); });
	}

	this.layer
		.append(this.photo)
		.append($('<div>')
			.attr('class', 'bar')
			.append(this.details)
			.append(this.buttons)
		);

	$('body')
		.prepend(this.layerPad)
		.prepend(this.layer);
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.isVisible = function() {
	return this.layer.is(':visible');
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.show = function() {
	this.layer.show();
	this.layerPad.show();
	$(this.siteSelector).hide();
	
	$('body')
		.css('background-color', '#000')
		.css('overflow-y', 'hidden');
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.hide = function() {
	this.layer.hide();
	this.layerPad.hide();
	$(this.siteSelector).show();

	$('body')
		.css('background-color', this.origBg)
		.css('overflow-y', this.origOy);
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.onKeyDown = function(pEvent) {
	if ( !this.isVisible() ) {
		return;
	}

	switch ( pEvent.which ) {
		case T_KEY: 
			this.tagModeBtn.mouseenter();
			break;

		case LEFT_ARROW:
			this.prevBtn.mouseenter();
			break;

		case RIGHT_ARROW:
			this.nextBtn.mouseenter();
			break;
			
		case ESCAPE_KEY:
			this.closeBtn.mouseenter();
			break;
	}
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.onKeyUp = function(pEvent) {
	if ( !this.isVisible() ) {
		return;
	}

	this.buttons.find('a').mouseleave();

	switch ( pEvent.which ) {
		case T_KEY:
			break;

		case LEFT_ARROW:
			this.photoSet.showPrevPhoto();
			break;

		case RIGHT_ARROW:
			this.photoSet.showNextPhoto();
			break;
			
		case ESCAPE_KEY:
			this.photoSet.hideCurrentPhoto();
			break;
	}
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.onPhoto = function() {
	var phoData = this.photoSet.getCurrentData();

	if ( !phoData ) {
		this.hide();
		return;
	}

	this.photo.attr('src', phoData.url);
	this.details.html(phoData.created.replace(' ', '<br/>'));
	this.onResize();
	this.show();
	this.preloadNextImage();

	trackPageview('/Photos/'+phoData.photoId);
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.onResize = function() {
	var w = this.layer.width();
	var h = this.layer.height();
	var phoRatio = this.photoSet.getCurrentData().ratio;

	if ( w/h > phoRatio ) {
		var imgW = phoRatio*h;

		this.photo
			.css('height', '100%')
			.css('width', 'auto')
			.css('margin', '0 0 0 '+(w-imgW)/2+'px');
	}
	else {
		var imgH = w/phoRatio;

		this.photo
			.css('width', '100%')
			.css('height', 'auto')
			.css('margin', (h-imgH)/2+'px 0 0 0');
	}
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.preloadNextImage = function() {
	var id = this.photoSet.getNextPhotoId();

	var img = new Image();
	img.src = this.photoSet.getData(id).url;
};
