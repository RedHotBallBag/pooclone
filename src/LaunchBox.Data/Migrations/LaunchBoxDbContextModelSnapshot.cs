using LaunchBox.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace LaunchBox.Data.Migrations
{
    [DbContext(typeof(LaunchBoxDbContext))]
    partial class LaunchBoxDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("LaunchBox.Core.Models.AppSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AutoDownloadMedia")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AutoDownloadMetadata")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DefaultViewMode")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("EnableAnimations")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GridItemSize")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("LastSelectedPlatformId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxRecentGames")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MediaFolder")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("RememberLastFilter")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("StartMaximized")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Theme")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AppSettings");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AutoDownloadMedia = true,
                            AutoDownloadMetadata = true,
                            DefaultViewMode = 0,
                            EnableAnimations = true,
                            GridItemSize = 200,
                            MaxRecentGames = 20,
                            MediaFolder = "Media",
                            RememberLastFilter = true,
                            StartMaximized = false,
                            Theme = "Light",
                            UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
