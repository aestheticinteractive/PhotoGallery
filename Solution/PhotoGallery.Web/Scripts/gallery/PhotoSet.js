/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
function PhotoSet() {
	this.dataList = [];
	this.dataMap = {};
	this.filtList = null;
	this.filtMap = null;
	this.activeId = null;
	this.listeners = {};
	
	this.currentList = function() {
		return (this.filtList == null ? this.dataList : this.filtList);
	};

	this.currentMap = function() {
		return (this.filtMap == null ? this.dataMap : this.filtMap);
	};
};


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.addListener = function(pEventName, pCallbackScope, pCallback) {
	if ( !this.listeners[pEventName] ) {
		this.listeners[pEventName] = [];
	}

	var closure = function(pScope) {
		return function() {
			pCallback.apply(pScope);
		};
	};

	this.listeners[pEventName].push(closure(pCallbackScope));
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.dispatchEvent = function(pEventName) {
	var list = this.listeners[pEventName];

	for ( var i = 0 ; i < list.length ; ++i ) {
		list[i].call();
	}
};


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.addData = function(pPhotoId, pUrl, pCreated, pRatio) {
	if ( this.dataList == null ) {
		alert('broke');
	}

	var d = new PhotoData(pPhotoId, pUrl, pCreated, pRatio);
	this.dataMap[pPhotoId] = this.dataList.length;
	this.dataList.push(d);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.getData = function(pPhotoId) { /* PhotoData */
	var index = this.dataMap[pPhotoId];
	return this.dataList[index];
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.getPhotoCount = function(pFiltered) {
	return (pFiltered && this.filtList ? this.filtList : this.dataList).length;
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.isPhotoAvailable = function(pPhotoId) {
	return (this.currentMap()[pPhotoId] >= 0);
};


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.setFilter = function(pShowPhotoIds) {
	if ( pShowPhotoIds ) {
		this.filtList = [];
		this.filtMap = {};

		for ( var i = 0 ; i < pShowPhotoIds.length ; ++i ) {
			var id = pShowPhotoIds[i];
			var d = this.getData(id);
			this.filtMap[id] = this.filtList.length;
			this.filtList.push(d);
		}
	}
	else {
		this.filtList = null;
		this.filtMap = null;
	}
	
	this.dispatchEvent("filterChange");
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.resetFilter = function() {
	this.setFilter(null);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.setTagFilter = function(pTagId) {
	var photoIds = [];

	for ( var i = 0 ; i < this.dataList.length ; ++i ) {
		var d = this.dataList[i];
		
		if ( d.hasTag(pTagId) ) {
			photoIds.push(d.photoId);
		}
	}

	this.setFilter(photoIds);
};


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.showPhoto = function(pPhotoId) {
	if ( !this.isPhotoAvailable(pPhotoId) ) {
		console.log('PhotoSet.showPhoto(): photo '+pPhotoId+' is not available.');
		return;
	}

	this.activeId = pPhotoId;
	this.dispatchEvent("filterChange");
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.showPrevPhoto = function() {
	var i = this.currentMap()[this.activeId]-1;
	var cl = this.currentList();
	this.showPhoto(cl[(i < 0 ? cl.length-1 : i)]);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.showNextPhoto = function() {
	var i = this.currentMap()[this.activeId]+1;
	var cl = this.currentList();
	this.showPhoto(cl[(i >= cl.length ? 0 : i)]);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.hideCurrentPhoto = function() {
	this.activeId = null;
	$(this).trigger('photoChange');
};
