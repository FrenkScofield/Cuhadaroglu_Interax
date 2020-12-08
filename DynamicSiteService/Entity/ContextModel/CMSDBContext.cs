using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public partial class CMSDBContext : DbContext
{
    public CMSDBContext()
    {
    }

    public CMSDBContext(DbContextOptions<CMSDBContext> options)
        : base(options)
    {

    }

    public virtual DbSet<Spec> Spec { get; set; }
    public virtual DbSet<SpecAttr> SpecAttr { get; set; }
    public virtual DbSet<SpecContentType> SpecContentType { get; set; }
    public virtual DbSet<SpecContentValue> SpecContentValue { get; set; }


    public virtual DbSet<FormType> FormType { get; set; }
    public virtual DbSet<Forms> Forms { get; set; }

    public virtual DbSet<User> User { get; set; }
    public virtual DbSet<Lang> Lang { get; set; }
    public virtual DbSet<ContentPage> ContentPage { get; set; }
    public virtual DbSet<ContentTypes> ContentTypes { get; set; }
    public virtual DbSet<Documents> Documents { get; set; }
    public virtual DbSet<SiteConfig> SiteConfig { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=109.228.254.40;Database=cmsdb;Username=postgres;Password=123_*1", x => x.MigrationsHistoryTable("__EFMigrationsHistory", "mySchema"));

        }
    }

    private static readonly MethodInfo ConfigureGlobalFiltersMethodInfo = typeof(CMSDBContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            ConfigureGlobalFiltersMethodInfo
                .MakeGenericMethod(entityType.ClrType)
                .Invoke(this, new object[] { modelBuilder, entityType });
        }

        modelBuilder.Entity<Spec>().HasMany(a => a.SpecAttrs).WithOne(b => b.Spec);



        modelBuilder.Entity<ContentPage>().HasOne(a => a.ThumbImage).WithOne(b => b.ThumbImage).HasForeignKey<Documents>(b => b.ThumbImageId);

        modelBuilder.Entity<ContentPage>().HasOne(a => a.Picture).WithOne(b => b.Picture).HasForeignKey<Documents>(b => b.PictureId);

        modelBuilder.Entity<ContentPage>().HasOne(a => a.BannerImage).WithOne(b => b.BannerImage).HasForeignKey<Documents>(b => b.BannerImageId);

        modelBuilder.Entity<ContentPage>().HasMany(a => a.Documents).WithOne(b => b.Document);

        modelBuilder.Entity<ContentPage>().HasMany(a => a.Gallery).WithOne(b => b.Gallery);

        modelBuilder.Entity<ContentPage>().HasMany(a => a.ContentPageChilds).WithOne(b => b.Parent);

        modelBuilder.Entity<Spec>().HasMany(a => a.SpecChilds).WithOne(b => b.Parent);


        //modelBuilder.Entity<User>().HasData(new User
        //{
        //    CreaUser = 1,
        //    CreaDate = DateTime.Now,
        //    UserName = "admin",
        //    Pass = "123_*1",
        //    Name = "admin",
        //    Surname = "admin",
        //    SexType = "admin",
        //    BirdhDay = DateTime.Now.AddYears(-28),

        //});

        //modelBuilder.Entity<Lang>().HasData(
        //new Lang
        //{
        //    CreaUser = 1,
        //    CreaDate = DateTime.Now,
        //    Name = "Türkçe",
        //    Code = "TR",
        //    IsDefault = true
        //},
        //new Lang
        //{
        //    CreaUser = 1,
        //    CreaDate = DateTime.Now,
        //    Name = "İngilizce",
        //    Code = "EN",
        //    IsDefault = false
        //});


        base.OnModelCreating(modelBuilder);
    }

    protected void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType) where TEntity : class
    {
        if (entityType.BaseType != null || !ShouldFilterEntity<TEntity>(entityType)) return;
        var filterExpression = CreateFilterExpression<TEntity>();
        if (filterExpression == null) return;
        //if (entityType.GetType().IsInterface==true)
        if (false)
            modelBuilder.Query<TEntity>().HasQueryFilter(filterExpression);
        else
            modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
    }

    protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
    {
        return typeof(IBaseModel).IsAssignableFrom(typeof(TEntity));
    }

    protected Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>() where TEntity : class
    {
        Expression<Func<TEntity, bool>> expression = null;

        if (typeof(IBaseModel).IsAssignableFrom(typeof(TEntity)))
        {
            Expression<Func<TEntity, bool>> removedFilter = e => (DateTime)((IBaseModel)e).IsDeleted == null;
            expression = expression == null ? removedFilter : CombineExpressions(expression, removedFilter);
        }

        return expression;
    }

    protected Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
    {
        return Helpers.Combine(expression1, expression2);
    }


}