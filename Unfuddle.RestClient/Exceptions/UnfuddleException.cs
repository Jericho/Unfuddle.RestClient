using System;

namespace Unfuddle.RestClient.Exceptions
{
	/// <summary>
	/// This class represents a generic Unfuddle error. All other exceptions thrown by
	/// the Unfuddle API subclass this exception
	/// </summary>
	public class UnfuddleException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UnfuddleException"/> class.
		/// </summary>
		/// <param name="message">A message that describes the error.</param>
		public UnfuddleException(string message)
			: base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="UnfuddleException"/> class.
		/// </summary>
		/// <param name="message">A message that describes the error.</param>
		/// <param name="innerException">The inner exception.</param>
		public UnfuddleException(string message, Exception innerException)
			: base(message, innerException) { }
	}
}