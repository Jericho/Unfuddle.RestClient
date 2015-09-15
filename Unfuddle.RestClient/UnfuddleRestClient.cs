using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using Unfuddle.RestClient.Exceptions;
using Unfuddle.RestClient.Models;
using Unfuddle.RestClient.Utilities;

namespace Unfuddle.RestClient
{
	/// <summary>
	/// Core class for using the Unfuddle Api
	/// </summary>
	public class UnfuddleRestClient //: IUnfuddleRestClient
	{
		#region Fields

		private static readonly string _version = GetVersion();
		private readonly IRestClient _client;
		private const string UNFUDDLE_API_URL = "{0}://{1}.unfuddle.com/api/v1/";

		#endregion

		#region Properties

		/// <summary>
		/// Your Unfuddle username
		/// </summary>
		public string UserName { get; private set; }

		/// <summary>
		/// Your Unfuddle password
		/// </summary>
		public SecureString Password { get; private set; }

		/// <summary>
		/// Your Unfuddle account subdomain
		/// </summary>
		public string SubDomain { get; private set; }

		/// <summary>
		/// Indicates if your Unfuddle account supports SSL
		/// </summary>
		public bool UseSecureConnection { get; private set; }

		/// <summary>
		/// The web proxy
		/// </summary>
		public IWebProxy Proxy
		{
			get { return _client.Proxy; }
		}

		/// <summary>
		/// The user agent
		/// </summary>
		public string UserAgent
		{
			get { return _client.UserAgent; }
		}

		/// <summary>
		/// The timeout
		/// </summary>
		public int Timeout
		{
			get { return _client.Timeout; }
		}

