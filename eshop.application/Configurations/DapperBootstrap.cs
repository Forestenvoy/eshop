using Dapper;
using eshop.application.Data;
using Serilog;
using System.Reflection;

namespace eshop.application.Configurations
{
    /// <summary>
    /// Dapper 初始化工具類別
    /// </summary>
    /// <remarks>
    /// <para>1. 統一管理 Dapper 自訂映射（ColumnAttribute）</para>
    /// <para>2. 註冊全域 TypeHandler</para>
    /// </remarks>
    public static class DapperBootstrap
    {
        /// <summary>
        /// 初始化 Dapper 設定
        /// </summary>
        public static void Init()
        {
            RegisterTypeMaps();

            Log.Information("DapperBootstrap initialized.");
        }

        /// <summary>
        /// 註冊 Model 與資料庫欄位對應規則（ColumnAttribute）
        /// </summary>
        /// <remarks>
        /// 掃描當前 Assembly 中所有符合 Models namespace 的 entity class，並套用 ColumnAttributeTypeMapper，使 Dapper 支援欄位映射
        /// </remarks>
        private static void RegisterTypeMaps()
        {
            var entityTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass
                    && !t.IsAbstract
                    && t.Namespace != null
                    && t.Namespace.StartsWith("eshop.application.Models"));

            foreach (var type in entityTypes)
            {
                var mapperType = typeof(ColumnAttributeTypeMapper<>).MakeGenericType(type);
                var mapper = Activator.CreateInstance(mapperType);

                SqlMapper.SetTypeMap(type, (SqlMapper.ITypeMap)mapper!);
            }
        }
    }
}
