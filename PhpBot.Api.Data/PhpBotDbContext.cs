using Microsoft.EntityFrameworkCore;
using PhpBot.Api.Data.Entities;

namespace PhpBot.Api.Data
{
    public class PhpBotDbContext : DbContext
    {
        public virtual DbSet<TelegramUser> TelegramUsers { get; set; }
        public virtual DbSet<Upload> Uploads { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public PhpBotDbContext(DbContextOptions<PhpBotDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TelegramUser>(entity =>
            {
                entity.ToTable("TelegramUsers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TelegramUsername).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
            });
            modelBuilder.Entity<TelegramUser>().Property(u => u.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Upload>(entity =>
            {
                entity.ToTable("Uploads");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.AppName).IsRequired();
                entity.Property(e => e.AppBundle).IsRequired();
                entity.Property(e => e.SFTPHost).IsRequired();
                entity.Property(e => e.SFTPLogin).IsRequired();
                entity.Property(e => e.SFTPFilePath).IsRequired();
                entity.Property(e => e.UploadTime).IsRequired();
                entity.Property(e => e.SecretKey).IsRequired();
                entity.Property(e => e.SecretKeyParam).IsRequired();
            });
            modelBuilder.Entity<Upload>().Property(u => u.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Login).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.AccessLevel).IsRequired();
            });
            modelBuilder.Entity<User>().Property(u => u.Id).ValueGeneratedOnAdd();
        }
    }
}
