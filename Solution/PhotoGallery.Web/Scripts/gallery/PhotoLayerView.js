/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function PhotoLayerView(pPhotoSet, pSelector) {
	this.photoSet = pPhotoSet;
	this.selector = pSelector;
	this.photoSet.events.listen("photoChanged", this, this.onPhoto);
}

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.initView = function() {
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

	$(document).keydown(keyClosure(this, false)).keyup(keyClosure(this, true));

	$(window).resize(resizeClosure(this));
	$(this.selector+' .photo').click(null, navClosure(this, 1));
	//$(this.selector+' .tagBtn').click(null, tagClosure(this));
	$(this.selector+' .prevBtn').click(null, navClosure(this, -1));
	$(this.selector+' .nextBtn').click(null, navClosure(this, 1));
	$(this.selector+' .closeBtn').click(null, navClosure(this, 0));

	if ( !isTouch() ) {
		$(this.selector+' .buttons a')
			.fadeTo(0, 0.75)
			.mouseenter(function() { $(this).fadeTo(0, 1.0); })
			.mouseleave(function() { $(this).fadeTo(0, 0.75); });
	}

	$('body').prepend($(this.selector+'Padding')).prepend($(this.selector));
	this.origBg = $('body').css('background-color');
	this.origOy = $('body').css('overflow-y');

	this.close();
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.isVisible = function() {
	return $(this.selector).is(':visible');
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.close = function() {
	$(this.selector).hide();
	$(this.selector+'Padding').hide();
	$('#Site').show();
	$('body').css('background-color', this.origBg).css('overflow-y', this.origOy);
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.onKeyDown = function(pEvent) {
	if ( !this.isVisible() ) {
		return;
	}

	var btn;

	switch ( pEvent.which ) {
		case T_KEY: 
			btn = $(this.selector+' .tagBtn');
			break;

		case LEFT_ARROW:
			btn = $(this.selector+' .prevBtn');
			break;

		case RIGHT_ARROW:
			btn = $(this.selector+' .nextBtn');
			break;
			
		case ESCAPE_KEY:
			btn = $(this.selector+' .closeBtn');
			break;

		default:
			return ;
	}

	btn.fadeTo(0, 1.0);
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerView.prototype.onKeyUp = function(pEvent) {
	if ( !this.isVisible() ) {
		return;
	}

	$(this.selector+' .buttons a').fadeTo(0, 0.75);

	switch ( pEvent.which ) {
		//case T_KEY:
		//	break;

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
		this.close();
		return;
	}

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
