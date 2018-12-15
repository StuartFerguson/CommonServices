﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SubscriptionService.Database;

namespace SubscriptionService.Database.Migrations
{
    [DbContext(typeof(SubscriptionServiceConfigurationContext))]
    [Migration("20181212111524_RemoveIsNetCoreStream")]
    partial class RemoveIsNetCoreStream
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
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

            modelBuilder.Entity("SubscriptionService.Database.Models.EndPoint", b =>
                {
                    b.Property<Guid>("EndPointId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Url");

                    b.HasKey("EndPointId");

                    b.ToTable("EndPoints");
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
#pragma warning restore 612, 618
        }
    }
}