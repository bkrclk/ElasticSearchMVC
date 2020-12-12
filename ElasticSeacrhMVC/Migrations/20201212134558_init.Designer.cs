﻿// <auto-generated />
using System;
using ElasticSearchMVC.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ElasticSearchMVC.Migrations
{
    [DbContext(typeof(ProductContext))]
    [Migration("20201212134558_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113");

            modelBuilder.Entity("ElasticSearchMVC.Models.ProductModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Category");

                    b.Property<string>("Description");

                    b.Property<string>("Image");

                    b.Property<string>("Price");

                    b.Property<string>("ProductCode");

                    b.Property<string>("ProductName");

                    b.Property<DateTime>("ProductionDate");

                    b.Property<int>("Quantity");

                    b.HasKey("Id");

                    b.ToTable("ProductModel");
                });
#pragma warning restore 612, 618
        }
    }
}
