using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MelihAkıncı_webTabanliAidatTakipSistemi.Models;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Data {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }





    }

}


