/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function TaggingLayer(pSearchUrl, pAddUrl) {
	this.searchUrl = pSearchUrl;
	this.addUrl = pAddUrl;
	this.spotX = null;
	this.spotY = null;
	this.events = new EventDispatcher('TaggingLayer');
	
	this.liveSearch = new LiveSearchTags(this.searchUrl);
	this.liveSearch.events.listen('itemSelected', this, this.onItemSelected);
}

/*--------------------------------------------------------------------------------------------*/
TaggingLayer.prototype.setPhotoId = function(pPhotoId) {
	this.photoId = pPhotoId;
};

/*--------------------------------------------------------------------------------------------*/
TaggingLayer.prototype.setSpotPos = function(pRelX, pRelY) {
	this.spotX = pRelX;
	this.spotY = pRelY;
};

/*--------------------------------------------------------------------------------------------*/
TaggingLayer.prototype.getSpotPos = function() {
	return {
		x: this.spotX,
		y: this.spotY
	};
};

/*--------------------------------------------------------------------------------------------*/
TaggingLayer.prototype.getSearchUrl = function() {
	return this.searchUrl;
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
TaggingLayer.prototype.onItemSelected = function() {
	this.addTag(this.liveSearch.getSelectId());
};

/*--------------------------------------------------------------------------------------------*/
TaggingLayer.prototype.addTag = function(pArtifactId) {
	var data = {
		PhotoId: this.photoId,
		ArtifactId: pArtifactId,
		PosX: this.spotX,
		PosY: this.spotY
	};

	var compClosure = function(pScope) {
		return function(pResponse) {
			pScope.onAddTagComplete(pResponse);
		};
	};
	
	console.log('TaggingLayer.addTag(): '+
		data.PhotoId+', '+data.ArtifactId+', '+data.PosX+', '+data.PosY);

	$.post(this.addUrl, data, compClosure(this));
};

/*--------------------------------------------------------------------------------------------*/
TaggingLayer.prototype.onAddTagComplete = function(pResponse) {
	this.events.send('addTag'+(pResponse.success == true ? 'Completed' : 'Failed'));
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
TaggingLayer.prototype.onClose = function() {
	this.events.send('closed');
};
