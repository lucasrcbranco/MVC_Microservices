﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShoppingCartAPI.Data;

#nullable disable

namespace Mango.Services.ShoppingCartAPI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240226181715_AdjustUserIdColumnShoppingCartHeaderTable")]
    partial class AdjustUserIdColumnShoppingCartHeaderTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Mango.Services.ShoppingCartAPI.Models.ShoppingCartDetail", b =>
                {
                    b.Property<int>("ShoppingCartDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ShoppingCartDetailId"));

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("ShoppingCartHeaderId")
                        .HasColumnType("int");

                    b.HasKey("ShoppingCartDetailId");

                    b.HasIndex("ShoppingCartHeaderId");

                    b.ToTable("Details");
                });

            modelBuilder.Entity("Mango.Services.ShoppingCartAPI.Models.ShoppingCartHeader", b =>
                {
                    b.Property<int>("ShoppingCartHeaderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ShoppingCartHeaderId"));

                    b.Property<string>("CouponCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ShoppingCartHeaderId");

                    b.ToTable("Headers");
                });

            modelBuilder.Entity("Mango.Services.ShoppingCartAPI.Models.ShoppingCartDetail", b =>
                {
                    b.HasOne("Mango.Services.ShoppingCartAPI.Models.ShoppingCartHeader", "ShoppingCartHeader")
                        .WithMany()
                        .HasForeignKey("ShoppingCartHeaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ShoppingCartHeader");
                });
#pragma warning restore 612, 618
        }
    }
}
