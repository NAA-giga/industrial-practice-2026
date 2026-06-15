using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace выгрузка_данных_о_проведение_олимпиады.Models;

public partial class School_OlympiadDBContext : DbContext
{
    public School_OlympiadDBContext()
    {
    }

    public School_OlympiadDBContext(DbContextOptions<School_OlympiadDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Класс> Классs { get; set; }

    public virtual DbSet<Образовательное_учереждение> Образовательное_учереждениеs { get; set; }

    public virtual DbSet<Обучающиеся> Обучающиесяs { get; set; }

    public virtual DbSet<Олимпиада> Олимпиадаs { get; set; }

    public virtual DbSet<Сведенье_о_обучающиеся> Сведенье_о_обучающиесяs { get; set; }

    public virtual DbSet<Сведенье_о_учителе> Сведенье_о_учителеs { get; set; }

    public virtual DbSet<Участие_в_олимпиаде> Участие_в_олимпиадеs { get; set; }

    public virtual DbSet<Учитель> Учительs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=School_OlympiadDB;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Класс>(entity =>
        {
            entity.ToTable("Класс");

            entity.Property(e => e.Цифра_класса).HasMaxLength(3);
        });

        modelBuilder.Entity<Образовательное_учереждение>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_Образовательное_организация");

            entity.ToTable("Образовательное_учереждение");

            entity.Property(e => e.Адрес)
                .HasMaxLength(100)
                .IsFixedLength();
        });

        modelBuilder.Entity<Обучающиеся>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_Школьник");

            entity.ToTable("Обучающиеся");

            entity.Property(e => e.Имя).HasMaxLength(50);
            entity.Property(e => e.Отчеcтво).HasMaxLength(50);
            entity.Property(e => e.Фамилия).HasMaxLength(50);

            entity.HasOne(d => d.Класс).WithMany(p => p.Обучающиесяs)
                .HasForeignKey(d => d.Класс_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Школьник_Класс");

            entity.HasOne(d => d.Образовательное_учереждение).WithMany(p => p.Обучающиесяs)
                .HasForeignKey(d => d.Образовательное_учереждение_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Обучающиеся_Образовательное_учереждение");
        });

        modelBuilder.Entity<Олимпиада>(entity =>
        {
            entity.ToTable("Олимпиада");

            entity.Property(e => e.Предмет).HasMaxLength(30);
        });

        modelBuilder.Entity<Сведенье_о_обучающиеся>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Сведенье_о_обучающиеся");

            entity.Property(e => e.Класс).HasMaxLength(3);
            entity.Property(e => e.Количество_участий_в_олимпиаде).HasColumnName("Количество участий в олимпиаде");
            entity.Property(e => e.Учитель).HasMaxLength(62);
            entity.Property(e => e.ФИО).HasMaxLength(152);
        });

        modelBuilder.Entity<Сведенье_о_учителе>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Сведенье_о_учителе");

            entity.Property(e => e.Количество_участий_в_олимпиаде_от_учеников).HasColumnName("Количество участий в олимпиаде от учеников");
            entity.Property(e => e.ФИО_учителя)
                .HasMaxLength(62)
                .HasColumnName("ФИО учителя");
        });

        modelBuilder.Entity<Участие_в_олимпиаде>(entity =>
        {
            entity.ToTable("Участие_в_олимпиаде");

            entity.Property(e => e.Результат_участия).HasMaxLength(10);

            entity.HasOne(d => d.Обучающиеся).WithMany(p => p.Участие_в_олимпиадеs)
                .HasForeignKey(d => d.Обучающиеся_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Участие_в_олимпиаде_Школьник");

            entity.HasOne(d => d.Олимпиада).WithMany(p => p.Участие_в_олимпиадеs)
                .HasForeignKey(d => d.Олимпиада_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Участие_в_олимпиаде_Олимпиада");

            entity.HasOne(d => d.Учитель).WithMany(p => p.Участие_в_олимпиадеs)
                .HasForeignKey(d => d.Учитель_ID)
                .HasConstraintName("FK_Участие_Учитель");
        });

        modelBuilder.Entity<Учитель>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_Классный руководитель");

            entity.ToTable("Учитель");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Имя).HasMaxLength(20);
            entity.Property(e => e.Отчество).HasMaxLength(20);
            entity.Property(e => e.Телефон).HasMaxLength(11);
            entity.Property(e => e.Фамилия).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
