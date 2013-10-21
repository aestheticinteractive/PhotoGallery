/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
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


////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.setFilter = function(pShowPhotoIds) {
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


////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.showPhoto = function(pPhotoId) {
	if ( !this.currentMap[pPhotoId] ) {
		console.log('PhotoSet.showPhoto(): photo '+pPhotoId+' is not available.');
		return;
	}

	this.activeId = pPhotoId;
	$(this).trigger('photoChange');
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.showPrevPhoto = function() {
	var i = this.currentMap[this.activeId]-1;
	var cl = this.currentList;
	this.showPhoto(cl[(i < 0 ? cl.length-1 : i)]);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.showNextPhoto = function() {
	var i = this.currentMap[this.activeId]+1;
	var cl = this.currentList;
	this.showPhoto(cl[(i >= cl.length ? 0 : i)]);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.hideCurrentPhoto = function() {
	this.activeId = null;
	$(this).trigger('photoChange');
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.setPhotoMetas = function(pMetas) {
	for ( var i = 0 ; i < pMetas.PhotoMetas.length ; ++i ) {
		var pm = pMetas.PhotoMetas[i];
		var d = this.getData(pm.PhotoId);

		if ( d ) {
			d.setMetas(pm.Exposure, pm.FNumber, pm.FocalLen, pm.IsoSpeed, pm.UsesFlash);
		}
	}
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.getMetaValues = function(pProp) {
	var cl = this.currentList;
	var vals = [];

	for ( var i = 0 ; i < cl.length ; ++i ) {
		var d = cl[i];
		var val = pProp(d);

		if ( val ) {
			vals.push(val);
		}
	}

	return vals;
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSet.prototype.getMetaAverage = function(pProp) {
	var vals = this.getMetaValues(pProp);
	var sum = 0;
	var count = 0;

	for ( var i = 0 ; i < vals.length ; ++i ) {
		sum += vals[i];
		count++;
	}

	return (count == 0 ? null : sum/count);
};

/*----------------------------------------------------------------------------------------------------* /
PhotoSet.prototype.GetAverageFNumberString = function() {
	double? fn = vAverages[Metric.FNumber];

	if ( fn == null ) {
		return null;
	}

	double fnd = (double)fn;
	return "f/"+fnd.ToString("0.00");
};
		
/*----------------------------------------------------------------------------------------------------* /
PhotoSet.prototype.GetAverageIsoSpeedString = function() {
	double? i = vAverages[Metric.IsoSpeed];

	if ( i == null ) {
		return null;
	}

	double id = (double)i;
	return id.ToString("0.0");
};

/*----------------------------------------------------------------------------------------------------* /
PhotoSet.prototype.GetAveragExposureString = function() {
	double? ex = vAverages[Metric.Exposure];

	if ( ex == null ) {
		return null;
	}

	double exd = (double)ex;

	if ( ex > 0.25 ) {
		return exd.ToString("0.00")+" sec";
	}

	return "1/"+(1/exd).ToString("0.00")+" sec";
};

/*----------------------------------------------------------------------------------------------------* /
PhotoSet.prototype.GetAverageFocalLengthString = function() {
	double? fl = vAverages[Metric.FocalLength];

	if ( fl == null ) {
		return null;
	}

	double fld = (double)fl;
	return fld.ToString("0.0")+" mm";
};

/*----------------------------------------------------------------------------------------------------* /
PhotoSet.prototype.GetFlashUsageString = function() {
	return (FlashCount/(double)PhotoCount*100).ToString("0.0")+"%";
};*/
