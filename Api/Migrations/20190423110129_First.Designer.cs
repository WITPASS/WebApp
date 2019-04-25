﻿// <auto-generated />
using System;
using Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20190423110129_First")]
    partial class First
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Data.AppUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("BranchId");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Data.Branch", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("BranchId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.ToTable("Branches");
                });

            modelBuilder.Entity("Data.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("BranchId");

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Data.UserRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("BranchId");

                    b.Property<Guid?>("RoleId");

                    b.Property<Guid?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Data.AppUser", b =>
                {
                    b.HasOne("Data.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId");
                });

            modelBuilder.Entity("Data.Branch", b =>
                {
                    b.HasOne("Data.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId");
                });

            modelBuilder.Entity("Data.Role", b =>
                {
                    b.HasOne("Data.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId");
                });

            modelBuilder.Entity("Data.UserRole", b =>
                {
                    b.HasOne("Data.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId");

                    b.HasOne("Data.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("Data.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}
