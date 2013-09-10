namespace PhotoGallery.Domain {
	
	/*================================================================================================*/
	public class Human {

		public virtual int Id { get; set; }
		public virtual string Name { get; set; }

		public virtual FabricInstance FabricInstance { get; set; }

    }

}
