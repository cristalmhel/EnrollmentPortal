using EnrollmentPortal.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentPortal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Entities or Database Tables
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<SubjectFile> SubjectFiles { get; set; }
        public DbSet<StudentFile> StudentFiles { get; set; }
        public DbSet<SubjectPreqFile> SubjectPreqFiles { get; set; }
        public DbSet<SubjectSchedFile> SubjectSchedFiles { get; set; }
        public DbSet<EnrollmentHeaderFile> EnrollmentHeaderFiles { get; set; }
        public DbSet<EnrollmentDetailFile> EnrollmentDetailFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define primary key for StudentFile
            modelBuilder.Entity<StudentFile>()
                .HasKey(s => s.StudId);

            // EnrollmentDetailFile relationships
            modelBuilder.Entity<EnrollmentDetailFile>()
                .HasOne(ed => ed.EnrollmentHeaderFile)
                .WithMany(eh => eh.EnrollmentDetailFiles)
                .HasForeignKey(ed => ed.EnrollmentHeaderFileId)
                .IsRequired();

            modelBuilder.Entity<EnrollmentDetailFile>()
                .HasOne(ed => ed.SubjectFile)
                .WithMany(sf => sf.EnrollmentDetailFiles)
                .HasForeignKey(ed => ed.SubjectFileId)
                .IsRequired();

            modelBuilder.Entity<EnrollmentDetailFile>()
                .HasOne(ed => ed.SubjectSchedFile)
                .WithMany(ssf => ssf.EnrollmentDetailFiles)
                .HasForeignKey(ed => ed.SubjectSchedFileId)
                .IsRequired();

            // StudentFile relationships
            modelBuilder.Entity<StudentFile>()
                .HasOne(s => s.Course)
                .WithMany(c => c.StudentFiles)
                .HasForeignKey(s => s.CourseId)
                .IsRequired();

            modelBuilder.Entity<StudentFile>()
                .HasMany(s => s.EnrollmentHeaderFiles)
                .WithOne(eh => eh.StudentFile)
                .HasForeignKey(eh => eh.StudentFileId)
                .IsRequired();

            // SubjectFile relationships
            modelBuilder.Entity<SubjectFile>()
                .HasOne(s => s.Course)
                .WithMany(c => c.SubjectFiles)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // Optional delete behavior

            modelBuilder.Entity<SubjectFile>()
                .HasMany(s => s.SubjectPreqFile)
                .WithOne(sp => sp.SubjectFile)
                .HasForeignKey(sp => sp.SubjectFileId)
                .OnDelete(DeleteBehavior.Cascade); // Optional delete behavior

            modelBuilder.Entity<SubjectFile>()
                .HasMany(s => s.SubjectSchedFile)
                .WithOne(ss => ss.SubjectFile)
                .HasForeignKey(ss => ss.SubjectFileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubjectFile>()
                .HasMany(s => s.EnrollmentDetailFiles)
                .WithOne(ed => ed.SubjectFile)
                .HasForeignKey(ed => ed.SubjectFileId)
                .OnDelete(DeleteBehavior.Restrict); // Optional delete behavior

            // SubjectPreqFile relationships
            modelBuilder.Entity<SubjectPreqFile>()
                .HasOne(spf => spf.SubjectFile)
                .WithMany() // No navigation property in SubjectFile
                .HasForeignKey(spf => spf.SubjectFileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubjectPreqFile>()
                .HasOne(spf => spf.PrerequisiteSubject)
                .WithMany() // No navigation property in SubjectFile
                .HasForeignKey(spf => spf.PrerequisiteSubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
