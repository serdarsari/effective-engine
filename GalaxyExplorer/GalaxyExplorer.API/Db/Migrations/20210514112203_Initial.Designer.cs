﻿// <auto-generated />
using System;
using GalaxyExplorer.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GalaxyExplorer.API.Db.Migrations
{
    [DbContext(typeof(GalaxyExplorerDbContext))]
    [Migration("20210514112203_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GalaxyExplorer.Entity.Mission", b =>
                {
                    b.Property<int>("MissionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlannedDuration")
                        .HasColumnType("int");

                    b.Property<int>("SpaceshipId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("MissionId");

                    b.ToTable("Missions");
                });

            modelBuilder.Entity("GalaxyExplorer.Entity.Spaceship", b =>
                {
                    b.Property<int>("SpaceshipId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("MaxCrewCount")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("OnMission")
                        .HasColumnType("bit");

                    b.Property<double>("Range")
                        .HasColumnType("float");

                    b.HasKey("SpaceshipId");

                    b.ToTable("Spaceships");

                    b.HasData(
                        new
                        {
                            SpaceshipId = 1,
                            MaxCrewCount = 2,
                            Name = "Saturn IV Rocket",
                            OnMission = false,
                            Range = 1.2
                        },
                        new
                        {
                            SpaceshipId = 2,
                            MaxCrewCount = 5,
                            Name = "Pathfinder",
                            OnMission = true,
                            Range = 2.6000000000000001
                        },
                        new
                        {
                            SpaceshipId = 3,
                            MaxCrewCount = 3,
                            Name = "Event Horizon",
                            OnMission = false,
                            Range = 9.9000000000000004
                        },
                        new
                        {
                            SpaceshipId = 4,
                            MaxCrewCount = 7,
                            Name = "Captain Marvel",
                            OnMission = false,
                            Range = 3.1400000000000001
                        },
                        new
                        {
                            SpaceshipId = 5,
                            MaxCrewCount = 7,
                            Name = "Lucky Tortiinn",
                            OnMission = false,
                            Range = 7.7000000000000002
                        },
                        new
                        {
                            SpaceshipId = 6,
                            MaxCrewCount = 5,
                            Name = "Battle Master",
                            OnMission = false,
                            Range = 10.0
                        },
                        new
                        {
                            SpaceshipId = 7,
                            MaxCrewCount = 3,
                            Name = "Zerash Guidah",
                            OnMission = true,
                            Range = 3.3500000000000001
                        },
                        new
                        {
                            SpaceshipId = 8,
                            MaxCrewCount = 4,
                            Name = "Ayran Hayd",
                            OnMission = false,
                            Range = 5.0999999999999996
                        },
                        new
                        {
                            SpaceshipId = 9,
                            MaxCrewCount = 7,
                            Name = "Nebukadnezar",
                            OnMission = false,
                            Range = 9.0
                        },
                        new
                        {
                            SpaceshipId = 10,
                            MaxCrewCount = 7,
                            Name = "Sifiyus Alpha Siera",
                            OnMission = false,
                            Range = 7.7000000000000002
                        });
                });

            modelBuilder.Entity("GalaxyExplorer.Entity.Voyager", b =>
                {
                    b.Property<int>("VoyagerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("FirstMissionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Grade")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MissionId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("OnMission")
                        .HasColumnType("bit");

                    b.HasKey("VoyagerId");

                    b.HasIndex("MissionId");

                    b.ToTable("Voyagers");
                });

            modelBuilder.Entity("GalaxyExplorer.Entity.Voyager", b =>
                {
                    b.HasOne("GalaxyExplorer.Entity.Mission", null)
                        .WithMany("Voyagers")
                        .HasForeignKey("MissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GalaxyExplorer.Entity.Mission", b =>
                {
                    b.Navigation("Voyagers");
                });
#pragma warning restore 612, 618
        }
    }
}
