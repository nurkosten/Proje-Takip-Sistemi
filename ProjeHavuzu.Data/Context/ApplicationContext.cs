using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Entites.Common;
using ProjeHavuzu.Data.Entites.Enums;
using ProjeHavuzu.Data.Entites.Identity;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Context
{
    public class ApplicationContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        private readonly ICurrentUserService _currentUser;

        public ApplicationContext(DbContextOptions<ApplicationContext> options, ICurrentUserService currentUserService) : base(options)
        {
            _currentUser = currentUserService;
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ProjectStudent> ProjectStudents { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<ProjectPhase> ProjectPhases { get; set; }
        public DbSet<ProjectLanguage> ProjectLanguages { get; set; }
        public DbSet<ProjectRequest> ProjectRequests { get; set; }
        public DbSet<ProjectSubmission> ProjectSubmissions { get; set; }



        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            var currentUserId = _currentUser?.UserId;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    // Eğer CreatedBy daha önce atanmadıysa (veya boşsa) mevcut kullanıcıyı ata
                    if (!entry.Entity.CreatedBy.HasValue || entry.Entity.CreatedBy == Guid.Empty)
                    {
                        entry.Entity.CreatedBy = currentUserId;
                    }
                    entry.Entity.Status = DataStatus.Inserted;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = currentUserId;
                    entry.Entity.Status = DataStatus.Updated;
                }
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            var currentUserId = _currentUser?.UserId;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    // Eğer CreatedBy daha önce atanmadıysa (veya boşsa) mevcut kullanıcıyı ata
                    if (!entry.Entity.CreatedBy.HasValue || entry.Entity.CreatedBy == Guid.Empty)
                    {
                        entry.Entity.CreatedBy = currentUserId;
                    }
                    entry.Entity.Status = DataStatus.Inserted;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = currentUserId;
                    entry.Entity.Status = DataStatus.Updated;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasOne(u => u.Faculty)
                .WithMany(f => f.AppUsers)
                .HasForeignKey(u => u.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);
        }



    }


}
