using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sample.Data
{
    public partial class CommonContext : DbContext
    {
        //public CommonContext()
        //{
        //}

        public CommonContext(DbContextOptions<CommonContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ContactGroups> ContactGroups { get; set; }
        public virtual DbSet<Contacts> Contacts { get; set; }
  
        public virtual DbSet<MigrationHistory> MigrationHistory { get; set; }
     

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Server=CDC2-L-0176F37;Database=Common;Trusted_Connection=True;");
//            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContactGroups>(entity =>
            {
                entity.HasKey(e => e.ContactGroupId)
                    .HasName("PK_CallTransactionDetail");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.ContactGroups)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Contgroup_Contact");
            });

            modelBuilder.Entity<Contacts>(entity =>
            {
                entity.HasKey(e => e.ContactId)
                    .HasName("PK_CallDetail");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MobileNumber)
                    .IsRequired()
                    .HasMaxLength(12);
            });

          

            modelBuilder.Entity<MigrationHistory>(entity =>
            {
                entity.HasKey(e => new { e.MigrationId, e.ContextKey })
                    .HasName("PK_dbo.__MigrationHistory");

                entity.ToTable("__MigrationHistory");

                entity.Property(e => e.MigrationId).HasMaxLength(150);

                entity.Property(e => e.ContextKey).HasMaxLength(300);

                entity.Property(e => e.Model).IsRequired();

                entity.Property(e => e.ProductVersion)
                    .IsRequired()
                    .HasMaxLength(32);
            });

         

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
