/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function PhotoLayer(pPhotoSet, pTagsUrl, pSearchUrl, pAddUrl) {
	this.photoSet = pPhotoSet;
	this.photoSet.events.listen("photoChanged", this, this.onPhoto);

	this.tags = new PhotoLayerTags(pTagsUrl);

	this.tagLayer = new TaggingLayer(pSearchUrl, pAddUrl);
	this.tagLayer.events.listen('addTagStarted', this, this.onAddTagStarted);
	this.tagLayer.events.listen('addTagCompleted', this, this.onAddTagCompleted);
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayer.prototype.getCurrentPhoto = function() {
	return this.currPhoto;
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayer.prototype.onPhoto = function() {
	this.currPhoto = this.photoSet.getCurrentData();

	if ( this.currPhoto ) {
		var id = this.currPhoto.photoId;

		this.tags.setPhotoId(id);
		this.tagLayer.setPhotoId(id);
		this.preloadNextImage();

		trackPageview('/Photos/'+id);
	}
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayer.prototype.preloadNextImage = function() {
	var id = this.photoSet.getNextPhotoId();

	var img = new Image();
	img.src = this.photoSet.getData(id).url;
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
PhotoLayer.prototype.onAddTagStarted = function() {
	this.tags.cacheTagData(this.tagLayer.getSelectedItem());
};

/*--------------------------------------------------------------------------------------------*/
PhotoLayer.prototype.onAddTagCompleted = function() {
	this.tags.loadTags();
};
