using System;

namespace Greg.Xrm.Command.Services
{
	/// <summary>
	/// Provides an abstraction for the system clock to enable deterministic testing.
	/// </summary>
	public interface IClock
	{
		/// <summary>
		/// Gets a <see cref="DateTimeOffset"/> object that is set to the current date and time on this computer,
		/// expressed as the Coordinated Universal Time (UTC).
		/// </summary>
		DateTimeOffset UtcNow { get; }

		/// <summary>
		/// Gets a <see cref="DateTimeOffset"/> object that is set to the current date and time on this computer,
		/// expressed as the local time.
		/// </summary>
		DateTimeOffset Now { get; }
	}
}
