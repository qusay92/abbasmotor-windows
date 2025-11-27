namespace Entities
{
    public class User : BaseModel
    {
        public User()
        {
            AutoBuyers = new HashSet<Auto>();
            AutoCreationUsers = new HashSet<Auto>();
            AutoImageCreationUsers = new HashSet<AutoImage>();
            AutoImageModificationUsers = new HashSet<AutoImage>();
            AutoModificationUsers = new HashSet<Auto>();
            ContainerCreationUsers = new HashSet<Container>();
            ContainerImageCreationUsers = new HashSet<ContainerImages>();
            ContainerImageModificationUsers = new HashSet<ContainerImages>();
            ContainerModificationUsers = new HashSet<Container>();
            InverseCreationUser = new HashSet<User>();
            PaymentCreationUsers = new HashSet<Payment>();
            PaymentModificationUsers = new HashSet<Payment>();
        }

        public string Name { get; set; }
        public string Password { get; set; }
        public string ActualPass { get; set; }
        public int Type { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string UserName { get; set; }
        public string Company { get; set; }
        public virtual ICollection<Auto> AutoBuyers { get; set; }
        public virtual ICollection<Auto> AutoCreationUsers { get; set; }
        public virtual ICollection<AutoImage> AutoImageCreationUsers { get; set; }
        public virtual ICollection<AutoImage> AutoImageModificationUsers { get; set; }
        public virtual ICollection<Auto> AutoModificationUsers { get; set; }
        public virtual ICollection<Container> ContainerCreationUsers { get; set; }
        public virtual ICollection<ContainerImages> ContainerImageCreationUsers { get; set; }
        public virtual ICollection<ContainerImages> ContainerImageModificationUsers { get; set; }
        public virtual ICollection<Container> ContainerModificationUsers { get; set; }
        public virtual ICollection<User> InverseCreationUser { get; set; }
        public virtual ICollection<Payment> PaymentCreationUsers { get; set; }
        public virtual ICollection<Payment> PaymentModificationUsers { get; set; }

    }
}
