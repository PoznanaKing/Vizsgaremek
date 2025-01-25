using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Backend_PM_Project.Models;

public partial class PmProjectDatabaseContext : DbContext
{
    public PmProjectDatabaseContext()
    {
    }

    public PmProjectDatabaseContext(DbContextOptions<PmProjectDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MessageContent> MessageContents { get; set; }

    public virtual DbSet<PlaceOwnerTable> PlaceOwnerTables { get; set; }

    public virtual DbSet<PlaceTable> PlaceTables { get; set; }

    public virtual DbSet<PlaceUserConnector> PlaceUserConnectors { get; set; }

    public virtual DbSet<PostTable> PostTables { get; set; }

    public virtual DbSet<TrainerTable> TrainerTables { get; set; }

    public virtual DbSet<TrainerUserMessageConnector> TrainerUserMessageConnectors { get; set; }

    public virtual DbSet<UserTable> UserTables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost;database=pm_project_database;user=root;password=;sslmode=none;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MessageContent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("message_content");

            entity.HasIndex(e => e.ChatId, "chat_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChatId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("chat_id");
            entity.Property(e => e.MessageContent1)
                .HasMaxLength(200)
                .IsFixedLength()
                .HasColumnName("message_content");
            entity.Property(e => e.MessageSenderId).HasColumnName("message_sender_id");
            entity.Property(e => e.MessageSentTime)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime")
                .HasColumnName("message_sent_time");

            entity.HasOne(d => d.Chat).WithMany(p => p.MessageContents)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("message_content_ibfk_1");
        });

        modelBuilder.Entity<PlaceOwnerTable>(entity =>
        {
            entity.HasKey(e => e.OwnerId).HasName("PRIMARY");

            entity.ToTable("place_owner_table");

            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.OwnerEmail)
                .HasMaxLength(40)
                .IsFixedLength()
                .HasColumnName("owner_email");
            entity.Property(e => e.OwnerName)
                .HasMaxLength(40)
                .IsFixedLength()
                .HasColumnName("owner_name");
            entity.Property(e => e.OwnerPassword)
                .HasMaxLength(40)
                .IsFixedLength()
                .HasColumnName("owner_password");
            entity.Property(e => e.Verified)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("verified");
        });

        modelBuilder.Entity<PlaceTable>(entity =>
        {
            entity.HasKey(e => e.Placeid).HasName("PRIMARY");

            entity.ToTable("place_table");

            entity.HasIndex(e => e.OwnerId, "owner_id");

            entity.HasIndex(e => new { e.TrainerId, e.OwnerId }, "trainer_id");

            entity.Property(e => e.Placeid).HasColumnName("placeid");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasDefaultValueSql("'NULL'")
                .IsFixedLength()
                .HasColumnName("description");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.PlaceName)
                .HasMaxLength(40)
                .IsFixedLength()
                .HasColumnName("place_name");
            entity.Property(e => e.PostalCode)
                .HasColumnType("int(11)")
                .HasColumnName("postal_code");
            entity.Property(e => e.Rating)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("rating");
            entity.Property(e => e.StoryLevle)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("story_levle");
            entity.Property(e => e.StreetName)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("street_name");
            entity.Property(e => e.TownName)
                .HasMaxLength(40)
                .IsFixedLength()
                .HasColumnName("town_name");
            entity.Property(e => e.TrainerId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("trainer_id");

            entity.HasOne(d => d.Owner).WithMany(p => p.PlaceTables)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("place_table_ibfk_1");

            entity.HasOne(d => d.Trainer).WithMany(p => p.PlaceTables)
                .HasForeignKey(d => d.TrainerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("place_table_ibfk_2");
        });

        modelBuilder.Entity<PlaceUserConnector>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("place_user_connector");

            entity.HasIndex(e => e.Placeid, "placeid");

            entity.HasIndex(e => new { e.Userid, e.Placeid }, "userid");

            entity.Property(e => e.Placeid)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("placeid");
            entity.Property(e => e.Userid)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("userid");

            entity.HasOne(d => d.Place).WithMany()
                .HasForeignKey(d => d.Placeid)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("place_user_connector_ibfk_2");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("place_user_connector_ibfk_1");
        });

        modelBuilder.Entity<PostTable>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PRIMARY");

            entity.ToTable("post_table");

            entity.HasIndex(e => e.Userid, "userid");

            entity.Property(e => e.PostId).HasColumnName("postId");
            entity.Property(e => e.PostDescription)
                .HasMaxLength(100)
                .HasDefaultValueSql("'NULL'")
                .IsFixedLength()
                .HasColumnName("postDescription");
            entity.Property(e => e.PostImage)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("postImage");
            entity.Property(e => e.PostTitle)
                .HasMaxLength(40)
                .IsFixedLength()
                .HasColumnName("postTitle");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.PostTables)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("post_table_ibfk_1");
        });

        modelBuilder.Entity<TrainerTable>(entity =>
        {
            entity.HasKey(e => e.TrainerId).HasName("PRIMARY");

            entity.ToTable("trainer_table");

            entity.Property(e => e.TrainerId).HasColumnName("trainer_id");
            entity.Property(e => e.TrainerEmail)
                .HasMaxLength(40)
                .IsFixedLength()
                .HasColumnName("trainer_email");
            entity.Property(e => e.TrainerName)
                .HasMaxLength(40)
                .IsFixedLength()
                .HasColumnName("trainer_name");
            entity.Property(e => e.TrainerPassword)
                .HasMaxLength(40)
                .IsFixedLength()
                .HasColumnName("trainer_password");
            entity.Property(e => e.Verified)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("verified");
        });

        modelBuilder.Entity<TrainerUserMessageConnector>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("PRIMARY");

            entity.ToTable("trainer_user_message_connector");

            entity.HasIndex(e => e.TrainerId, "trainer_id");

            entity.HasIndex(e => new { e.UserId, e.TrainerId }, "user_id");

            entity.Property(e => e.ChatId).HasColumnName("chat_id");
            entity.Property(e => e.TrainerId).HasColumnName("trainer_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Trainer).WithMany(p => p.TrainerUserMessageConnectors)
                .HasForeignKey(d => d.TrainerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("trainer_user_message_connector_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.TrainerUserMessageConnectors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("trainer_user_message_connector_ibfk_1");
        });

        modelBuilder.Entity<UserTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_table");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(40)
                .IsFixedLength()
                .HasColumnName("email");
            entity.Property(e => e.Username)
                .HasMaxLength(40)
                .IsFixedLength()
                .HasColumnName("username");
            entity.Property(e => e.Userpassword)
                .HasMaxLength(40)
                .IsFixedLength()
                .HasColumnName("userpassword");
            entity.Property(e => e.Verified)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("verified");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
