using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DACN_N3.Data;

public partial class MovieDbContext : DbContext
{
    public MovieDbContext()
    {
    }

    public MovieDbContext(DbContextOptions<MovieDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Advertisement> Advertisements { get; set; }

    public virtual DbSet<CinemaTicket> CinemaTickets { get; set; }

    public virtual DbSet<Episode> Episodes { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Season> Seasons { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<ShowTime> ShowTimes { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }

    public virtual DbSet<ViewHistory> ViewHistories { get; set; }

    public virtual DbSet<Watchlist> Watchlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-GI0R0OL;Initial Catalog=MovieDB;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Advertisement>(entity =>
        {
            entity.HasKey(e => e.AdId).HasName("PK__Advertis__7130D58E3FADC2BB");

            entity.Property(e => e.AdId).HasColumnName("AdID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("ImageURL");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CinemaTicket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__CinemaTi__712CC6270AD54C2E");

            entity.Property(e => e.TicketId).HasColumnName("TicketID");
            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MovieId).HasColumnName("MovieID");
            entity.Property(e => e.SeatId).HasColumnName("SeatID");
            entity.Property(e => e.SeatNumber).HasMaxLength(10);
            entity.Property(e => e.ShowTime).HasColumnType("datetime");
            entity.Property(e => e.TicketPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Movie).WithMany(p => p.CinemaTickets)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK__CinemaTic__Movie__6A30C649");

            entity.HasOne(d => d.Seat).WithMany(p => p.CinemaTickets)
                .HasForeignKey(d => d.SeatId)
                .HasConstraintName("FK__CinemaTic__SeatI__6C190EBB");

            entity.HasOne(d => d.User).WithMany(p => p.CinemaTickets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__CinemaTic__UserI__6B24EA82");
        });

        modelBuilder.Entity<Episode>(entity =>
        {
            entity.HasKey(e => e.EpisodeId).HasName("PK__Episodes__AC6676153E1D2EEE");

            entity.Property(e => e.EpisodeId).HasColumnName("EpisodeID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.SeasonId).HasColumnName("SeasonID");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(500)
                .HasColumnName("VideoURL");

            entity.HasOne(d => d.Season).WithMany(p => p.Episodes)
                .HasForeignKey(d => d.SeasonId)
                .HasConstraintName("FK__Episodes__Season__5EBF139D");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK__Genres__0385055E4B1A1938");

            entity.Property(e => e.GenreId).HasColumnName("GenreID");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK__Movies__4BD2943A97D8A804");

            entity.Property(e => e.MovieId).HasColumnName("MovieID");
            entity.Property(e => e.AgeRating).HasMaxLength(10);
            entity.Property(e => e.Cast).HasMaxLength(500);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Director).HasMaxLength(100);
            entity.Property(e => e.HorizontalPoster).HasMaxLength(555);
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.Poster).HasMaxLength(555);
            entity.Property(e => e.ScreeningOrComing).HasDefaultValue(false);
            entity.Property(e => e.TheatricalScreening).HasDefaultValue(false);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Trailer).HasMaxLength(555);

            entity.HasMany(d => d.Genres).WithMany(p => p.Movies)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieGenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MovieGenr__Genre__440B1D61"),
                    l => l.HasOne<Movie>().WithMany()
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MovieGenr__Movie__4316F928"),
                    j =>
                    {
                        j.HasKey("MovieId", "GenreId").HasName("PK__MovieGen__BBEAC46F041932D5");
                        j.ToTable("MovieGenres");
                        j.IndexerProperty<int>("MovieId").HasColumnName("MovieID");
                        j.IndexerProperty<int>("GenreId").HasColumnName("GenreID");
                    });
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79AEE1AA56D9");

            entity.Property(e => e.ReviewId).HasColumnName("ReviewID");
            entity.Property(e => e.Comment).HasMaxLength(255);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MovieId).HasColumnName("MovieID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Movie).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK__Reviews__MovieID__48CFD27E");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Reviews__UserID__49C3F6B7");
        });

        modelBuilder.Entity<Season>(entity =>
        {
            entity.HasKey(e => e.SeasonId).HasName("PK__Seasons__C1814E18C8B5E609");

            entity.Property(e => e.SeasonId).HasColumnName("SeasonID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.MovieId).HasColumnName("MovieID");

            entity.HasOne(d => d.Movie).WithMany(p => p.Seasons)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK__Seasons__MovieID__5BE2A6F2");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("PK__Seats__311713D35EA22033");

            entity.Property(e => e.SeatId).HasColumnName("SeatID");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.IsVip)
                .HasDefaultValue(false)
                .HasColumnName("IsVIP");
            entity.Property(e => e.SeatNumber).HasMaxLength(10);
            entity.Property(e => e.ShowTimeId).HasColumnName("ShowTimeID");

            entity.HasOne(d => d.ShowTime).WithMany(p => p.Seats)
                .HasForeignKey(d => d.ShowTimeId)
                .HasConstraintName("FK__Seats__ShowTimeI__66603565");
        });

        modelBuilder.Entity<ShowTime>(entity =>
        {
            entity.HasKey(e => e.ShowTimeId).HasName("PK__ShowTime__DF1BC9FFE40B12FA");

            entity.Property(e => e.ShowTimeId).HasColumnName("ShowTimeID");
            entity.Property(e => e.CinemaRoom).HasMaxLength(50);
            entity.Property(e => e.MovieId).HasColumnName("MovieID");

            entity.HasOne(d => d.Movie).WithMany(p => p.ShowTimes)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK__ShowTimes__Movie__619B8048");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__Subscrip__9A2B24BDB5EA01DF");

            entity.Property(e => e.SubscriptionId).HasColumnName("SubscriptionID");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC850C4CBB");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(e => e.UserSubscriptionId).HasName("PK__UserSubs__D1FD775C5F3F8BD5");

            entity.Property(e => e.UserSubscriptionId).HasColumnName("UserSubscriptionID");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SubscriptionId).HasColumnName("SubscriptionID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Subscription).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("FK__UserSubsc__Subsc__59063A47");

            entity.HasOne(d => d.User).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserSubsc__UserI__5812160E");
        });

        modelBuilder.Entity<ViewHistory>(entity =>
        {
            entity.HasKey(e => e.ViewId).HasName("PK__ViewHist__1E371C16FE87977D");

            entity.ToTable("ViewHistory");

            entity.Property(e => e.ViewId).HasColumnName("ViewID");
            entity.Property(e => e.MovieId).HasColumnName("MovieID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.WatchedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Movie).WithMany(p => p.ViewHistories)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK__ViewHisto__Movie__5441852A");

            entity.HasOne(d => d.User).WithMany(p => p.ViewHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ViewHisto__UserI__534D60F1");
        });

        modelBuilder.Entity<Watchlist>(entity =>
        {
            entity.HasKey(e => e.WatchlistId).HasName("PK__Watchlis__48DEAA2BA7D1DA96");

            entity.ToTable("Watchlist");

            entity.Property(e => e.WatchlistId).HasColumnName("WatchlistID");
            entity.Property(e => e.AddedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MovieId).HasColumnName("MovieID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Movie).WithMany(p => p.Watchlists)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK__Watchlist__Movie__4E88ABD4");

            entity.HasOne(d => d.User).WithMany(p => p.Watchlists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Watchlist__UserI__4D94879B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
