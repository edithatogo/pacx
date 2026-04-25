using System.Reflection;
using Greg.Xrm.Command.Commands.Explore;
using Greg.Xrm.Command;
using Greg.Xrm.Command.Plugin.Automation.Commands.Workflow;

namespace Greg.Xrm.Command.ArchitectureTests
{
	public class ArchitectureRulesTests
	{
		public void CommandsShouldNotReferenceHttpClientDirectly()
		{
			var failures = new List<string>();
			foreach (var type in GetExecutorTypes().Where(type => type.Namespace == "Greg.Xrm.Command.Commands.Explore"))
			{
				var offending = GetSystemNetHttpDependencies(type).ToArray();
				if (offending.Any())
				{
					Console.WriteLine($"{type.FullName}: {string.Join(", ", offending.Select(t => t.FullName ?? t.Name))}");
					failures.Add(type.FullName ?? type.Name);
				}
			}

			Ensure(!failures.Any(), string.Join(Environment.NewLine, failures));
		}

		public void ExecutorsShouldNotDependOnOtherExecutors()
		{
			var executorTypes = GetExecutorTypes().ToArray();
			var executorTypeSet = executorTypes.ToHashSet();

			var violations = executorTypes
				.SelectMany(executorType => GetDirectDependencies(executorType)
					.Where(dependency => dependency != executorType && executorTypeSet.Contains(dependency))
					.Select(dependency => $"{executorType.FullName} depends on {dependency.FullName}"))
				.ToArray();

			Ensure(!violations.Any(), string.Join(Environment.NewLine, violations));
		}

		public void ExecutorsShouldImplementICommandExecutor()
		{
			var failures = GetExecutorTypes()
				.Where(type => !ImplementsCommandExecutor(type))
				.Select(type => type.FullName)
				.ToArray();

			Ensure(!failures.Any(), string.Join(Environment.NewLine, failures));
		}

		private static IEnumerable<Type> GetExecutorTypes()
		{
			var assemblies = new[]
			{
				typeof(ExploreBranchesCommandExecutor).Assembly,
				typeof(ConnectionListCommandExecutor).Assembly
			};

			return assemblies
				.SelectMany(assembly => assembly.GetTypes())
				.Where(type => type.IsClass && !type.IsAbstract && type.Name.EndsWith("Executor", StringComparison.Ordinal));
		}

		private static IEnumerable<Type> GetDirectDependencies(Type type)
		{
			var dependencies = new HashSet<Type>();

			foreach (var constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
			{
				foreach (var parameter in constructor.GetParameters())
				{
					AddIfExecutor(parameter.ParameterType, dependencies);
				}
			}

			foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
			{
				AddIfExecutor(field.FieldType, dependencies);
			}

			foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
			{
				AddIfExecutor(property.PropertyType, dependencies);
			}

			if (type.BaseType != null)
			{
				AddIfExecutor(type.BaseType, dependencies);
			}

			return dependencies;
		}

		private static void AddIfExecutor(Type candidate, ISet<Type> dependencies)
		{
			if (candidate.IsGenericType)
			{
				foreach (var genericArgument in candidate.GetGenericArguments())
				{
					AddIfExecutor(genericArgument, dependencies);
				}
			}

			if (candidate.IsClass && candidate.Name.EndsWith("Executor", StringComparison.Ordinal))
			{
				dependencies.Add(candidate);
			}
		}

		private static bool ImplementsCommandExecutor(Type type)
		{
			return type.GetInterfaces()
				.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandExecutor<>));
		}

		private static IEnumerable<Type> GetSystemNetHttpDependencies(Type type)
		{
			return type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.SelectMany(constructor => constructor.GetParameters())
				.Select(parameter => parameter.ParameterType)
				.Concat(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Select(field => field.FieldType))
				.Concat(type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Select(property => property.PropertyType))
				.Concat(type.BaseType is null ? Array.Empty<Type>() : new[] { type.BaseType })
				.Where(IsSystemNetHttpType)
				.Distinct();
		}

		private static bool IsSystemNetHttpType(Type type)
		{
			if (type.Namespace is not null && type.Namespace.StartsWith("System.Net.Http", StringComparison.Ordinal))
			{
				return true;
			}

			if (type == typeof(HttpClient))
			{
				return true;
			}

			if (type.IsGenericType)
			{
				return type.GetGenericArguments().Any(IsSystemNetHttpType);
			}

			return false;
		}

		private static void Ensure(bool condition, string message)
		{
			if (!condition)
			{
				throw new InvalidOperationException(message);
			}
		}
	}
}
