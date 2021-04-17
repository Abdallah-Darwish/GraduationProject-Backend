using System.Collections;

namespace GradProjectServer.Common
{
    public static class Extensions
    {
        public static bool IsSingle(this IEnumerable e)
        {
            var enumerator = e.GetEnumerator();
            if (enumerator.MoveNext() == false) { return false; }
            if (enumerator.MoveNext()) { return false; }
            return true;
        }
    }
}
