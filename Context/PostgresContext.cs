using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PM02.Models;

namespace PM02.Context;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Measurement> Measurements { get; set; }

    public virtual DbSet<MeasurementsType> MeasurementsTypes { get; set; }

    public virtual DbSet<Meteostation> Meteostations { get; set; }

    public virtual DbSet<MeteostationsSensor> MeteostationsSensors { get; set; }

    public virtual DbSet<Sensor> Sensors { get; set; }

    public virtual DbSet<SensorsMeasurement> SensorsMeasurements { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseLazyLoadingProxies().UseNpgsql("Host=localhost;Database=postgres;Username=postgres;Password=postgres;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("PM02", "pgcrypto")
            .HasPostgresExtension("pg_catalog", "adminpack");

        modelBuilder.Entity<Measurement>(entity =>
        {
            entity.HasKey(e => e.MeasurementId).HasName("measurements_pk");

            entity.ToTable("measurements", "PM02");

            entity.Property(e => e.MeasurementId).HasColumnName("measurement_id");
            entity.Property(e => e.MeasurementTs)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("measurement_ts");
            entity.Property(e => e.MeasurementType).HasColumnName("measurement_type");
            entity.Property(e => e.MeasurementValue)
                .HasPrecision(17, 2)
                .HasColumnName("measurement_value");
            entity.Property(e => e.SensorInventoryNumber).HasColumnName("sensor_inventory_number");

            entity.HasOne(d => d.MeasurementTypeNavigation).WithMany(p => p.Measurements)
                .HasForeignKey(d => d.MeasurementType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("measurements_measurements_type_fk");

            entity.HasOne(d => d.SensorInventoryNumberNavigation).WithMany(p => p.Measurements)
                .HasForeignKey(d => d.SensorInventoryNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("measurements_meteostations_sensors_fk");
        });

        modelBuilder.Entity<MeasurementsType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("measurements_type_pk");

            entity.ToTable("measurements_type", "PM02");

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.TypeName)
                .HasColumnType("character varying")
                .HasColumnName("type_name");
            entity.Property(e => e.TypeUnits)
                .HasMaxLength(4)
                .IsFixedLength()
                .HasColumnName("type_units");
        });

        modelBuilder.Entity<Meteostation>(entity =>
        {
            entity.HasKey(e => e.StationId).HasName("meteostations_pk");

            entity.ToTable("meteostations", "PM02");

            entity.Property(e => e.StationId).HasColumnName("station_id");
            entity.Property(e => e.StationLatitude)
                .HasPrecision(5, 2)
                .HasColumnName("station_latitude");
            entity.Property(e => e.StationLongitude)
                .HasPrecision(5, 2)
                .HasColumnName("station_longitude");
            entity.Property(e => e.StationName)
                .HasColumnType("character varying")
                .HasColumnName("station_name");
        });

        modelBuilder.Entity<MeteostationsSensor>(entity =>
        {
            entity.HasKey(e => e.SensorInventoryNumber).HasName("meteostations_sensors_pk");

            entity.ToTable("meteostations_sensors", "PM02");

            entity.Property(e => e.SensorInventoryNumber)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("sensor_inventory_number");
            entity.Property(e => e.AddedTs)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("added_ts");
            entity.Property(e => e.RemovedTs)
                .HasDefaultValueSql("'9999-01-01 00:00:00'::timestamp without time zone")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("removed_ts");
            entity.Property(e => e.SensorId).HasColumnName("sensor_id");
            entity.Property(e => e.StationId).HasColumnName("station_id");

            entity.HasOne(d => d.Sensor).WithMany(p => p.MeteostationsSensors)
                .HasForeignKey(d => d.SensorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("meteostations_sensors_sensors_fk");

            entity.HasOne(d => d.Station).WithMany(p => p.MeteostationsSensors)
                .HasForeignKey(d => d.StationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("meteostations_sensors_meteostations_fk");
        });

        modelBuilder.Entity<Sensor>(entity =>
        {
            entity.HasKey(e => e.SensorId).HasName("sensors_pk");

            entity.ToTable("sensors", "PM02");

            entity.Property(e => e.SensorId).HasColumnName("sensor_id");
            entity.Property(e => e.SensorName)
                .HasColumnType("character varying")
                .HasColumnName("sensor_name");
        });

        modelBuilder.Entity<SensorsMeasurement>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("sensors_measurements", "PM02");

            entity.Property(e => e.MeasurmentFormula)
                .HasColumnType("character varying")
                .HasColumnName("measurment_formula");
            entity.Property(e => e.SensorId).HasColumnName("sensor_id");
            entity.Property(e => e.TypeId).HasColumnName("type_id");

            entity.HasOne(d => d.Sensor).WithMany()
                .HasForeignKey(d => d.SensorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sensors_measurements_sensors_fk");

            entity.HasOne(d => d.Type).WithMany()
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sensors_measurements_measurements_type_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
