﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public delegate void DbContextEventHandler(IDbContextWithSaveEvent context);

    public interface IDbContextWithSaveEvent : IDisposable
    {
        event DbContextEventHandler SaveStart;

        event DbContextEventHandler SaveCompleted;

        DbChangeTracker ChangeTracker { get; }

        Task<int> SaveChangesAsync();

        int SaveChanges();
    }
}
