﻿// <auto-generated />
using System;
using Dataport.Terminfinder.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Dataport.Terminfinder.Repository.Migrations
{
    /// <summary>
    /// DB Migration
    /// </summary>
    [DbContext(typeof(DataContext))]
    [Migration("20221129134715_CreateCustomer")]
    partial class CreateCustomer
    {
        /// <summary>
        /// BuildTargetModel
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Dataport.Terminfinder.BusinessObject.AppConfig", b =>
                {
                    b.Property<string>("Key")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("configkey");

                    b.Property<string>("Value")
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)")
                        .HasColumnName("configvalue");

                    b.HasKey("Key")
                        .HasName("appconfig_pkey");

                    b.ToTable("appconfig", "public");
                });

            modelBuilder.Entity("Dataport.Terminfinder.BusinessObject.Appointment", b =>
                {
                    b.Property<Guid>("AppointmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("appointmentid");

                    b.Property<Guid>("AdminId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("adminid");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("creationdate")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("CreatorName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("creatorname");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid")
                        .HasColumnName("customerid");

                    b.Property<string>("Description")
                        .HasMaxLength(1500)
                        .HasColumnType("character varying(1500)")
                        .HasColumnName("description");

                    b.Property<string>("Password")
                        .HasMaxLength(120)
                        .HasColumnType("character varying(120)")
                        .HasColumnName("password");

                    b.Property<string>("Place")
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)")
                        .HasColumnName("place");

                    b.Property<string>("StatusIdentifier")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("status");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)")
                        .HasColumnName("subject");

                    b.HasKey("AppointmentId")
                        .HasName("appointment_pkey");

                    b.HasIndex("AdminId")
                        .IsUnique()
                        .HasDatabaseName("appointment_adminid_ix");

                    b.HasIndex("CustomerId")
                        .HasDatabaseName("appointment_customerid_ix");

                    b.ToTable("appointment", "public");
                });

            modelBuilder.Entity("Dataport.Terminfinder.BusinessObject.Customer", b =>
                {
                    b.Property<Guid>("CustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("customerid");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("creationdate")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("customername");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("status");

                    b.HasKey("CustomerId")
                        .HasName("customer_pkey");

                    b.ToTable("customer", "public");
                });

            modelBuilder.Entity("Dataport.Terminfinder.BusinessObject.Participant", b =>
                {
                    b.Property<Guid>("ParticipantId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("participantid");

                    b.Property<Guid>("AppointmentId")
                        .HasColumnType("uuid")
                        .HasColumnName("appointmentid");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("creationdate")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid")
                        .HasColumnName("customerid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("name");

                    b.HasKey("ParticipantId")
                        .HasName("participant_pkey");

                    b.HasIndex("AppointmentId")
                        .HasDatabaseName("participant_appointmentid_ix");

                    b.HasIndex("CustomerId")
                        .HasDatabaseName("participant_customerid_ix");

                    b.ToTable("participant", "public");
                });

            modelBuilder.Entity("Dataport.Terminfinder.BusinessObject.SuggestedDate", b =>
                {
                    b.Property<Guid>("SuggestedDateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("suggesteddateid");

                    b.Property<Guid>("AppointmentId")
                        .HasColumnType("uuid")
                        .HasColumnName("appointmentid");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("creationdate")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid")
                        .HasColumnName("customerid");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("date")
                        .HasColumnName("enddate");

                    b.Property<DateTimeOffset?>("EndTime")
                        .HasColumnType("time with time zone")
                        .HasColumnName("endtime");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("date")
                        .HasColumnName("startdate");

                    b.Property<DateTimeOffset?>("StartTime")
                        .HasColumnType("time with time zone")
                        .HasColumnName("starttime");

                    b.HasKey("SuggestedDateId")
                        .HasName("suggesteddate_pkey");

                    b.HasIndex("AppointmentId")
                        .HasDatabaseName("suggesteddate_appointmentid_ix");

                    b.HasIndex("CustomerId")
                        .HasDatabaseName("suggestedDate_customerid_ix");

                    b.HasIndex("EndDate")
                        .HasDatabaseName("suggesteddate_enddate_ix");

                    b.HasIndex("StartDate")
                        .HasDatabaseName("suggesteddate_startdate_ix");

                    b.ToTable("suggesteddate", "public");
                });

            modelBuilder.Entity("Dataport.Terminfinder.BusinessObject.Voting", b =>
                {
                    b.Property<Guid>("VotingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("votingid");

                    b.Property<Guid>("AppointmentId")
                        .HasColumnType("uuid")
                        .HasColumnName("appointmentid");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("creationdate")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid")
                        .HasColumnName("customerid");

                    b.Property<Guid>("ParticipantId")
                        .HasColumnType("uuid")
                        .HasColumnName("participantid");

                    b.Property<string>("StatusIdentifier")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("status");

                    b.Property<Guid>("SuggestedDateId")
                        .HasColumnType("uuid")
                        .HasColumnName("suggesteddateid");

                    b.HasKey("VotingId")
                        .HasName("voting_pkey");

                    b.HasIndex("AppointmentId")
                        .HasDatabaseName("voting_appointmentid_ix");

                    b.HasIndex("CustomerId")
                        .HasDatabaseName("voting_customerid_ix");

                    b.HasIndex("ParticipantId")
                        .HasDatabaseName("voting_participantid_ix");

                    b.HasIndex("SuggestedDateId")
                        .HasDatabaseName("voting_suggesteddateid_ix");

                    b.ToTable("voting", "public");
                });

            modelBuilder.Entity("Dataport.Terminfinder.BusinessObject.Appointment", b =>
                {
                    b.HasOne("Dataport.Terminfinder.BusinessObject.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("appointment_customerid_fkey");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Dataport.Terminfinder.BusinessObject.Participant", b =>
                {
                    b.HasOne("Dataport.Terminfinder.BusinessObject.Appointment", "Appointment")
                        .WithMany("Participants")
                        .HasForeignKey("AppointmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("participant_appointmentid_fkey");

                    b.HasOne("Dataport.Terminfinder.BusinessObject.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("participant_customerid_fkey");

                    b.Navigation("Appointment");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Dataport.Terminfinder.BusinessObject.SuggestedDate", b =>
                {
                    b.HasOne("Dataport.Terminfinder.BusinessObject.Appointment", "Appointment")
                        .WithMany("SuggestedDates")
                        .HasForeignKey("AppointmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("suggesteddate_appointmentid_fkey");

                    b.HasOne("Dataport.Terminfinder.BusinessObject.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("suggesteddate_customerid_fkey");

                    b.Navigation("Appointment");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Dataport.Terminfinder.BusinessObject.Voting", b =>
                {
                    b.HasOne("Dataport.Terminfinder.BusinessObject.Appointment", "Appointment")
                        .WithMany()
                        .HasForeignKey("AppointmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("voting_appointmentid_fkey");

                    b.HasOne("Dataport.Terminfinder.BusinessObject.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("voting_customerid_fkey");

                    b.HasOne("Dataport.Terminfinder.BusinessObject.Participant", "Participant")
                        .WithMany("Votings")
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("voting_participantid_fkey");

                    b.HasOne("Dataport.Terminfinder.BusinessObject.SuggestedDate", "SuggestedDate")
                        .WithMany("Votings")
                        .HasForeignKey("SuggestedDateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("voting_suggesteddateid_fkey");

                    b.Navigation("Appointment");

                    b.Navigation("Customer");

                    b.Navigation("Participant");

                    b.Navigation("SuggestedDate");
                });

            modelBuilder.Entity("Dataport.Terminfinder.BusinessObject.Appointment", b =>
                {
                    b.Navigation("Participants");

                    b.Navigation("SuggestedDates");
                });

            modelBuilder.Entity("Dataport.Terminfinder.BusinessObject.Participant", b =>
                {
                    b.Navigation("Votings");
                });

            modelBuilder.Entity("Dataport.Terminfinder.BusinessObject.SuggestedDate", b =>
                {
                    b.Navigation("Votings");
                });
#pragma warning restore 612, 618
        }
    }
}
