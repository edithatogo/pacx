using System;

namespace Greg.Xrm.Command.Services
{
	public class Clock : IClock
	{
		public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
		public DateTimeOffset Now => DateTimeOffset.Now;
	}
}
