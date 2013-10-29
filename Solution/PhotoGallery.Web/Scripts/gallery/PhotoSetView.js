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

	var n = this.photoSet.dataList.length;

	var top = $('<div>')
		.attr('class', 'row')
		.css('border-bottom', '1px solid #999')
		.css('margin-bottom', '20px')
		.append($('<div>')
			.attr('class', 'large-8 columns')
			.append($('<h2>')
				.html(this.photoSet.title)
			)
		)
		.append($('<div>')
			.attr('class', 'large-2 columns')
			.append($('<p>')
				.html(this.photoSet.subtitle)
			)
		)
		.append($('<div>')
			.attr('class', 'large-2 columns')
			.append($('<p>')
				.html(n+' total photos')
			)
		);
		
	var info = $('<div>')
		.attr('class', 'row')
		.append($('<div>')
			.attr('class', 'large-6 columns')
			.append($('<div>')
				.attr('id', 'Meta')
				.html('Loading meta...')
			)
		)
		.append($('<div>')
			.attr('class', 'large-6 columns')
			.append($('<div>')
				.attr('id', 'Tags')
				.html('Loading tags...')
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

	$(this.selector)
		.append(top)
		.append(info)
		.append($('<div>')
			.attr('class', 'row')
			.append($('<div>')
				.attr('class', 'large-12 columns')
				.append(this.photoHold)
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
	this.tagsView = new PhotoSetTagsView(this.photoSet.tags, '#Tags');

	this.layerView = new PhotoLayerView(this.photoSet.photoLayer, this.siteSelector);
	this.layerView.buildView();
	this.layerView.hide();

	this.photoSet.meta.loadData();
	this.photoSet.tags.loadData();

	$(window).resize(resizeClosure(this));
	this.onResize();
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
