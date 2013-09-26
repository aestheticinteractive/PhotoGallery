namespace PhotoGallery.Domain {
	
	/*================================================================================================*/
	public class FabricPersonSession {

		public virtual int Id { get; set; }
		public virtual string SessionId { get; set; }
		public virtual string GrantCode { get; set; }
		public virtual string BearerToken { get; set; }
		public virtual string RefreshToken { get; set; }
		public virtual long Expiration { get; set; }

		public virtual FabricUser FabricUser { get; set; }

    }

}
