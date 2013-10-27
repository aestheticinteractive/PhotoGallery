/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function PhotoLayerView(pPhotoLayer, pSiteSelector) {
	this.photoLayer = pPhotoLayer;
	this.photoSet = this.photoLayer.photoSet;
	this.photoSet.events.listen("photoChanged", this, this.onPhoto);
	this.siteSelector = pSiteSelector;
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
	
	var tagClosure = function(pScope) {
		return function() {
			pScope.showTagLayer();
		};
	};
	
	var detailClosure = function(pScope) {
		return function() {
			pScope.toggleDetailPanel();
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
		.click(navClosure(this, 1));

	this.details = $('<p>')
		.attr('class', 'details')
		.click(detailClosure(this));

	this.tagModeBtn = $('<a>')
		.attr('class', 'tagModeBtn')
		.attr('title', 'Enter Tag Mode')
		.click(tagClosure(this));

	this.prevBtn = $('<a>')
		.attr('class', 'prevBtn')
		.attr('title', 'Previous Photo')
		.click(navClosure(this, -1));

	this.nextBtn = $('<a>')
		.attr('class', 'nextBtn')
		.attr('title', 'Next Photo')
		.click(navClosure(this, 1));

	this.closeBtn = $('<a>')
		.attr('class', 'closeBtn')
		.attr('title', 'Close Photo')
		.click(navClosure(this, 0));

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

	this.bar = $('<div>')
		.attr('class', 'bar')
		.append(this.details)
		.append(this.buttons);

	this.layer
		.append(this.photo)
		.append(this.bar);

	$('body')
		.prepend(this.layerPad)
		.prepend(this.layer);

	///

	var tagsDiv = $('<div>').attr('class', 'tagsPanel');
	this.layer.append(tagsDiv);

	this.tagsView = new PhotoLayerTagsView(this.photoLayer.tags, '#PhotoLayer .tagsPanel');
	this.tagsView.buildView();
	this.tagsView.hide();

	///

	var taggingDiv = $('<div>').attr('id', 'TaggingLayer');
	this.layer.append(taggingDiv);

	this.photoLayer.tagLayer.events.listen('closed', this, this.onTagLayerClose);

	this.tagLayerView = new TaggingLayerView(this.photoLayer.tagLayer,
		'#TaggingLayer', '#PhotoLayer .photo');
	this.tagLayerView.buildView();
	this.tagLayerView.hide();
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
	if ( !this.isVisible() || this.tagLayerView.isVisible() ) {
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
	if ( !this.isVisible() || this.tagLayerView.isVisible() ) {
		return;
	}

	this.buttons.find('a').mouseleave();

	switch ( pEvent.which ) {
		case T_KEY:
			this.showTagLayer();
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

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.showTagLayer = function() {
	this.tagLayerView.show();
	this.bar.hide();
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.onTagLayerClose = function() {
	this.bar.show();
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.toggleDetailPanel = function() {
	if ( this.tagsView.isVisible() ) {
		this.tagsView.hide();
	}
	else {
		this.tagsView.show();
		this.tagsView.update();
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
	this.tagsView.update();
	this.show();
	this.onResize();
	this.photoLayer.tagLayer.setPhotoId(phoData.photoId);
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.onResize = function() {
	var pho = this.photoSet.getCurrentData();

	if ( !pho ) {
		return;
	}

	var w = this.layer.width();
	var h = this.layer.height();
	var phoRatio = pho.ratio;

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
