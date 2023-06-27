using CSTScheduling.Data.Models;
using CSTScheduling.Data.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CSTScheduling.Data.Context
{
    public class CstScheduleDbContext : DbContext
    {
        public CstScheduleDbContext(DbContextOptions<CstScheduleDbContext> options) 
            : base(options)
        {
            Debug.WriteLine($"Context Created ^_^ {this}");
        }


        public DbSet<Room> Room { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Instructor> Instructor { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<Semester> Semester { get; set; }
        public DbSet<CISR> CISR { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            /*modelBuilder.Entity<Instructor>()
                .HasAlternateKey(b => b.email);*/
            //aight so this completely destroys the purpose of not having the email as PK... now it can't be changed

            //linking cisr to course
            modelBuilder.Entity<CISR>()
                .HasOne<Course>(e => e.course)
                .WithMany(e => e.cisrList);

            //linking cisr to primary instructor
            modelBuilder.Entity<CISR>()
                .HasOne<Instructor>(e => e.primaryInstructor)
                .WithMany(e => e.cisrPrimaryList);

            //linking cisr to secondary instructor
            modelBuilder.Entity<CISR>()
                .HasOne<Instructor>(e => e.secondaryInstructor)
                .WithMany(e => e.cisrSecondaryList);

            //linking cisr to semester
            modelBuilder.Entity<CISR>()
                .HasOne<Semester>(e => e.semester)
                .WithMany(e => e.cisrList);

            //linking cisr to room
            modelBuilder.Entity<CISR>()
                .HasOne<Room>(e => e.room)
                .WithMany(e => e.cisrList);

        }

        /// <summary>
        /// Dispose pattern.
        /// </summary>
        /// 
        public override void Dispose()
        {
            Debug.WriteLine($"{ContextId} context disposed.");
            base.Dispose();
        }
    }
}
