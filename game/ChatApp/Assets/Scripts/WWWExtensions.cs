using UnityEngine.Networking;

namespace DefaultNamespace
{
    public static class WWWExtensions
    {
        public static UnityWebRequest WithHeader(this UnityWebRequest request, string name, string value)
        {
            request.SetRequestHeader(name, value);
            return request;
        }
    }
}