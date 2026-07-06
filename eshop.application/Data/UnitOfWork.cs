using System.Data;
using System.Data.Common;

namespace eshop.application.Data
{
    /// <summary>
    /// 工作單元實作
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbConnection _dbConnection;
        private DbTransaction? _dbTransaction;
        private bool _disposed = false;

        /// <summary>
        /// 取得當前資料庫交易。
        /// </summary>
        public DbTransaction? CurrentTransaction => _dbTransaction;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbConnection"></param>
        public UnitOfWork(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// 嘗試開啟資料庫連線
        /// </summary>
        /// <returns></returns>
        public async Task TryOpenConnectionAsync()
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                await _dbConnection.OpenAsync();
            }
        }

        /// <summary>
        /// 開啟交易
        /// </summary>
        /// <remarks>不支援多重交易與分散式交易</remarks>
        public async Task BeginTransactionAsync()
        {
            await TryOpenConnectionAsync();

            if (_dbTransaction == null)
            {
                _dbTransaction = await _dbConnection.BeginTransactionAsync();
            }
        }

        /// <summary>
        /// 回退交易
        /// </summary>
        /// <returns></returns>
        public async Task RollbackTransactionAsync()
        {
            if (_dbTransaction != null)
            {
                try
                {
                    await _dbTransaction.RollbackAsync();
                }
                finally
                {
                    await _dbTransaction.DisposeAsync();
                    _dbTransaction = null;
                }
            }
        }

        /// <summary>
        /// 認可交易
        /// </summary>
        /// <returns></returns>
        public async Task CommitTransactionAsync()
        {
            //GPT建議修改 這裡不要包rollback
            if (_dbTransaction == null)
            {
                return;
            }

            await _dbTransaction.CommitAsync();

            await _dbTransaction.DisposeAsync();
            _dbTransaction = null;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 釋放由 <see cref="UnitOfWork"/> 持有的資源。
        /// </summary>
        /// <param name="disposing">
        /// true：由 <see cref="Dispose()"/> 呼叫，應釋放受控資源。
        /// false：由 finalizer 呼叫（若有），僅能釋放非受控資源。
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbTransaction?.Dispose();
                    _dbConnection?.Dispose();
                }
                _disposed = true;
            }
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 非同步釋放由 <see cref="UnitOfWork"/> 持有的資源。
        /// </summary>
        /// <param name="disposing">
        /// true：由 <see cref="DisposeAsync()"/> 呼叫，應非同步釋放受控資源。
        /// false：由 finalizer 呼叫（目前未使用）。
        /// </param>
        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_dbTransaction != null)
                    {
                        await _dbTransaction.DisposeAsync();
                    }

                    if (_dbConnection != null)
                    {
                        await _dbConnection.DisposeAsync();
                    }
                }
                _disposed = true;
            }
        }
    }
}
