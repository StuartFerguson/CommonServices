﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SubscriptionService.Database;

namespace SubscriptionService.Database.Migrations
{
    [DbContext(typeof(SubscriptionServiceConfigurationContext))]
    partial class SubscriptionServiceConfigurationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SubscriptionService.Database.Models.CatchUpSubscription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDateTime");

                    b.Property<Guid>("EndPointId");

                    b.Property<string>("Name");

                    b.Property<int>("Position");

                    b.Property<string>("StreamName");

                    b.HasKey("Id");

                    b.HasIndex("EndPointId");

                    b.ToTable("CatchUpSubscriptions");
                });

            modelBuilder.Entity("SubscriptionService.Database.Models.CatchupSubscriptionConfiguration", b =>
                {
                    b.Property<Guid>("SubscriptionId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDateTime");

                    b.Property<string>("EndPointUri");

                    b.Property<Guid>("EventStoreServerId");

                    b.Property<string>("Name");

                    b.Property<int>("Position");

                    b.Property<string>("StreamName");

                    b.HasKey("SubscriptionId");

                    b.HasIndex("EventStoreServerId");

                    b.ToTable("CatchupSubscriptionConfigurations");
                });

            modelBuilder.Entity("SubscriptionService.Database.Models.EndPoint", b =>
                {
                    b.Property<Guid>("EndPointId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Url");

                    b.HasKey("EndPointId");

                    b.ToTable("EndPoints");
                });

            modelBuilder.Entity("SubscriptionService.Database.Models.EventStoreServer", b =>
                {
                    b.Property<Guid>("EventStoreServerId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConnectionString");

                    b.Property<string>("Name");

                    b.HasKey("EventStoreServerId");

                    b.ToTable("EventStoreServers");
                });

            modelBuilder.Entity("SubscriptionService.Database.Models.SubscriptionConfiguration", b =>
                {
                    b.Property<Guid>("SubscriptionId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("EndPointUri");

                    b.Property<Guid>("EventStoreServerId");

                    b.Property<string>("GroupName");

                    b.Property<string>("StreamName");

                    b.Property<int?>("StreamPosition");

                    b.HasKey("SubscriptionId");

                    b.HasIndex("EventStoreServerId");

                    b.ToTable("SubscriptionConfigurations");
                });

            modelBuilder.Entity("SubscriptionService.Database.Models.SubscriptionGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BufferSize");

                    b.Property<Guid>("EndPointId");

                    b.Property<string>("Name");

                    b.Property<int?>("StreamPosition");

                    b.Property<Guid>("SubscriptionStreamId");

                    b.HasKey("Id");

                    b.HasIndex("EndPointId");

                    b.HasIndex("SubscriptionStreamId");

                    b.ToTable("SubscriptionGroups");
                });

            modelBuilder.Entity("SubscriptionService.Database.Models.SubscriptionService", b =>
                {
                    b.Property<Guid>("SubscriptionServiceId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.HasKey("SubscriptionServiceId");

                    b.ToTable("SubscriptionServices");
                });

            modelBuilder.Entity("SubscriptionService.Database.Models.SubscriptionServiceGroup", b =>
                {
                    b.Property<Guid>("SubscriptionServiceGroupId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("SubscriptionGroupId");

                    b.Property<Guid>("SubscriptionServiceId");

                    b.HasKey("SubscriptionServiceGroupId");

                    b.HasIndex("SubscriptionGroupId");

                    b.HasIndex("SubscriptionServiceId");

                    b.ToTable("SubscriptionServiceGroups");
                });

            modelBuilder.Entity("SubscriptionService.Database.Models.SubscriptionStream", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("StreamName");

                    b.Property<int>("SubscriptionType");

                    b.HasKey("Id");

                    b.ToTable("SubscriptionStream");
                });

            modelBuilder.Entity("SubscriptionService.Database.Models.CatchUpSubscription", b =>
                {
                    b.HasOne("SubscriptionService.Database.Models.EndPoint", "EndPoint")
                        .WithMany()
                        .HasForeignKey("EndPointId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SubscriptionService.Database.Models.CatchupSubscriptionConfiguration", b =>
                {
                    b.HasOne("SubscriptionService.Database.Models.EventStoreServer", "EventStoreServer")
                        .WithMany()
                        .HasForeignKey("EventStoreServerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SubscriptionService.Database.Models.SubscriptionConfiguration", b =>
                {
                    b.HasOne("SubscriptionService.Database.Models.EventStoreServer", "EventStoreServer")
                        .WithMany()
                        .HasForeignKey("EventStoreServerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SubscriptionService.Database.Models.SubscriptionGroup", b =>
                {
                    b.HasOne("SubscriptionService.Database.Models.EndPoint", "EndPoint")
                        .WithMany()
                        .HasForeignKey("EndPointId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SubscriptionService.Database.Models.SubscriptionStream", "SubscriptionStream")
                        .WithMany()
                        .HasForeignKey("SubscriptionStreamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SubscriptionService.Database.Models.SubscriptionServiceGroup", b =>
                {
                    b.HasOne("SubscriptionService.Database.Models.SubscriptionGroup", "SubscriptionGroup")
                        .WithMany()
                        .HasForeignKey("SubscriptionGroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SubscriptionService.Database.Models.SubscriptionService", "SubscriptionService")
                        .WithMany()
                        .HasForeignKey("SubscriptionServiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
