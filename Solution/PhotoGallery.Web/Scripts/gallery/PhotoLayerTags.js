/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function PhotoLayerTags(pTagsUrl) {
	this.tagsUrl = pTagsUrl;
	this.cache = {};
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

/*--------------------------------------------------------------------------------------------*/
PhotoLayerTags.prototype.setPhotoId = function(pPhotoId) {
	this.photoId = pPhotoId;
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
PhotoLayerTags.prototype.cacheTagData = function(pTag) {
	this.cache[pTag.ArtifactId] = pTag;
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerTags.prototype.getTagData = function(pArtifactId) {
	return this.cache[pArtifactId];
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
PhotoLayerTags.prototype.loadTags = function() {
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
	this.request = $.post(this.tagsUrl+this.photoId, null, tagsClosure(this));
	this.events.send('loadStarted');
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayerTags.prototype.onTagsLoad = function(pData) {
	this.tags = pData;
	this.events.send('loadCompleted');
};
