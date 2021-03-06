﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using RAIDAChatNode.Model;
using System;

namespace RAIDAChatNode.Migrations
{
    [DbContext(typeof(RaidaContext))]
    partial class RaidaContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("RAIDAChatNode.Model.Entity.Groups", b =>
                {
                    b.Property<Guid>("group_id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("group_name_part");

                    b.Property<bool>("one_to_one");

                    b.Property<Guid?>("organizationprivate_id");

                    b.Property<Guid?>("ownerprivate_id");

                    b.Property<string>("photo_fragment");

                    b.Property<bool>("privated");

                    b.HasKey("group_id");

                    b.HasIndex("organizationprivate_id");

                    b.HasIndex("ownerprivate_id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("RAIDAChatNode.Model.Entity.MemberInGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("groupId");

                    b.Property<Guid>("memberId");

                    b.HasKey("Id");

                    b.HasIndex("groupId");

                    b.HasIndex("memberId", "groupId")
                        .IsUnique();

                    b.ToTable("MemberInGroup");
                });

            modelBuilder.Entity("RAIDAChatNode.Model.Entity.Members", b =>
                {
                    b.Property<Guid>("private_id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("description_fragment");

                    b.Property<int>("kb_bandwidth_used");

                    b.Property<long>("last_use");

                    b.Property<string>("login");

                    b.Property<string>("nick_name");

                    b.Property<bool>("online");

                    b.Property<Guid?>("organizationprivate_id");

                    b.Property<string>("pass");

                    b.Property<byte[]>("photo_fragment");

                    b.HasKey("private_id");

                    b.HasIndex("organizationprivate_id");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("RAIDAChatNode.Model.Entity.Organizations", b =>
                {
                    b.Property<Guid>("private_id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("kb_of_credit");

                    b.Property<string>("org_name_part");

                    b.Property<Guid?>("ownerprivate_id");

                    b.Property<Guid>("public_id");

                    b.HasKey("private_id");

                    b.HasIndex("ownerprivate_id");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("RAIDAChatNode.Model.Entity.Shares", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("current_fragment");

                    b.Property<long>("death_date");

                    b.Property<byte[]>("file_data");

                    b.Property<string>("file_extention");

                    b.Property<int>("kb_size");

                    b.Property<Guid?>("ownerprivate_id");

                    b.Property<long>("sending_date");

                    b.Property<Guid?>("to_groupgroup_id");

                    b.Property<int>("total_fragment");

                    b.HasKey("id");

                    b.HasIndex("ownerprivate_id");

                    b.HasIndex("to_groupgroup_id");

                    b.ToTable("Shares");
                });

            modelBuilder.Entity("RAIDAChatNode.Model.Entity.Transactions", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("ownerprivate_id");

                    b.Property<long>("rollbackTime");

                    b.Property<string>("tableName");

                    b.Property<string>("tableRowId");

                    b.Property<Guid>("transactionId");

                    b.HasKey("Id");

                    b.HasIndex("ownerprivate_id");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("RAIDAChatNode.Model.Entity.Groups", b =>
                {
                    b.HasOne("RAIDAChatNode.Model.Entity.Organizations", "organization")
                        .WithMany("groups")
                        .HasForeignKey("organizationprivate_id");

                    b.HasOne("RAIDAChatNode.Model.Entity.Members", "owner")
                        .WithMany()
                        .HasForeignKey("ownerprivate_id");
                });

            modelBuilder.Entity("RAIDAChatNode.Model.Entity.MemberInGroup", b =>
                {
                    b.HasOne("RAIDAChatNode.Model.Entity.Groups", "group")
                        .WithMany("MemberInGroup")
                        .HasForeignKey("groupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RAIDAChatNode.Model.Entity.Members", "member")
                        .WithMany("MemberInGroup")
                        .HasForeignKey("memberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RAIDAChatNode.Model.Entity.Members", b =>
                {
                    b.HasOne("RAIDAChatNode.Model.Entity.Organizations", "organization")
                        .WithMany("members")
                        .HasForeignKey("organizationprivate_id");
                });

            modelBuilder.Entity("RAIDAChatNode.Model.Entity.Organizations", b =>
                {
                    b.HasOne("RAIDAChatNode.Model.Entity.Members", "owner")
                        .WithMany()
                        .HasForeignKey("ownerprivate_id");
                });

            modelBuilder.Entity("RAIDAChatNode.Model.Entity.Shares", b =>
                {
                    b.HasOne("RAIDAChatNode.Model.Entity.Members", "owner")
                        .WithMany()
                        .HasForeignKey("ownerprivate_id");

                    b.HasOne("RAIDAChatNode.Model.Entity.Groups", "to_group")
                        .WithMany("Shares")
                        .HasForeignKey("to_groupgroup_id");
                });

            modelBuilder.Entity("RAIDAChatNode.Model.Entity.Transactions", b =>
                {
                    b.HasOne("RAIDAChatNode.Model.Entity.Members", "owner")
                        .WithMany("Transactions")
                        .HasForeignKey("ownerprivate_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
