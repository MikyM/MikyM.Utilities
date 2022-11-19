using MikyM.Utilities.Optionals;

namespace MikyM.Utilities.Extensions;

/// <summary>
/// Type extensions.
/// </summary>
[PublicAPI]
public static class TypeExtensions
{
    /// <summary>
    /// Extended get interfaces method.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="includeInherited"></param>
    /// <param name="fullSet"></param>
    /// <returns></returns>
    public static IEnumerable<Type> GetInterfaces(this Type type, bool includeInherited, bool fullSet = true)
    {
        if ((includeInherited || type.BaseType is null) && fullSet)
            return type.GetInterfaces();

        var allInterfaces = type.GetInterfaces();

        switch (includeInherited)
        {
            case false when !fullSet:
                return type.GetInterfaces().Except(allInterfaces.SelectMany(x => x.GetInterfaces()));
            case true when type.BaseType is not null && !fullSet:
            {
                var res = type.GetInterfaces().Except(allInterfaces.SelectMany(x => x.GetInterfaces())).ToList();
                res.AddRange(type.BaseType.GetInterfaces());

                return res;
            }
            default:
                return type.GetInterfaces().Except(allInterfaces.SelectMany(x => x.GetInterfaces())).ToList();
        }
    }

    /// <summary>
    /// Determines whether the given type is a closed <see cref="Optional{TValue}"/>.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>true if the type is a closed Optional; otherwise, false.</returns>
    public static bool IsOptional(this Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        return type.GetGenericTypeDefinition() == typeof(Optional<>);
    }
    
    /// <summary>
    /// Gets the types name while discarding anything that comes after "`" for generic types.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Types name.</returns>
    public static string GetName(this Type type)
    {
        return !type.IsGenericType ? type.Name : type.Name.Split('`').First();
    }

    /// <summary>
    /// Determines whether the given type is a closed <see cref="Nullable{TValue}"/>.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>true if the type is a closed Nullable; otherwise, false.</returns>
    public static bool IsNullable(this Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        return type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    /// Retrieves the innermost type from a type wrapped by
    /// <see cref="Nullable{T}"/> or <see cref="Optional{TValue}"/>.
    /// </summary>
    /// <param name="type">The type to unwrap.</param>
    /// <returns>The unwrapped type.</returns>
    public static Type Unwrap(this Type type)
    {
        var currentType = type;
        while (currentType.IsGenericType)
        {
            if (currentType.IsOptional() || currentType.IsNullable())
            {
                currentType = currentType.GetGenericArguments()[0];
                continue;
            }

            break;
        }

        return currentType;
    }

    /// <summary>
    /// Gets a dictionary with interface implementation pairs that implement a given base interface.
    /// </summary>
    /// <param name="interfaceToSearchFor">Base interface to search for.</param>
    public static Dictionary<Type, Type?> GetInterfaceImplementationPairs(this Type interfaceToSearchFor)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var dict = assemblies
            .SelectMany(x => x.GetTypes()
                .Where(t => interfaceToSearchFor.IsDirectAncestor(t) &&
                            t.IsInterface))
            .ToDictionary(intr => intr,
                intr => assemblies.SelectMany(impl => impl.GetTypes())
                    .FirstOrDefault(impl =>
                        impl.IsAssignableToWithGenerics(intr) && impl.IsClass &&
                        intr.IsDirectAncestor(impl)));

        return dict;
    }

    /// <summary>
    /// Checks whether the given type is assignable to another type supporting generic types.
    /// </summary>
    /// <param name="givenType">Type to check.</param>
    /// <param name="genericType">Type to compare with.</param>
    /// <returns>True if the given type is assignable to another type, otherwise false.</returns>
    public static bool IsAssignableToWithGenerics(this Type givenType, Type genericType)
    {
        if (!genericType.IsGenericType)
            return givenType.IsAssignableTo(genericType);

        var interfaceTypes = givenType.GetInterfaces();

        if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
            return true;
        
        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            return true;

        Type? baseType = givenType.BaseType;
        if (baseType == null) return false;

        return IsAssignableToWithGenerics(baseType, genericType);
    }

    /// <summary>
    /// Retrieves the type inheritance tree
    /// </summary>
    /// <param name="type">The type to find tree for.</param>
    /// <returns>The inheritance tree.</returns>
    public static InheritanceTree GetTypeInheritance(this Type type)
    {
        //get all the interfaces for this type
        var interfaces = type.GetInterfaces();

        //get all the interfaces for the ancestor interfaces
        var baseInterfaces = interfaces.SelectMany(i => i.GetInterfaces());

        //filter based on only the direct interfaces
        var directInterfaces = interfaces.Where(i => baseInterfaces.All(b => b != i));

        return new InheritanceTree(type, directInterfaces.Select(GetTypeInheritance).ToList());
    }

    /// <summary>
    /// Check if a type is a direct ancestor of given type
    /// </summary>
    public static bool IsDirectAncestor(this Type ancestorCandidate, Type type)
        => type.GetTypeInheritance().IsDirectAncestor(ancestorCandidate);

    /// <summary>
    /// Gets the direct ancestors of a given type.
    /// </summary>
    public static IEnumerable<Type> GetDirectAncestors(this Type type, bool onlyInterfaces = false)
        => onlyInterfaces
            ? type.GetTypeInheritance().Ancestors.Select(x => x.Node).Where(x => x.IsInterface)
            : type.GetTypeInheritance().Ancestors.Select(x => x.Node);

    /// <summary>
    /// Gets interface by naming convention.
    /// </summary>
    public static Type? GetInterfaceByNamingConvention(this Type type)
        => type.GetInterface($"I{type.Name}");
    
    /// <summary>
    /// Gets the direct ancestors of a given type.
    /// </summary>
    public static IEnumerable<Type> GetDirectInterfaceAncestors(this Type type)
        => GetDirectAncestors(type, true);
    
    /// <summary>
    /// Gets the direct ancestors of a given type.
    /// </summary>
    public static IEnumerable<Type> GetDirectClassAncestors(this Type type, bool skipAbstract = false)
        => skipAbstract
            ? type.GetTypeInheritance().Ancestors.Select(x => x.Node).Where(x => x.IsClass && !x.IsAbstract)
            : type.GetTypeInheritance().Ancestors.Select(x => x.Node).Where(x => x.IsClass);

    /// <summary>
    /// Inheritance tree
    /// </summary>
    [PublicAPI]
    public class InheritanceTree
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="ancestors"></param>
        public InheritanceTree(Type node, List<InheritanceTree> ancestors)
        {
            Node = node;
            Ancestors = ancestors;
        }

        /// <summary>
        /// Current type node.
        /// </summary>
        public Type Node { get; set; }
        /// <summary>
        /// Ancestors.
        /// </summary>
        public List<InheritanceTree> Ancestors { get; set; }

        /// <summary>
        /// Checks whether the given type is a direct ancestor.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if the given type is a direct ancestor, otherwise false.</returns>
        public bool IsDirectAncestor(Type type)
            => Ancestors.Any(x =>
                (x.Node.IsGenericType ? x.Node.GetGenericTypeDefinition() : x.Node) ==
                (type.IsGenericType ? type.GetGenericTypeDefinition() : type));
    }
}
