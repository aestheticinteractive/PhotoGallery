/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function PhotoLayerTagsView(pPhotoLayerTags, pSelector) {
	this.photoLayerTags = pPhotoLayerTags;
	this.selector = pSelector;
	this.photoLayerTags.events.listen('loadStarted', this, this.onLoadStart);
	this.photoLayerTags.events.listen('loadCompleted', this, this.onLoadComplete);
}

/*--------------------------------------------------------------------------------------------*/
PhotoLayerTagsView.prototype.buildView = function() {
	this.hold = $(this.selector);
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerTagsView.prototype.isVisible = function() {
	return this.hold.is(':visible');
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerTagsView.prototype.show = function() {
	this.hold.show();
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerTagsView.prototype.hide = function() {
	this.hold.hide();
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerTagsView.prototype.update = function() {
	if ( this.isVisible() ) {
		this.photoLayerTags.loadTags();
	}
	else {
		this.onLoadStart();
	}
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
PhotoLayerTagsView.prototype.onLoadStart = function() {
	this.hold.html('');

	this.hold
		.append($('<div>')
			.attr('class', 'tag')
			.css('font-style', 'italic')
			.html('Loading...')
		);
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerTagsView.prototype.onLoadComplete = function() {
	this.hold.html('');

	var n = this.photoLayerTags.getTagCount();

	for ( var i = 0 ; i < n ; ++i ) {
		var tag = this.photoLayerTags.getTag(i);

		if ( tag.Name == null ) {
			tag = this.photoLayerTags.getTagData(tag.ArtifactId);
		}

		this.hold
			.append($('<div>')
				.attr('class', 'tag')
				.attr('title', tag.Disamb)
				.html(tag.Name ? tag.Name : tag.ArtifactId)
			);
	}
};
