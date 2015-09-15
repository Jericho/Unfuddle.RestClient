using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Unfuddle.RestClient.Utilities
{
	public static class ExtensionMethods
	{
		/// <summary>
		/// How to properly convert a string to SecureString
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		/// <remarks>http://blogs.msdn.com/b/fpintos/archive/2009/06/12/how-to-properly-convert-securestring-to-string.aspx</remarks>
		public static SecureString ConvertToSecureString(this string password)
		{
			if (password == null) throw new ArgumentNullException("password");

			var securePassword = new SecureString();
			foreach (char c in password)
			{
				securePassword.AppendChar(c);
			}
			securePassword.MakeReadOnly();
			return securePassword;
		}

		/// <summary>
		/// How to properly convert a SecureString to String
		/// </summary>
		/// <param name="securePassword"></param>
		/// <returns></returns>
		/// <remarks>http://blogs.msdn.com/b/fpintos/archive/2009/06/12/how-to-properly-convert-securestring-to-string.aspx</remarks>
		public static string ConvertToUnsecureString(this SecureString securePassword)
		{
			if (securePassword == null) throw new ArgumentNullException("securePassword");

			IntPtr unmanagedString = IntPtr.Zero;
			try
			{
				unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
				return Marshal.PtrToStringUni(unmanagedString);
			}
			finally
			{
				Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
			}
		}
	}
}
