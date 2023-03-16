﻿// <auto-generated />
using System;
using FiveWords.ChallengeResultsSaverService.PgSqlStoring;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FiveWords.ChallengeResultsSaverService.Migrations
{
    [DbContext(typeof(ChallengeResultsDbContext))]
    [Migration("20230315011452_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models.AnswerOption", b =>
                {
                    b.Property<int>("ChallengeUnitId")
                        .HasColumnType("integer")
                        .HasColumnName("unit_id");

                    b.Property<int>("Index")
                        .HasColumnType("integer")
                        .HasColumnName("index");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("text");

                    b.HasKey("ChallengeUnitId", "Index");

                    b.ToTable("answer_options");
                });

            modelBuilder.Entity("FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models.Challenge", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CompletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("completed_at");

                    b.Property<Guid>("UserGuid")
                        .HasColumnType("uuid")
                        .HasColumnName("user_guid");

                    b.HasKey("Id");

                    b.ToTable("challenges");
                });

            modelBuilder.Entity("FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models.ChallengeUnit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AnswerTimeInMilliseconds")
                        .HasColumnType("integer")
                        .HasColumnName("answer_time_ms");

                    b.Property<Guid>("ChallengeId")
                        .HasColumnType("uuid")
                        .HasColumnName("challenge_id");

                    b.Property<int>("IndexInChallenge")
                        .HasColumnType("integer")
                        .HasColumnName("index");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("question");

                    b.Property<int>("RightAnswerOptionIndex")
                        .HasColumnType("integer")
                        .HasColumnName("right_index");

                    b.Property<int>("SelectedAnswerOptionIndex")
                        .HasColumnType("integer")
                        .HasColumnName("answered_index");

                    b.HasKey("Id");

                    b.HasIndex("ChallengeId");

                    b.ToTable("units");
                });

            modelBuilder.Entity("FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models.AnswerOption", b =>
                {
                    b.HasOne("FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models.ChallengeUnit", null)
                        .WithMany("AnswerOptions")
                        .HasForeignKey("ChallengeUnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models.ChallengeUnit", b =>
                {
                    b.HasOne("FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models.Challenge", null)
                        .WithMany("ChallengeUnits")
                        .HasForeignKey("ChallengeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models.Challenge", b =>
                {
                    b.Navigation("ChallengeUnits");
                });

            modelBuilder.Entity("FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models.ChallengeUnit", b =>
                {
                    b.Navigation("AnswerOptions");
                });
#pragma warning restore 612, 618
        }
    }
}
