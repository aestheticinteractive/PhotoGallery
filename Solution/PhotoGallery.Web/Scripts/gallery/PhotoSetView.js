/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
function PhotoSetView(pPhotoSet, pSelector, pSiteSelector) {
	this.photoSet = pPhotoSet;
	this.selector = pSelector;
	this.siteSelector = pSiteSelector;

	this.photoSet.events.listen("filterChanged", this, this.updateView);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetView.prototype.buildView = function() {
	var clickClosure = function(pScope) {
		return function() {
			var id = $(this).attr('data-id');
			pScope.photoSet.showPhoto(id);
		};
	};
	
	var resizeClosure = function(pScope) {
		return function() {
			pScope.onResize();
		};
	};

	var metaClosure = function(pScope) {
		return function() {
			pScope.toggleMeta();
		};
	};

	var tagsClosure = function(pScope) {
		return function() {
			pScope.toggleTags();
		};
	};
	
	var keyClosure = function(pScope) {
		return function(pEvent) {
			pScope.onKeyup(pEvent);
		};
	};

	var n = this.photoSet.dataList.length;

	var top = $('<div>')
		.attr('class', 'row header')
		.append($('<div>')
			.attr('class', 'large-12 columns')
			.append($('<div>')
				.attr('class', 'menu')
				.append($('<div>')
					.attr('class', 'item title')
					.attr('title', 'Title')
					.css('width', 'auto')
					.html(this.photoSet.title)
				)
				.append($('<div>')
					.attr('class', 'item text')
					.attr('title', 'Dates')
					.css('width', 'auto')
					.html(this.photoSet.subtitle)
				)
				.append($('<div>')
					.attr('class', 'item text')
					.attr('title', 'Photo Count')
					.css('width', 'auto')
					.html(n+' photos')
				)
				.append($('<div>')
					.attr('class', 'item')
					.html('Info')
					.click(metaClosure(this))
				)
				.append($('<div>')
					.attr('class', 'item')
					.html('Tags')
					.click(tagsClosure(this))
				)
			)
		);

	this.photoHold = $('<div>');

	for ( var i = 0 ; i < n ; ++i ) {
		var data = this.photoSet.dataList[i];

		var phoDiv = $('<div>')
			.attr('class', 'thumb photo')
			.attr('data-id', data.photoId+'')
			.click(clickClosure(this))
			.append($('<div>')
				.attr('class', 'image')
				.css('background-image', 'url('+data.url+')')
			)
			.append($('<p>')
				.attr('class', 'textBox details')
				.html(data.created)
				.hide()
			);

		this.photoHold.append(phoDiv);
	}

	this.photoHold
		.append($('<div>')
			.css('clear', 'left')
		);

	var metaDiv = $('<div>')
		.attr('id', 'Meta')
		.attr('class', 'infoScreen');

	var tagsDiv = $('<div>')
		.attr('id', 'Tags')
		.attr('class', 'infoScreen');

	$(this.selector)
		.append(top)
		.append($('<div>')
			.attr('class', 'row')
			.append($('<div>')
				.attr('class', 'large-12 columns')
				.css('position', 'relative')
				.append(this.photoHold)
				.append(metaDiv)
				.append(tagsDiv)
			)
		);

	this.thumbs = $(this.selector+' .thumb');

	if ( !isTouch() ) {
		this.thumbs
			.mouseenter(function() {
				$(this).find(".textBox").show();
			})
			.mouseleave(function() {
				$(this).find(".textBox").hide();
			});
	}
	
	this.metaView = new PhotoSetMetaView(this.photoSet.meta, '#Meta');
	this.metaView.hide();

	this.tagsView = new PhotoSetTagsView(this.photoSet.tags, '#Tags');
	this.tagsView.hide();

	this.layerView = new PhotoLayerView(this.photoSet.photoLayer, this.siteSelector);
	this.layerView.buildView();
	this.layerView.hide();

	$(window).resize(resizeClosure(this));
	$(document).keyup(keyClosure(this));

	this.onResize();
};

/*--------------------------------------------------------------------------------------------*/
PhotoSetView.prototype.isVisible = function() {
	return $(this.selector).is(':visible');
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetView.prototype.updateView = function() {
	var ps = this.photoSet;

	this.thumbs.each(function() {
		var id = $(this).attr('data-id');
		
		if ( ps.isPhotoAvailable(id) ) {
			$(this).show();
		}
		else {
			$(this).hide();
		}
	});
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetView.prototype.onResize = function() {
	var fullW = this.photoHold.width();
	var minThumbW = 200;

	if ( fullW <= 480 ) {
		minThumbW = 100;
	}
	else if ( fullW <= 1024 ) {
		minThumbW = 120;
	}
	else if ( fullW <= 1400 ) {
		minThumbW = 160;
	}

	var cols = Math.floor(fullW/minThumbW);
	var w = 100/cols;
	var h = fullW/cols;

	$(this.selector+' .thumb')
		.css('width', w+'%')
		.css('height', h+'px');
};

/*--------------------------------------------------------------------------------------------*/
PhotoSetView.prototype.onKeyup = function(pEvent) {
	if ( !this.isVisible() ) {
		return;
	}

	if ( pEvent.which == ESCAPE_KEY ) {
		this.hideInfoScreen();
	}
};


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
PhotoSetView.prototype.showInfoScreen = function() {
	this.tagsView.hide();
	this.metaView.hide();
	this.thumbs.fadeTo(0, 0.15);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetView.prototype.hideInfoScreen = function() {
	this.tagsView.hide();
	this.metaView.hide();
	this.thumbs.fadeTo(0, 1.0);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetView.prototype.toggleMeta = function() {
	if ( this.metaView.isVisible() ) {
		this.hideInfoScreen();
		return;
	};

	this.showInfoScreen();
	this.metaView.show();
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetView.prototype.toggleTags = function() {
	if ( this.tagsView.isVisible() ) {
		this.hideInfoScreen();
		return;
	};

	this.showInfoScreen();
	this.tagsView.show();
};
