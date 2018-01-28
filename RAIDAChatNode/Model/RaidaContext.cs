using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RAIDAChatNode.Model.Entity;

namespace RAIDAChatNode.Model
{
    public class RaidaContext: DbContext
    {
        public RaidaContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<Members> Members { get; set; }
        public DbSet<Groups> Groups { get; set; }
        public DbSet<MemberInGroup> MemberInGroup { get; set; }
        public DbSet<Organizations> Organizations { get; set; }
        public DbSet<Shares> Shares { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { 
            optionsBuilder.UseSqlite("Filename=RAIDAChat.sqlite");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Members>()
                            .HasOne(m => m.organization)
                            .WithMany(o => o.members);

            modelBuilder.Entity<Groups>()
                           .HasOne(g => g.organization)
                           .WithMany(o => o.groups);


            modelBuilder.Entity<MemberInGroup>()
                            .HasKey(mg => new { mg.memberId, mg.groupId, mg.Id });


            modelBuilder.Entity<MemberInGroup>()
                            .HasOne(mg => mg.member)
                            .WithMany(m => m.MemberInGroup)
                            .HasForeignKey(mg => mg.memberId);

            modelBuilder.Entity<MemberInGroup>()
                            .HasOne(mg => mg.group)
                            .WithMany(g => g.MemberInGroup)
                            .HasForeignKey(mg => mg.groupId);

        }

    }
}
