using System.Diagnostics;

namespace Greg.Xrm.Command.Services.Output
{
	public class ConsoleProgressBar : IProgressIndicator
	{
		private readonly int totalWidth;
		private readonly bool showSpinner;
		private int currentProgress;
		private int currentSpinnerIndex;
		private string currentMessage;
		private bool disposed;
		private readonly object syncRoot = new();
		private readonly Timer? spinnerTimer;

		private static readonly char[] SpinnerChars = ['|', '/', '-', '\\'];

		public ConsoleProgressBar(int totalWidth = 40, bool showSpinner = true)
		{
			this.totalWidth = totalWidth;
			this.showSpinner = showSpinner;
			this.currentMessage = string.Empty;
			this.currentProgress = 0;
			this.currentSpinnerIndex = 0;

			if (showSpinner)
			{
				spinnerTimer = new Timer(_ => TickSpinner(), null, 100, 100);
			}
		}

		public void Report(double progress, string message)
		{
			lock (syncRoot)
			{
				currentProgress = (int)Math.Clamp(progress * 100, 0, 100);
				currentMessage = message;
				Render();
			}
		}

		public void SetMessage(string message)
		{
			lock (syncRoot)
			{
				currentMessage = message;
				Render();
			}
		}

		public void Complete(string? completionMessage = null)
		{
			lock (syncRoot)
			{
				currentProgress = 100;
				if (completionMessage != null)
					currentMessage = completionMessage;
				spinnerTimer?.Dispose();
				currentSpinnerIndex = -1;
				Render();
				Console.WriteLine();
			}
		}

		public void Fail(string? errorMessage = null)
		{
			lock (syncRoot)
			{
				spinnerTimer?.Dispose();
				currentSpinnerIndex = -1;

				var savedColor = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write($"\r  [{new string('#', totalWidth)}] FAILED");
				if (errorMessage != null)
					Console.Write($" - {errorMessage}");
				Console.WriteLine();
				Console.ForegroundColor = savedColor;
			}
		}

		public void Dispose()
		{
			if (disposed) return;
			disposed = true;
			spinnerTimer?.Dispose();
			Complete();
		}

		private void Render()
		{
			var savedColor = Console.ForegroundColor;
			try
			{
				var filledWidth = currentProgress * totalWidth / 100;
				var bar = new string('#', filledWidth).PadRight(totalWidth);
				var spinner = currentSpinnerIndex >= 0 ? SpinnerChars[currentSpinnerIndex % SpinnerChars.Length] : ' ';

				Console.Write($"\r  [{bar}] {currentProgress,3}% {spinner} {currentMessage}");
			}
			finally
			{
				Console.ForegroundColor = savedColor;
			}
		}

		private void TickSpinner()
		{
			lock (syncRoot)
			{
				if (disposed) return;
				currentSpinnerIndex = (currentSpinnerIndex + 1) % SpinnerChars.Length;
				Render();
			}
		}
	}
}
