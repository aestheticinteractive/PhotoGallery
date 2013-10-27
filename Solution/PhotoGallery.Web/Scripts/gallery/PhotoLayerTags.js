/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function PhotoLayerTags(pTagsUrl) {
	this.tagsUrl = pTagsUrl;
	this.events = new EventDispatcher('PhotoLayerTags');
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerTags.prototype.getTagCount = function() {
	return this.tags.length;
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerTags.prototype.getTag = function(pIndex) {
	return this.tags[pIndex];
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
PhotoLayerTags.prototype.loadTags = function(pPhotoId) {
	if ( this.request ) {
		this.request.abort();
		this.request = null;
	}

	var tagsClosure = function(pScope) {
		return function(pData) {
			pScope.onTagsLoad(pData);
		};
	};

	this.tags = [];
	this.request = $.post(this.tagsUrl+pPhotoId, null, tagsClosure(this));
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerTags.prototype.onTagsLoad = function(pData) {
	this.tags = pData;
	this.events.send('tagsLoaded');
};
