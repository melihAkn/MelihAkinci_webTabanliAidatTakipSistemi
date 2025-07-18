using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MelihAkıncı_webTabanliAidatTakipSistemi.Models;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Data {
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
        public DbSet<ApartmentResident> ApartmentResidents { get; set; }
        public DbSet<ApartmentManager> ApartmentManagers { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<Apartment> Apartments { get; set; }

        public DbSet<ApartmentUnit> ApartmentUnits { get; set; }

        public DbSet<MaintenanceFee> MaintenanceFees { get; set; }
        public DbSet<ResidentsSpecificFee> ResidentsSpecificFees { get; set; }
        public DbSet<PaymentNotifications> PaymentNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            //apartman yönetici tablosu için unique ayarlamaları
            modelBuilder.Entity<ApartmentManager>()
               .HasIndex(ApartmentManager => ApartmentManager.Email)
               .IsUnique();

            modelBuilder.Entity<ApartmentManager>()
               .HasIndex(ApartmentManager => ApartmentManager.Username)
               .IsUnique();

            //apartman sakini tablosu için unique ayarlamaları
            modelBuilder.Entity<ApartmentResident>()
               .HasIndex(ApartmentResident => ApartmentResident.Username)
               .IsUnique();

            modelBuilder.Entity<ApartmentResident>()
               .HasIndex(ApartmentResident => ApartmentResident.Email)
               .IsUnique();
            //role ilişkilendirmesi
            modelBuilder.Entity<ApartmentManager>()
               .HasOne(ApartmentManager => ApartmentManager.UserRoles)
               .WithMany()
               .HasForeignKey(ApartmentManager => ApartmentManager.RoleId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApartmentResident>()
               .HasOne(ApartmentResident => ApartmentResident.UserRoles)
               .WithMany()
               .HasForeignKey(ApartmentResident => ApartmentResident.RoleId)
               .OnDelete(DeleteBehavior.Cascade);

            // ApartmentResident ile ApartmentUnit arasındaki ilişkiyi özelleştirme
            modelBuilder.Entity<ApartmentResident>()
                .HasOne(ApartmentResident => ApartmentResident.ApartmentUnit)
                .WithMany()
                .HasForeignKey(ApartmentResident => ApartmentResident.ApartmentUnitId)
                .OnDelete(DeleteBehavior.Cascade);

            // ApartmentManager ile Apartments arasındaki ilişkiyi özelleştirme
            modelBuilder.Entity<Apartment>()
               .HasOne(Apartments => Apartments.ApartmentManager)
               .WithMany()
               .HasForeignKey(Apartments => Apartments.ManagerId)
               .OnDelete(DeleteBehavior.Cascade);

            // MaintenanceFee ve ResidentsSpecificDebt ile ApartmentResident arasındaki ilişkiyi özelleştirme
            modelBuilder.Entity<MaintenanceFee>()
               .HasOne(MaintenanceFee => MaintenanceFee.ApartmentResident)
               .WithMany()
               .HasForeignKey(MaintenanceFee => MaintenanceFee.ResidentId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResidentsSpecificFee>()
               .HasOne(ResidentsSpecificDebt => ResidentsSpecificDebt.ApartmentResident)
               .WithMany()
               .HasForeignKey(ResidentsSpecificDebt => ResidentsSpecificDebt.ResidentId);

            //apartmentUnit ile apartman arasındaki ilişkiyi özelleştirme
            modelBuilder.Entity<ApartmentUnit>()
               .HasOne(ApartmentUnit => ApartmentUnit.Apartment)
               .WithMany(apartment => apartment.ApartmentUnits)
               .HasForeignKey(unit => unit.ApartmentId)
               .OnDelete(DeleteBehavior.Cascade);

            // payment notifications ile maintenance fee, residents special fee, apartment resident ve apartment arasındaki ilişkiyi özelleştirme
            modelBuilder.Entity<PaymentNotifications>()
                .HasOne(notification => notification.MaintenanceFee)
                .WithMany()
                .HasForeignKey(notification => notification.MaintenanceFeeId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentNotifications>()
                .HasOne(notification => notification.ResidentsSpecificFee)
                .WithMany()
                .HasForeignKey(notification => notification.SpecialFeeId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentNotifications>()
                .HasOne(notification => notification.ApartmentResident)
                .WithMany()
                .HasForeignKey(notification => notification.ResidentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentNotifications>()
                .HasOne(notification => notification.Apartment)
                .WithMany()
                .HasForeignKey(notification => notification.ApartmentId)
                .OnDelete(DeleteBehavior.Cascade);


            base.OnModelCreating(modelBuilder);

            // modeller oluştuktan sonra seed yöntemi ile rol bilgilerini ekleme
            modelBuilder.Entity<UserRoles>().HasData(
                new UserRoles { Id = 1, Role = "Admin" },
                new UserRoles { Id = 2, Role = "ApartmentManager" },
                new UserRoles { Id = 3, Role = "ApartmentResident" }
            );
        }



    }

}