		public string BaseUrl { get { return string.Format(UNFUDDLE_API_URL, this.UseSecureConnection ? "https" : "http", this.SubDomain); } }

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="CakeMailRestClient"/> class.
		/// </summary>
		/// <param name="restClient">The rest client</param>
		public UnfuddleRestClient(IRestClient restClient)
		{
			_client = restClient;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CakeMailRestClient"/> class.
		/// </summary>
		/// <param name="subDomain">Your Unfuddle account subdomain</param>
		/// <param name="useSecureConnection">Indicate if your Unfuddle account supports SSL</param>
		/// <param name="timeout">Timeout in milliseconds for connection to web service. The default is 5000.</param>
		/// <param name="webProxy">The web proxy</param>
		public UnfuddleRestClient(string userName, string password, string subDomain, bool useSecureConnection, int timeout = 5000, IWebProxy webProxy = null)
		{
			this.UserName = UserName;
			this.Password = password.ConvertToSecureString();
			this.SubDomain = subDomain;
			this.UseSecureConnection = useSecureConnection;

			var baseUrl = string.Format(UNFUDDLE_API_URL, this.UseSecureConnection ? "https" : "http", subDomain);

			_client = new RestSharp.RestClient(baseUrl)
			{
				Authenticator = new HttpBasicAuthenticator(this.UserName, password),
				Timeout = timeout,
				UserAgent = string.Format("Unfuddle .NET REST Client {0}", _version),
				Proxy = webProxy
			};
		}

		#endregion

		#region Methods related to ACCOUNTS

		/// <summary>
		/// Retrieve a list of accounts.
		/// </summary>
		/// <returns>Enumeration of <see cref="Account">accounts</see></returns>
		public IEnumerable<Account> GetAccounts()
		{
			var path = "/account.xml";

			ExecuteRequest(path, null, Method.GET);

			return null;
		}


		#endregion

		#region Methods related to PEOPLE

		/// <summary>
		/// Retrieve a list of people.
		/// </summary>
		/// <returns>Enumeration of <see cref="People">people</see></returns>
		public IEnumerable<People> GetPeople()
		{
			var path = "/people";

			ExecuteRequest(path, null, Method.GET);

			return null;
		}

		/// <summary>
		/// Retrieve a list of people.
		/// </summary>
		/// <returns>Enumeration of <see cref="People">people</see></returns>
		public IEnumerable<People> GetUser(string userId)
		{
			var path = "/people/" + userId;

			ExecuteRequest(path, null, Method.GET);

			return null;
		}


		#endregion

		#region Private Methods

		private static string GetVersion()
		{
			try
			{
				// The following may throw 'System.Security.Permissions.FileIOPermission' under some circumpstances
				//var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

				// Here's an alternative suggested by Phil Haack: http://haacked.com/archive/2010/11/04/assembly-location-and-medium-trust.aspx
				var assemblyVersion = new AssemblyName(typeof(UnfuddleRestClient).Assembly.FullName).Version;
				var version = string.Format("{0}.{1}.{2}.{3}", assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build, assemblyVersion.Revision);

				return version;
			}
			catch
			{
				return "0.0.0.0";
			}
		}

		private IRestResponse ExecuteRequest(string urlPath, IEnumerable<KeyValuePair<string, object>> parameters, Method httpMethod)
		{
			var request = new RestRequest(urlPath, httpMethod) { RequestFormat = DataFormat.Json };

			if (parameters != null)
			{
				foreach (var parameter in parameters)
				{
					request.AddParameter(parameter.Key, parameter.Value);
				}
			}

			var response = _client.Execute(request);
			var responseUri = response.ResponseUri ?? new Uri(string.Format("{0}/{1}", _client.BaseUrl.ToString().TrimEnd('/'), request.Resource.TrimStart('/')));

			if (response.ResponseStatus == ResponseStatus.Error)
			{
				var errorMessage = string.Format("Error received while making request: {0}", response.ErrorMessage);
				throw new HttpException(errorMessage, response.StatusCode, responseUri);
			}
			else if (response.ResponseStatus == ResponseStatus.TimedOut)
			{
				throw new HttpException("Request timed out", response.StatusCode, responseUri, response.ErrorException);
			}

			var statusCode = (int)response.StatusCode;
			if (statusCode == 200)
			{
				if (string.IsNullOrEmpty(response.Content))
				{
					var missingBodyMessage = string.Format("Received a 200 response from {0} but there was no message body.", request.Resource);
					throw new HttpException(missingBodyMessage, response.StatusCode, responseUri);
				}
				else if (response.ContentType == null || !response.ContentType.Contains("json"))
				{
					var unsupportedContentTypeMessage = string.Format("Received a 200 response from {0} but the content type is not JSON: {1}", request.Resource, response.ContentType ?? "NULL");
					throw new UnfuddleException(unsupportedContentTypeMessage);
				}

				#region DEBUGGING
#if DEBUG
				var debugRequestMsg = string.Format("Request sent to Unfuddle: {0}/{1}", _client.BaseUrl.ToString().TrimEnd('/'), urlPath.TrimStart('/'));
				var debugHeadersMsg = string.Format("Request headers: {0}", string.Join("&", request.Parameters.Where(p => p.Type == ParameterType.HttpHeader).Select(p => string.Concat(p.Name, "=", p.Value))));
				var debugParametersMsg = string.Format("Request parameters: {0}", string.Join("&", request.Parameters.Where(p => p.Type != ParameterType.HttpHeader).Select(p => string.Concat(p.Name, "=", p.Value))));
				var debugResponseMsg = string.Format("Response received from Unfuddle: {0}", response.Content);
				Debug.WriteLine("{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}\r\n{0}", new string('=', 25), debugRequestMsg, debugHeadersMsg, debugParametersMsg, debugResponseMsg);
#endif
				#endregion

				// Request was successful
				return response;
			}
			else if (statusCode >= 400 && statusCode < 500)
			{
				if (string.IsNullOrEmpty(response.Content))
				{
					var missingBodyMessage = string.Format("Received a {0} error from {1} with no body", response.StatusCode, request.Resource);
					throw new HttpException(missingBodyMessage, response.StatusCode, responseUri);
				}

				var errorMessage = string.Format("Received a {0} error from {1} with the following content: {2}", response.StatusCode, request.Resource, response.Content);
				throw new HttpException(errorMessage, response.StatusCode, responseUri);
			}
			else if (statusCode >= 500 && statusCode < 600)
			{
				var errorMessage = string.Format("Received a server ({0}) error from {1}", (int)response.StatusCode, request.Resource);
				throw new HttpException(errorMessage, response.StatusCode, responseUri);
			}
			else if (!string.IsNullOrEmpty(response.ErrorMessage))
			{
				var errorMessage = string.Format("Received an error message from {0} (status code: {1}) (error message: {2})", request.Resource, (int)response.StatusCode, response.ErrorMessage);
				throw new HttpException(errorMessage, response.StatusCode, responseUri);
			}
			else
			{
				var errorMessage = string.Format("Received an unexpected response from {0} (status code: {1})", request.Resource, (int)response.StatusCode);
				throw new HttpException(errorMessage, response.StatusCode, responseUri);
			}
		}

		/*
		private JToken ParseCakeMailResponse(IRestResponse response)
		{
			try
			{
				/* A typical response from the CakeMail API looks like this:
				 *	{
				 *		"status" : "success",
				 *		"data" : { ... data for the API call ... }
				 *	}
				 *	
				 * In case of an error, the response looks like this:
				 *	{
				 *		"status" : "failed",
				 *		"data" : "An error has occured"
				 *	}
				 *
				var cakeResponse = JObject.Parse(response.Content);
				var status = cakeResponse["status"].ToString();
				var data = cakeResponse["data"];
				var postData = cakeResponse["post"];

				if (status != "success")
				{
					if (postData != null) throw new CakeMailPostException(data.ToString(), postData.ToString());
					else throw new CakeMailException(data.ToString());
				}

				return data;
			}
			catch (JsonReaderException ex)
			{
				throw new CakeMailException(string.Format("Unable to decode response from CakeMail as JSON: {0}", response.Content), ex);
			}
		}
*/

		#endregion
	}
}