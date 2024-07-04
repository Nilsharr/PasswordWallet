﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    [DbContext(typeof(PasswordWalletDbContext))]
    partial class PasswordWalletDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Core.Entities.Credential", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)")
                        .HasColumnName("description");

                    b.Property<Guid>("FolderId")
                        .HasColumnType("uuid")
                        .HasColumnName("folder_id");

                    b.Property<string>("Password")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)")
                        .HasColumnName("password");

                    b.Property<long>("Position")
                        .HasColumnType("bigint")
                        .HasColumnName("position");

                    b.Property<string>("Username")
                        .HasMaxLength(60)
                        .HasColumnType("character varying(60)")
                        .HasColumnName("username");

                    b.Property<string>("WebAddress")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("web_address");

                    b.HasKey("Id")
                        .HasName("pk_credentials");

                    b.HasIndex("FolderId")
                        .HasDatabaseName("ix_credentials_folder_id");

                    b.ToTable("credentials", (string)null);
                });

            modelBuilder.Entity("Core.Entities.Folder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("name");

                    b.Property<long>("Position")
                        .HasColumnType("bigint")
                        .HasColumnName("position");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_folders");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_folders_user_id");

                    b.ToTable("folders", (string)null);
                });

            modelBuilder.Entity("Core.Entities.LoginHistory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<bool>("Correct")
                        .HasColumnType("boolean")
                        .HasColumnName("correct");

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(46)
                        .HasColumnType("character varying(46)")
                        .HasColumnName("ip_address");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_login_histories");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_login_histories_user_id");

                    b.ToTable("login_histories", (string)null);
                });

            modelBuilder.Entity("Core.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTimeOffset?>("LockoutTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("lockout_time");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)")
                        .HasColumnName("password_hash");

                    b.Property<string>("RefreshToken")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("refresh_token");

                    b.Property<DateTimeOffset?>("RefreshTokenExpiry")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("refresh_token_expiry");

                    b.Property<int>("SubsequentBadLogins")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("subsequent_bad_logins");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("Username")
                        .IsUnique()
                        .HasDatabaseName("ix_users_username");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("Core.Entities.Credential", b =>
                {
                    b.HasOne("Core.Entities.Folder", "Folder")
                        .WithMany("Credentials")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_credentials_folders_folder_id");

                    b.Navigation("Folder");
                });

            modelBuilder.Entity("Core.Entities.Folder", b =>
                {
                    b.HasOne("Core.Entities.User", "User")
                        .WithMany("Folders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_folders_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Core.Entities.LoginHistory", b =>
                {
                    b.HasOne("Core.Entities.User", "User")
                        .WithMany("LoginHistories")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_login_histories_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Core.Entities.Folder", b =>
                {
                    b.Navigation("Credentials");
                });

            modelBuilder.Entity("Core.Entities.User", b =>
                {
                    b.Navigation("Folders");

                    b.Navigation("LoginHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
