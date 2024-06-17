# DomainBaseEntity [![CodeFactor](https://www.codefactor.io/repository/github/matindewet/domainbaseentity/badge)](https://www.codefactor.io/repository/github/matindewet/domainbaseentity) ![NuGet Version](https://img.shields.io/nuget/v/MatinDeWet.DomainBaseEntity) ![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/MatinDeWet/DomainBaseEntity/dotnet.yml)


DomainBaseEntity was designed to be used as a extension library when generating Database models using Entity Framework Core. It provides a base class that can be used to add common properties to all entities in a database model.

There are two main classes that can be used to extend your entities:
- BaseEntity
- BaseEnumerabeEntity

## Class examples

### BaseEntity
```C#
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateDeleted { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
```

### BaseEnumerabeEntity
```C#
    public abstract class BaseEnumerabeEntity : IComparable
    {
        public string Name { get; set; }

        public int Id { get; set; }

        protected BaseEnumerabeEntity(int id, string name) => (Id, Name) = (id, name);

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : BaseEnumerabeEntity =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                        .Select(f => f.GetValue(null))
                        .Cast<T>();

        public override bool Equals(object? obj)
        {
            if (obj is not BaseEnumerabeEntity otherValue)
                return false;

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static int AbsoluteDifference(BaseEnumerabeEntity firstValue, BaseEnumerabeEntity secondValue)
        {
            var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
            return absoluteDifference;
        }

        public static T FromValue<T>(int value) where T : BaseEnumerabeEntity
        {
            var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
            return matchingItem;
        }

        public static T FromDisplayName<T>(string displayName) where T : BaseEnumerabeEntity
        {
            var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
            return matchingItem;
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : BaseEnumerabeEntity
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
                throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

            return matchingItem;
        }

        public int CompareTo(object? other) => Id.CompareTo(((BaseEnumerabeEntity)other!).Id);
    }
```

## Usage
To use the classes in your project, simply inherit from the class you want to
extend your entity with.

### BaseEntity
```C#
	public class User : BaseEntity
	{
		public string Name { get; set; }
	}
```

### BaseEnumerabeEntity
```C#
    public class Currency : BaseEnumerabeEntity
    {
        public static readonly Currency ZAR = new(1, "Rand", nameof(ZAR), "R");

        public Currency(int id, string name, string code, string symbol = "") : base(id, name)
        {
            Code = code;
            Symbol = symbol;
            SortOrder = sortOrder;
        }

        public string Code { get; set; }

        public string Symbol { get; set; }
    }
```

The BaseEnumerabeEntity allows you to create a list of properties, when needed.
```C#
    private readonly HashSet<Currency> _currencies
        = BaseEnumerabeEntity.GetAll<Currency>().ToHashSet();
```

