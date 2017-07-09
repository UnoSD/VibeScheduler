using System;

namespace VibeScheduler
{
    public static class EnumExtensions
    {
        public static T Next<T>(this Enum current) where T : struct
        {
            if (!typeof(T).IsEnum || current.GetType() != typeof(T))
                throw new ArgumentException($"Argumnent {typeof(T).FullName} is not compatible with {current.GetType().FullName}");

            var members = (T[])Enum.GetValues(current.GetType());

            var indexOfNextMember = Array.IndexOf(members, current) + 1;

            return members.Length == indexOfNextMember ?
                   members[0] :
                   members[indexOfNextMember];
        }
    }
}