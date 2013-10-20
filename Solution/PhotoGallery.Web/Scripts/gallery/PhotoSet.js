/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function PhotoSet() {
	this.dataList = [];
	this.dataMap = {};
	this.filtList = null;
	this.filtMap = null;
	this.activeId = null;
	
	this.currentList = function() {
		return (this.filtList == null ? this.dataList : this.filtList);
	};

	this.currentMap = function() {
		return (this.filtMap == null ? this.dataMap : this.filtMap);
	};
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
PhotoDataSet.prototype.addData = function(pPhotoId, pUrl, pCreated, pRatio) {
	var d = new PhotoData(pPhotoId, pUrl, pCreated, pRatio);
	this.dataMap[pPhotoId] = this.dataList.length;
	this.dataList.push(d);
};

/*--------------------------------------------------------------------------------------------*/
PhotoDataSet.prototype.getData = function(pPhotoId) { /* PhotoData */
	return this.dataMap[pPhotoId];
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
PhotoDataSet.prototype.setFilter = function(pShowPhotoIds) {
	this.filtList = pShowPhotoIds;
	this.filtMap = null;
	
	if ( this.filtList != null ) {
		this.filtMap = {};

		for ( var i = 0 ; i < this.filtList.length ; ++i ) {
			var id = this.filtList[i];
			this.filtMap[id] = i;
		}
	}

	$(this).trigger('filterChange');
};

/*--------------------------------------------------------------------------------------------*/
PhotoDataSet.prototype.resetFilter = function() {
	this.setFilter(null);
};

/*--------------------------------------------------------------------------------------------*/
PhotoDataSet.prototype.setTagFilter = function(pTagId) {
	var photoIds = [];

	for ( var i = 0 ; i < this.dataList.length ; ++i ) {
		var d = this.dataList[i];
		
		if ( d.hasTag(pTagId) ) {
			photoIds.push(d.photoId);
		}
	}

	this.setFilter(photoIds);
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
PhotoDataSet.prototype.showPhoto = function(pPhotoId) {
	if ( !this.currentMap[pPhotoId] ) {
		console.log('PhotoSet.showPhoto(): photo '+pPhotoId+' is not available.');
		return;
	}

	this.activeId = pPhotoId;
	$(this).trigger('photoChange');
};

/*--------------------------------------------------------------------------------------------*/
PhotoDataSet.prototype.showPrevPhoto = function() {
	var i = this.currentMap[this.activeId]-1;
	var cl = this.currentList;
	this.showPhoto(cl[(i < 0 ? cl.length-1 : i)]);
};

/*--------------------------------------------------------------------------------------------*/
PhotoDataSet.prototype.showNextPhoto = function() {
	var i = this.currentMap[this.activeId]+1;
	var cl = this.currentList;
	this.showPhoto(cl[(i >= cl.length ? 0 : i)]);
};

/*--------------------------------------------------------------------------------------------*/
PhotoDataSet.prototype.hideCurrentPhoto = function() {
	this.activeId = null;
	$(this).trigger('photoChange');
};
