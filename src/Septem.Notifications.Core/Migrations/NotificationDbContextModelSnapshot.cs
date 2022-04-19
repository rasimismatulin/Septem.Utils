﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Septem.Notifications.Core.Infrastructure;

#nullable disable

namespace Septem.Notifications.Core.Migrations
{
    [DbContext(typeof(NotificationDbContext))]
    partial class NotificationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Septem.Notifications.Core.Entities.NotificationEntity", b =>
                {
                    b.Property<Guid>("Uid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CancellationKey")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Data")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("DefaultLanguage")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("FcmConfiguration")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<Guid?>("GroupKey")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("ModifiedDateUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Payload")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<byte>("Status")
                        .HasColumnType("smallint");

                    b.Property<DateTime>("TimeToSendUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Title")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<byte>("Type")
                        .HasColumnType("smallint");

                    b.HasKey("Uid");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Septem.Notifications.Core.Entities.NotificationMessageEntity", b =>
                {
                    b.Property<Guid>("Uid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ExceptionMessage")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsView")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("ModifiedDateUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("NotificationTokenUid")
                        .HasColumnType("uuid");

                    b.Property<Guid>("NotificationUid")
                        .HasColumnType("uuid");

                    b.Property<string>("Payload")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("ServiceMessage")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<byte>("Status")
                        .HasColumnType("smallint");

                    b.Property<string>("Title")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Token")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<byte?>("TokenType")
                        .HasColumnType("smallint");

                    b.HasKey("Uid");

                    b.HasIndex("NotificationTokenUid");

                    b.HasIndex("NotificationUid");

                    b.ToTable("NotificationMessages");
                });

            modelBuilder.Entity("Septem.Notifications.Core.Entities.NotificationReceiverEntity", b =>
                {
                    b.Property<Guid>("Uid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("NotificationUid")
                        .HasColumnType("uuid");

                    b.Property<string>("Parameters")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<byte>("ReceiverType")
                        .HasColumnType("smallint");

                    b.Property<Guid?>("TargetUid")
                        .HasColumnType("uuid");

                    b.Property<string>("Token")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.HasKey("Uid");

                    b.HasIndex("NotificationUid");

                    b.ToTable("NotificationReceivers");
                });

            modelBuilder.Entity("Septem.Notifications.Core.Entities.NotificationTokenEntity", b =>
                {
                    b.Property<Guid>("Uid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DeviceId")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Language")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedDateUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("TargetUid")
                        .HasColumnType("uuid");

                    b.Property<string>("Token")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<byte>("Type")
                        .HasColumnType("smallint");

                    b.HasKey("Uid");

                    b.ToTable("NotificationTokens");
                });

            modelBuilder.Entity("Septem.Notifications.Core.Entities.NotificationMessageEntity", b =>
                {
                    b.HasOne("Septem.Notifications.Core.Entities.NotificationTokenEntity", "NotificationToken")
                        .WithMany()
                        .HasForeignKey("NotificationTokenUid");

                    b.HasOne("Septem.Notifications.Core.Entities.NotificationEntity", "Notification")
                        .WithMany("Messages")
                        .HasForeignKey("NotificationUid")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Notification");

                    b.Navigation("NotificationToken");
                });

            modelBuilder.Entity("Septem.Notifications.Core.Entities.NotificationReceiverEntity", b =>
                {
                    b.HasOne("Septem.Notifications.Core.Entities.NotificationEntity", "Notification")
                        .WithMany("Receivers")
                        .HasForeignKey("NotificationUid")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Notification");
                });

            modelBuilder.Entity("Septem.Notifications.Core.Entities.NotificationEntity", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("Receivers");
                });
#pragma warning restore 612, 618
        }
    }
}
