using System.IO;
using System.Resources;
using AccountingServer.BLL;
using AccountingServer.Shell.Serializer;

namespace AccountingServer.Shell.Plugins
{
    /// <summary>
    ///     插件基类
    /// </summary>
    internal abstract class PluginBase
    {
        /// <summary>
        ///     基本会计业务处理类
        /// </summary>
        protected readonly Accountant Accountant;

        protected PluginBase(Accountant accountant) => Accountant = accountant;

        /// <summary>
        ///     执行插件表达式
        /// </summary>
        /// <param name="expr">表达式</param>
        /// <param name="serializer">表示器</param>
        /// <returns>执行结果</returns>
        public abstract IQueryResult Execute(string expr, IEntitiesSerializer serializer);

        /// <summary>
        ///     显示插件帮助
        /// </summary>
        /// <returns>帮助内容</returns>
        public virtual string ListHelp()
        {
            var type = GetType();
            var resName = $"{type.Namespace}.Resources.Document.txt";
            using (var stream = type.Assembly.GetManifestResourceStream(resName))
            {
                if (stream == null)
                    throw new MissingManifestResourceException();

                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }
    }
}
