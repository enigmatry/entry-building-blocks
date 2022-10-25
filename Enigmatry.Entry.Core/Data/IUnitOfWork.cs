﻿using System.Threading;
using System.Threading.Tasks;

namespace Enigmatry.Entry.Core.Data
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void CancelSaving();
    }
}