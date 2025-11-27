

namespace Entities
{
    public partial class AmdDBContext : DbContext
    {
        public AmdDBContext()
        {
           
        }

        public AmdDBContext(DbContextOptions<AmdDBContext> options) : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Container> Containers { get; set; }
        public virtual DbSet<Auto> Autos { get; set; }
        public virtual DbSet<AutoImage> AutoImages { get; set; }
        public virtual DbSet<Lookup> Lookups { get; set; }
        public virtual DbSet<LookupValue> LookupValues { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentDetails> PaymentDetails { get; set; }
        public virtual DbSet<ContainerImages> ContainerImages { get; set; }
        public virtual DbSet<Resources> Resources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auto>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.RemainingPayment).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Auction)
                    .WithMany(p => p.AutoAuctions)
                    .HasForeignKey(d => d.AuctionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.AutoBrands)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Buyer)
                    .WithMany(p => p.AutoBuyers)
                    .HasForeignKey(d => d.BuyerId);

                entity.HasOne(d => d.BuyingAccount)
                    .WithMany(p => p.AutoBuyingAccounts)
                    .HasForeignKey(d => d.BuyingAccountId);

                entity.HasOne(d => d.City)
                    .WithMany(p => p.AutoCities)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Container)
                    .WithMany(p => p.Autos)
                    .HasForeignKey(d => d.ContainerId);

                entity.HasOne(d => d.CreationUser)
                    .WithMany(p => p.AutoCreationUsers)
                    .HasForeignKey(d => d.CreationUserId);

                entity.HasOne(d => d.Destination)
                    .WithMany(p => p.AutoDestinations)
                    .HasForeignKey(d => d.DestinationId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LoadPort)
                    .WithMany(p => p.AutoLoadPorts)
                    .HasForeignKey(d => d.LoadPortId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ModificationUser)
                    .WithMany(p => p.AutoModificationUsers)
                    .HasForeignKey(d => d.ModificationUserId);
            });

            modelBuilder.Entity<AutoImage>(entity =>
            {
                entity.HasOne(d => d.Auto)
                    .WithMany(p => p.Images)
                    .HasForeignKey(d => d.AutoId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CreationUser)
                    .WithMany(p => p.AutoImageCreationUsers)
                    .HasForeignKey(d => d.CreationUserId);

                entity.HasOne(d => d.ModificationUser)
                    .WithMany(p => p.AutoImageModificationUsers)
                    .HasForeignKey(d => d.ModificationUserId);
            });

            modelBuilder.Entity<Container>(entity =>
            {
                entity.HasOne(d => d.CreationUser)
                    .WithMany(p => p.ContainerCreationUsers)
                    .HasForeignKey(d => d.CreationUserId);

                entity.HasOne(d => d.DeparturePort)
                    .WithMany(p => p.ContainerDeparturePorts)
                    .HasForeignKey(d => d.DeparturePortId);

                entity.HasOne(d => d.Destination)
                    .WithMany(p => p.ContainerDestinations)
                    .HasForeignKey(d => d.DestinationId);

                entity.HasOne(d => d.ModificationUser)
                    .WithMany(p => p.ContainerModificationUsers)
                    .HasForeignKey(d => d.ModificationUserId);

                entity.HasOne(d => d.ShippingCompany)
                    .WithMany(p => p.ContainerShippingCompanies)
                    .HasForeignKey(d => d.ShippingCompanyId);
            });

            modelBuilder.Entity<ContainerImages>(entity =>
            {
                entity.HasOne(d => d.Container)
                    .WithMany(p => p.Images)
                    .HasForeignKey(d => d.ContainerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CreationUser)
                    .WithMany(p => p.ContainerImageCreationUsers)
                    .HasForeignKey(d => d.CreationUserId);

                entity.HasOne(d => d.ModificationUser)
                    .WithMany(p => p.ContainerImageModificationUsers)
                    .HasForeignKey(d => d.ModificationUserId);
            });

            modelBuilder.Entity<Lookup>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<LookupValue>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.Lookup)
                    .WithMany(p => p.LookupValues)
                    .HasForeignKey(d => d.LookupId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Auto)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.AutoId);

                entity.HasOne(d => d.CreationUser)
                    .WithMany(p => p.PaymentCreationUsers)
                    .HasForeignKey(d => d.CreationUserId);

                entity.HasOne(d => d.ModificationUser)
                    .WithMany(p => p.PaymentModificationUsers)
                    .HasForeignKey(d => d.ModificationUserId);
            });

            modelBuilder.Entity<PaymentDetails>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Resources>(entity =>
            {
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Password).IsRequired();

                entity.HasOne(d => d.CreationUser)
                    .WithMany(p => p.InverseCreationUser)
                    .HasForeignKey(d => d.CreationUserId);
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
