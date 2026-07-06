using System.Data.Common;

namespace eshop.application.Data.Repositories
{
    public abstract class BaseRepository
    {
        /// <summary>
        /// 資料庫連線物件
        /// </summary>
        protected readonly DbConnection _dbConnection;

        /// <summary>
        /// 工作單元
        /// </summary>
        /// <remarks>供取得交易使用</remarks>
        protected readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// 取得當前交易。
        /// </summary>
        protected DbTransaction? _dbTransaction => _unitOfWork.CurrentTransaction;

        /// <summary>
        /// 建構式
        /// </summary>
        protected BaseRepository(DbConnection dbConnection, IUnitOfWork unitOfWork)
        {
            _dbConnection = dbConnection;
            _unitOfWork = unitOfWork;
        }
    }
}
