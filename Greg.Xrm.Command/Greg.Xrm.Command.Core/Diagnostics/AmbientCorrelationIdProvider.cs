using System;
using System.Threading;

namespace Greg.Xrm.Command.Diagnostics
{
	/// <summary>
	/// Provides an ambient correlation ID using <see cref="AsyncLocal{T}"/>.
	/// </summary>
	public class AmbientCorrelationIdProvider : ICorrelationIdProvider
	{
		private static readonly AsyncLocal<string?> _current = new();

		/// <summary>
		/// Gets or sets the current correlation ID.
		/// </summary>
		public string Current
		{
			get
			{
				if (_current.Value == null)
				{
					// Generate a new ID if one hasn't been set.
					// We use a Guid as a fallback, though UUIDv7 is preferred for time-ordering.
					_current.Value = Guid.NewGuid().ToString();
				}
				return _current.Value;
			}
			set
			{
				_current.Value = value;
			}
		}
	}
}
