﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Infrastructure
{
    public class Dependency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<Dependency> b)
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Name)
                .IsRequired()
                .IsUnicode();
        }

    }
}