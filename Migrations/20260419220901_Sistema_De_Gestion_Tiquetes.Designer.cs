
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

#nullable disable

namespace Sistema_de_gestion_de_tiquetes_Aereos.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260419220901_Sistema_De_Gestion_Tiquetes")]
    partial class Sistema_De_Gestion_Tiquetes
    {
        
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);
#pragma warning restore 612, 618
        }
    }
}
