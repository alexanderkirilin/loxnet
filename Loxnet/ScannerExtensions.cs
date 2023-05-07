using System.Reflection;

namespace Loxnet;

public static class ScannerExtensions
{
    #if DEBUG
    public static T? GetFieldValue<T>(this Scanner obj, string name) {
        var field = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return (T?)field?.GetValue(obj);
    }
    #endif
}