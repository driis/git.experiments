using System.Threading.Tasks;

namespace git.net
{
    internal class AsyncBridge
    {
        /// <summary>
        ///  Dummy "async bridge" which mostly exists to provide a convenient 
        /// refactoring point when I eventually need to address the sync and async mixing and matching of code.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static T RunSync<T>(Task<T> task)
        {
            return task.Result;
        }


    }
}