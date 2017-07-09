using Java.Lang;

namespace VibeScheduler
{
    public static class ObjectExtensions
    {
        public static T Cast<T>(this Object obj) where T : class => 
            obj.GetType()
               .GetProperty("Instance")?
               .GetValue(obj, null) as T;
    }
}