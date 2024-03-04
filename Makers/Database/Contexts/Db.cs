using Makers.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Makers.Database.Contexts;

public class Db : DbContext
{
    public Db(DbContextOptions<Db> options) : base(options)
    {

    }

#pragma warning disable IDE1006
    public DbSet<T_USERS> T_USERS { get; set; }
    public DbSet<T_ROLES> T_ROLES { get; set; }
    public DbSet<T_CLAIMS> T_CLAIMS { get; set; }
    public DbSet<T_MAP_ROLES_CLAIMS> T_MAP_ROLES_CLAIMS { get; set; }
    public DbSet<T_SYS_PARAMS> T_SYS_PARAMS { get; set; }
    public DbSet<T_DICT> T_DICT { get; set; }
    public DbSet<T_ROUTE> T_ROUTE { get; set; }
    public DbSet<T_MAP_ROUTE_CLAIM> T_MAP_ROUTE_CLAIM { get; set; }
    public DbSet<T_AUDIT> T_AUDIT { get; set; }
    public DbSet<T_REPORTS> T_REPORTS { get; set; }
    public DbSet<T_STORE_SETTINGS> T_STORE_SETTINGS { get; set; }
    public DbSet<T_INST> T_INST { get; set; }
    public DbSet<T_PROJECTS> T_PROJECTS { get; set; }
    public DbSet<T_TRAINING> T_TRAINING { get; set; }
    public DbSet<T_MAP_TRAINING_TRAINERS> T_MAP_TRAINING_TRAINERS { get; set; }
    public DbSet<T_TRAINERS> T_TRAINERS { get; set; }
    public DbSet<T_MAP_PROJ_INST> T_MAP_PROJ_INST { get; set; }
    public DbSet<T_STARTUPS> T_STARTUPS { get; set; }
    public DbSet<T_FOUNDERS> T_FOUNDERS { get; set; }
    public DbSet<T_EVENTS> T_EVENTS { get; set; }
    public DbSet<T_INTERNS> T_INTERNS { get; set; }
    public DbSet<T_STUDENTS> T_STUDENTS { get; set; }
    public DbSet<T_PRODUCTS> T_PRODUCTS { get; set; }
    public DbSet<T_SERVICES> T_SERVICES { get; set; }
    public DbSet<T_CONTESTS> T_CONTESTS { get; set; }
    public DbSet<T_SUCCESS_STORIES> T_SUCCESS_STORIES { get; set; }

#pragma warning restore IDE1006
}