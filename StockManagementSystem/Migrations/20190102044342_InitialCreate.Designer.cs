﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StockManagementSystem.Data;

namespace StockManagementSystem.Migrations
{
    [DbContext(typeof(ObjectContext))]
    [Migration("20190102044342_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Common.GenericAttribute", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("EntityId");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(400);

                    b.Property<string>("KeyGroup")
                        .IsRequired()
                        .HasMaxLength(400);

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("GenericAttribute");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Devices.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOnUtc");

                    b.Property<DateTime?>("EndDate");

                    b.Property<string>("Latitude");

                    b.Property<string>("Longitude");

                    b.Property<string>("ModelNo")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<string>("ModifiedBy");

                    b.Property<DateTime?>("ModifiedOnUtc");

                    b.Property<string>("SerialNo")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<DateTime?>("StartDate");

                    b.Property<string>("Status");

                    b.Property<int>("StoreId");

                    b.Property<string>("TokenId");

                    b.HasKey("Id");

                    b.HasIndex("StoreId");

                    b.ToTable("Device");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Identity.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ConcurrencyStamp")
                        .HasMaxLength(2147483647);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOnUtc");

                    b.Property<string>("Description")
                        .HasMaxLength(2147483647);

                    b.Property<string>("ModifiedBy");

                    b.Property<DateTime?>("ModifiedOnUtc");

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.Property<string>("SystemName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Identity.RoleClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Identity.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("AdminComment");

                    b.Property<string>("ConcurrencyStamp")
                        .HasMaxLength(2147483647);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOnUtc");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<DateTime>("LastActivityDateUtc");

                    b.Property<string>("LastIpAddress");

                    b.Property<DateTime?>("LastLoginDateUtc");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasMaxLength(7);

                    b.Property<string>("ModifiedBy");

                    b.Property<DateTime?>("ModifiedOnUtc");

                    b.Property<string>("Name")
                        .HasMaxLength(2147483647);

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasMaxLength(2147483647);

                    b.Property<string>("SecurityStamp")
                        .HasMaxLength(2147483647);

                    b.Property<Guid>("UserGuid");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail");

                    b.HasIndex("NormalizedUserName");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Identity.UserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Identity.UserLogin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("LoginProvider")
                        .IsRequired();

                    b.Property<string>("ProviderDisplayName")
                        .HasMaxLength(2147483647);

                    b.Property<string>("ProviderKey")
                        .IsRequired();

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Identity.UserRole", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Identity.UserToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("LoginProvider")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("UserId");

                    b.Property<string>("Value")
                        .HasMaxLength(2147483647);

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Logging.ActivityLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ActivityLogTypeId");

                    b.Property<string>("Comment")
                        .IsRequired();

                    b.Property<DateTime>("CreatedOnUtc");

                    b.Property<int?>("EntityId");

                    b.Property<string>("EntityName")
                        .HasMaxLength(400);

                    b.Property<string>("IpAddress")
                        .HasMaxLength(200);

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ActivityLogTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("ActivityLog");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Logging.ActivityLogType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Enabled");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("SystemKeyword")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("ActivityLogType");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.PushNotification.NotificationCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("NotificationCategories");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.PushNotification.PushNotificationStore", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOnUtc");

                    b.Property<bool?>("IsHHTDownloaded");

                    b.Property<string>("ModifiedBy");

                    b.Property<DateTime?>("ModifiedOnUtc");

                    b.Property<int>("PushNotificationId");

                    b.Property<int>("StoreId");

                    b.HasKey("Id");

                    b.HasIndex("PushNotificationId");

                    b.HasIndex("StoreId");

                    b.ToTable("PushNotificationStores");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.PushNotification.PushNotifications", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOnUtc");

                    b.Property<string>("Desc")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<string>("IsShift");

                    b.Property<string>("ModifiedBy");

                    b.Property<DateTime?>("ModifiedOnUtc");

                    b.Property<int>("NotificationCategoryId");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NotificationCategoryId");

                    b.ToTable("PushNotification");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Security.AclRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("EntityId");

                    b.Property<string>("EntityName")
                        .IsRequired()
                        .HasMaxLength(400);

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AclRecord");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Security.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("SystemName")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Permission");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Security.PermissionRoles", b =>
                {
                    b.Property<int>("PermissionId");

                    b.Property<int>("RoleId");

                    b.HasKey("PermissionId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("PermissionRoles");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Settings.Approval", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsApprovalEnabled");

                    b.HasKey("Id");

                    b.ToTable("Approval");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Stores.Store", b =>
                {
                    b.Property<int>("P_BranchNo");

                    b.Property<string>("P_Addr1");

                    b.Property<string>("P_Addr2");

                    b.Property<string>("P_Addr3");

                    b.Property<string>("P_AreaCode");

                    b.Property<string>("P_Brand");

                    b.Property<string>("P_City");

                    b.Property<string>("P_CompID");

                    b.Property<string>("P_Country");

                    b.Property<string>("P_Name");

                    b.Property<string>("P_PostCode");

                    b.Property<string>("P_RecStatus");

                    b.Property<string>("P_SellPriceLevel");

                    b.Property<string>("P_State");

                    b.HasKey("P_BranchNo");

                    b.ToTable("Store");
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Devices.Device", b =>
                {
                    b.HasOne("StockManagementSystem.Core.Domain.Stores.Store", "Store")
                        .WithMany("Device")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Identity.RoleClaim", b =>
                {
                    b.HasOne("StockManagementSystem.Core.Domain.Identity.Role", "Role")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Identity.UserClaim", b =>
                {
                    b.HasOne("StockManagementSystem.Core.Domain.Identity.User", "User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Identity.UserLogin", b =>
                {
                    b.HasOne("StockManagementSystem.Core.Domain.Identity.User", "User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Identity.UserRole", b =>
                {
                    b.HasOne("StockManagementSystem.Core.Domain.Identity.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StockManagementSystem.Core.Domain.Identity.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Identity.UserToken", b =>
                {
                    b.HasOne("StockManagementSystem.Core.Domain.Identity.User", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Logging.ActivityLog", b =>
                {
                    b.HasOne("StockManagementSystem.Core.Domain.Logging.ActivityLogType", "ActivityLogType")
                        .WithMany()
                        .HasForeignKey("ActivityLogTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StockManagementSystem.Core.Domain.Identity.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.PushNotification.PushNotificationStore", b =>
                {
                    b.HasOne("StockManagementSystem.Core.Domain.PushNotification.PushNotifications", "PushNotifications")
                        .WithMany("PushNotificationStores")
                        .HasForeignKey("PushNotificationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StockManagementSystem.Core.Domain.Stores.Store", "Store")
                        .WithMany("PushNotificationStores")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.PushNotification.PushNotifications", b =>
                {
                    b.HasOne("StockManagementSystem.Core.Domain.PushNotification.NotificationCategory", "NotificationCategories")
                        .WithMany("PushNotification")
                        .HasForeignKey("NotificationCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Security.AclRecord", b =>
                {
                    b.HasOne("StockManagementSystem.Core.Domain.Identity.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StockManagementSystem.Core.Domain.Security.PermissionRoles", b =>
                {
                    b.HasOne("StockManagementSystem.Core.Domain.Security.Permission", "Permission")
                        .WithMany("PermissionRoles")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StockManagementSystem.Core.Domain.Identity.Role", "Role")
                        .WithMany("PermissionRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
