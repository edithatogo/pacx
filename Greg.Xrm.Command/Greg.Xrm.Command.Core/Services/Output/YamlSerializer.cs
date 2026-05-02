using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Greg.Xrm.Command.Services.Output
{
	public static class YamlSerializer
	{
		private static readonly ISerializer Serializer = new SerializerBuilder()
			.WithNamingConvention(CamelCaseNamingConvention.Instance)
			.Build();

		public static string Serialize<T>(T obj)
		{
			return Serializer.Serialize(obj);
		}
	}
}
