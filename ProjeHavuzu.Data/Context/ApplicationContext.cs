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

        public ApplicationContext(DbContextOptions<ApplicationContext> options, ICurrentUserService currentUserService = null) : base(options)
        {
            _currentUser = currentUserService;
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ProjectStudent> ProjectStudents { get; set; }



        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUser.UserId;
                    entry.Entity.Status = DataStatus.Inserted;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = _currentUser.UserId;
                    entry.Entity.Status = DataStatus.Updated;
                }
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            if (_currentUser == null)
            {
                _currentUser.UserId = Guid.Empty;
            }

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUser.UserId;
                    entry.Entity.Status = DataStatus.Inserted;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = _currentUser.UserId;
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
