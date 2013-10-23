/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
function PhotoSetTags(pPhotoSet, pTagsUrl) {
	this.photoSet = pPhotoSet;
	this.tagsUrl = pTagsUrl;
	this.activeTagId = null;
	this.events = new EventDispatcher('PhotoSetTags');
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetTags.prototype.loadData = function() {
	var onData = function(pScope) {
		return function(pData) {
			pScope.setTags(pData);
		};
	};

	jQuery.post(this.tagsUrl, null, onData(this));
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetTags.prototype.setTags = function(pTags) {
	this.tagList = [];
	this.idMap = {};
	this.tagCount = 0;
	this.tagMaxCount = 0;

	for ( var i = 0 ; i < pTags.length ; ++i ) {
		var t = pTags[i];
		var count = t.PhotoIds.length;

		this.idMap[t.Id] = this.tagList.length;
		this.tagList.push(t);
		
		this.tagCount += count;
		this.tagMaxCount = Math.max(count, this.tagMaxCount);

		for ( var j = 0 ; j < count ; ++j ) {
			var phoId = t.PhotoIds[j];
			var d = this.photoSet.getData(phoId);

			if ( d ) {
				d.addTag(t);
			}
		}
	}

	this.events.send('dataLoaded');
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetTags.prototype.getTag = function(pTagId) {
	var index = this.idMap[pTagId];
	return this.tagList[index];
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetTags.prototype.getTags = function() {
	return this.tagList;
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetTags.prototype.isPersonTag = function(pTagId) {
	var t = this.getTag(pTagId);
	return (t.Disamb == "female person" || t.Disamb == "male person");
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetTags.prototype.getTagWeight = function(pTagId) {
	var t = this.getTag(pTagId);
	return t.PhotoIds.length/this.tagMaxCount;
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetTags.prototype.isTagActive = function(pTagId) {
	return (this.activeTagId == pTagId);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetTags.prototype.onTagClick = function(pTagId) {
	this.activeTagId = (this.isTagActive(pTagId) ? null : pTagId);

	if ( this.activeTagId == null ) {
		this.photoSet.resetFilter();
	}
	else {
		this.photoSet.setTagFilter(pTagId);
	}
};
