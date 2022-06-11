﻿// <auto-generated />
using System;
using DiscordYoutubeNotify.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DiscordYoutubeNotify.Migrations
{
    [DbContext(typeof(YoutubeNotifyDbContext))]
    partial class YoutubeNotifyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.5");

            modelBuilder.Entity("DiscordYoutubeNotify.Models.Channel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ChannelId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ChannelName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LatestVideoId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("DiscordYoutubeNotify.Models.Subscription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ChannelId")
                        .HasColumnType("TEXT");

                    b.Property<string>("DiscordChannelId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("DiscordYoutubeNotify.Models.Subscription", b =>
                {
                    b.HasOne("DiscordYoutubeNotify.Models.Channel", "Channel")
                        .WithMany("Subscriptions")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("DiscordYoutubeNotify.Models.Channel", b =>
                {
                    b.Navigation("Subscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
