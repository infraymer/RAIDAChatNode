using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RAIDAChatNode.Model.Entity;
using System.IO;
using RAIDAChatNode.DTO.Configuration;

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
            switch (MainConfig.DB.NameDB.ToLower())
            {
                case "sqlite":
                    optionsBuilder.UseSqlite(MainConfig.DB.ConnectionString);
                    break;
                case "mssql":
                    optionsBuilder.UseSqlServer(MainConfig.DB.ConnectionString);
                    break;
                case "mysql":
                    optionsBuilder.UseMySql(MainConfig.DB.ConnectionString);
                    break;
                default:
                    optionsBuilder.UseSqlite("Filename=RAIDAChat.db");
                    break;
            }
            
            //optionsBuilder.UseSqlServer(@"data source=SEREGA\SQLSERV2016;initial catalog=CloudChatPortable;persist security info=True;user id=test;password=test;multipleactiveresultsets=True;application name=EntityFramework");
            //optionsBuilder.UseSqlite("Filename=RAIDAChat.sqlite");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Members>()
                            .HasOne(m => m.organization)
                            .WithMany(o => o.members);
            modelBuilder.Entity<Members>()
                            .HasMany(m => m.Transactions)
                            .WithOne(t => t.owner)
                            .OnDelete(DeleteBehavior.Cascade); ;


            modelBuilder.Entity<Groups>()
                           .HasOne(g => g.organization)
                           .WithMany(o => o.groups);


            modelBuilder.Entity<MemberInGroup>()
                            .HasIndex(mg=> new { mg.memberId, mg.groupId }).IsUnique(true);


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
