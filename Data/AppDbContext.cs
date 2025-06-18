using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MelihAkıncı_webTabanliAidatTakipSistemi.Models;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Data {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ApartmentResident> ApartmentResidents { get; set; }
        public DbSet<ApartmentManager> ApartmentManagers { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<Apartments> Apartments { get; set; }

        public DbSet<ApartmentUnit> ApartmentUnits { get; set; }
        
        public DbSet<MaintenanceFee> MaintenanceFees { get; set; }
        public DbSet<ResidentsSpecificDebt> ResidentsSpecificDebts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
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
            modelBuilder.Entity<Apartments>()
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

            // ResidentsSpecificDebt ve ApartmentResident arasındaki ilişkiyi özelleştirme
            modelBuilder.Entity<ResidentsSpecificDebt>()
             .HasOne(ResidentsSpecificDebt => ResidentsSpecificDebt.ApartmentResident)
             .WithMany()
             .HasForeignKey(ResidentsSpecificDebt => ResidentsSpecificDebt.ResidentId);


            // Diğer ilişki tanımlamaları
            base.OnModelCreating(modelBuilder);
        }



    }

}


