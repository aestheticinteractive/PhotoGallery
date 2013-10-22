/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />

var ENTER = 13;
var ESCAPE_KEY = 27;
var LEFT_ARROW = 37;
var UP_ARROW = 38;
var RIGHT_ARROW = 39;
var DOWN_ARROW = 40;
var T_KEY = 84;


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function PhotoLayerView(pPhotoSet, pSelector) {
	this.photoSet = pPhotoSet;
	this.selector = pSelector;
	this.photoSet.events.listen("photoChanged", this, this.onPhoto);
}

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.initView = function() {
	$(document).keyup(this.onKeyUp);
	$(window).resize(this.onResize);

	$('body').prepend($(this.selector+'Padding')).prepend($(this.selector));
	this.origBg = $('body').css('background-color');
	this.origOy = $('body').css('overflow-y');

	//$(this.selector+' .photo').click(null, nextPhoto);
	this.close();
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.isLayerVisible = function() {
	return $(this.selector).is(':visible');
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.onKeyUp = function(pEvent) {
	if ( !this.isLayerVisible() ) {
		return;
	}

	var key = pEvent.which;

	switch ( key ) {
		//case T_KEY: toggleTagMode(); break;
		
		case ESCAPE_KEY:
			this.closePhoto();
			break;

		case LEFT_ARROW:
			this.photoSet.showPrevPhoto();
			break;

		case RIGHT_ARROW:
			this.photoSet.showNextPhoto();
			break;
	}
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.onPhoto = function() {
	var phoData = this.photoSet.getCurrentData();

	$(this.selector).show();
	$(this.selector+'Padding').show();
	$("#Site").hide();

	$(this.selector+' .photo').attr('src', phoData.url);
	this.onResize();

	$(this.selector+' .details').html(phoData.created.replace(' ', '<br/>'));
	$('body').css('background-color', '#000').css('overflow-y', 'hidden');

	_gaq.push(['_trackPageview', '/Photos/'+phoData.photoId]);

	// preload next image

	var nextId = this.photoSet.getNextPhotoId();
	alert('next: '+nextId);
	alert('next: '+this.photoSet.getData(nextId));
	var img = new Image();
	img.src = this.photoSet.getData(nextId).url;
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.onResize = function() {
	var pv = $(this.selector);
	var pvW = pv.width();
	var pvH = pv.height();
	var pvRatio = pvW/pvH;
	var phoRatio = this.photoSet.getCurrentData().ratio;
	var img = $(this.selector+' .photo');

	if ( pvRatio > phoRatio ) {
		var imgW = phoRatio*pvH;
		img.css('height', '100%').css('width', 'auto').css('margin', '0 0 0 '+(pvW-imgW)/2+'px');
	}
	else {
		var imgH = pvW/phoRatio;
		img.css('width', '100%').css('height', 'auto').css('margin', (pvH-imgH)/2+'px 0 0 0');
	}
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.close = function() {
	$(this.selector).hide();
	$(this.selector+'Padding').hide();
	$('#Site').show();
	$('body').css('background-color', this.origBg).css('overflow-y', this.origOy);
};
