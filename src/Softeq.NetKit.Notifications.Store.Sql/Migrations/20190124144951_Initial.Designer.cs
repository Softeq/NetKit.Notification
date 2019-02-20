﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Softeq.NetKit.Notifications.Store.Sql;

namespace Softeq.NetKit.Notifications.Store.Sql.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190124144951_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Softeq.NetKit.Notifications.Store.Sql.Models.NotificationRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("Created");

                    b.Property<string>("OwnerUserId");

                    b.Property<string>("Parameters");

                    b.Property<string>("Event");

                    b.Property<Guid>("UserSettingsId");

                    b.HasKey("Id");

                    b.HasIndex("UserSettingsId");

                    b.ToTable("NotificationRecords");
                });

            modelBuilder.Entity("Softeq.NetKit.Notifications.Store.Sql.Models.NotificationSetting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Enabled");

                    b.Property<string>("Event");

                    b.Property<string>("Type");

                    b.Property<Guid>("UserSettingsId");

                    b.HasKey("Id");

                    b.HasIndex("UserSettingsId");

                    b.ToTable("NotificationSetting");
                });

            modelBuilder.Entity("Softeq.NetKit.Notifications.Store.Sql.Models.UserSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("Created");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("Language");

                    b.Property<string>("LastName");

                    b.Property<string>("PhoneNumber");

                    b.Property<DateTimeOffset?>("Updated");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("Softeq.NetKit.Notifications.Store.Sql.Models.NotificationRecord", b =>
                {
                    b.HasOne("Softeq.NetKit.Notifications.Store.Sql.Models.UserSettings", "UserSettings")
                        .WithMany("NotificationRecords")
                        .HasForeignKey("UserSettingsId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Softeq.NetKit.Notifications.Store.Sql.Models.NotificationSetting", b =>
                {
                    b.HasOne("Softeq.NetKit.Notifications.Store.Sql.Models.UserSettings", "UserSettings")
                        .WithMany("Settings")
                        .HasForeignKey("UserSettingsId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}