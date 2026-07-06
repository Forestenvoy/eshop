using System.Data.Common;

namespace eshop.application.Data
{
    public interface IUnitOfWork : IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// 當前資料庫交易
        /// </summary>
        DbTransaction? CurrentTransaction { get; }

        /// <summary>
        /// 開啟交易
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// 提交交易
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// 回滾交易
        /// </summary>
        Task RollbackTransactionAsync();
    }
}
