namespace KEEK.Trainer.Utils;

public static class ReflectionExtensions {
    private static readonly HashSet<Type> SimpleTypes = new() {
        typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong),
        typeof(float), typeof(double), typeof(decimal), typeof(char), typeof(string), typeof(bool),
        typeof(DateTime), typeof(TimeSpan), typeof(DateTimeOffset), typeof(Vector2), typeof(Vector3),
        typeof(Color)
    };

    private static readonly Dictionary<Type, HashSet<FieldInfo>> CachedSimpleFieldInfos = new();

    private static bool IsSimple(this Type type) {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
            type = type.GetGenericArguments()[0];
        }

        return SimpleTypes.Contains(type);
    }

    public static HashSet<FieldInfo> GetAllSimpleFieldInfos(this Type type) {
        if (CachedSimpleFieldInfos.TryGetValue(type, out var result)) {
            return result;
        }

        CachedSimpleFieldInfos[type] = result = new HashSet<FieldInfo>();

        while (type != null && type.IsSubclassOf(typeof(object))) {
            IEnumerable<FieldInfo> fieldInfos =
                type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            foreach (FieldInfo fieldInfo in fieldInfos) {
                if (fieldInfo.FieldType.IsSimple() && !result.Contains(fieldInfo)) {
                    result.Add(fieldInfo);
                }
            }

            type = type.BaseType;
        }

        return result;
    }
}