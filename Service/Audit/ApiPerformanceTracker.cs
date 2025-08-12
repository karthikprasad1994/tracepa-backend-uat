using Dapper;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using TracePca.Data;

namespace TracePca.Service.Audit
{
	
	public class ApiPerformanceTracker
	{
		//private readonly Trdmyus1Context _dbcontext;
		//private readonly IConfiguration _configuration;
		//private readonly IHttpContextAccessor _httpContextAccessor;

		//private readonly IWebHostEnvironment _env;
		//private readonly DbConnectionProvider _dbConnectionProvider;
		//private readonly string _connectionString;

		private readonly Trdmyus1Context _dbcontext;
		private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;

		private readonly IWebHostEnvironment _env;
		private readonly DbConnectionProvider _dbConnectionProvider;

		//public ApiPerformanceTracker(IConfiguration configuration)
		//{
		//	//_connectionString = configuration.GetConnectionString("DefaultConnection");
		//}

		public ApiPerformanceTracker(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, DbConnectionProvider dbConnectionProvider)
		{
			_dbcontext = dbcontext;
			_configuration = configuration;
			_httpContextAccessor = httpContextAccessor;

		}

		//public async Task<IActionResult> TrackPerformanceAsync(
		//Func<Task<HttpResponseMessage>> apiCall,
		//string apiUrl,
		//string method = "GET")
		//{
		//	var stopwatch = Stopwatch.StartNew();
		//	HttpResponseMessage response = null;
		//	string errorMessage = null;
		//	bool isSuccess = false;

		//	try
		//	{
		//		response = await apiCall();
		//		isSuccess = response.IsSuccessStatusCode;

		//		if (!isSuccess)
		//		{
		//			return new StatusCodeResult((int)response.StatusCode);
		//		}

		//		var content = await response.Content.ReadAsStringAsync();
		//		return new OkObjectResult(content);
		//	}
		//	catch (Exception ex)
		//	{
		//		errorMessage = ex.Message;
		//		return new ObjectResult($"API call failed: {errorMessage}")
		//		{
		//			StatusCode = (int)HttpStatusCode.InternalServerError
		//		};
		//	}
		//	finally
		//	{
		//		stopwatch.Stop();
		//		await LogToDatabaseAsync(
		//			apiUrl,
		//			method,
		//			DateTime.UtcNow,
		//			stopwatch.ElapsedMilliseconds,
		//			response?.StatusCode,
		//			isSuccess,
		//			errorMessage
		//		);
		//	}
		//}


		public async Task<IActionResult> TrackPerformanceAsync(
	Func<Task<HttpResponseMessage>> apiCall,
	string apiUrl,
	string method = "GET")
		{
			//_httpContextAccessor.HttpContext.Session.SetString("CustomerCode", "trdm");
			
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);


			var stopwatch = Stopwatch.StartNew();
			HttpResponseMessage response = null;
			string errorMessage = null;
			bool isSuccess = false;

			try
			{
				response = await apiCall();
				isSuccess = response.IsSuccessStatusCode;

				if (!isSuccess)
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					errorMessage = $"API returned error: {response.StatusCode} - {errorContent}";
					return new ObjectResult(errorMessage)
					{
						StatusCode = (int)response.StatusCode
					};
				}

				var content = await response.Content.ReadAsStringAsync();
				return new OkObjectResult(content);
			}
			catch (HttpRequestException httpEx)
			{
				errorMessage = $"HTTP Request failed: {httpEx.Message}";
				if (httpEx.InnerException != null)
				{
					errorMessage += $"\nInner Exception: {httpEx.InnerException.Message}";
				}
				return new ObjectResult(errorMessage)
				{
					StatusCode = (int)HttpStatusCode.InternalServerError
				};
			}
			catch (Exception ex)
			{
				errorMessage = $"Unexpected error: {ex}";
				return new ObjectResult(errorMessage)
				{
					StatusCode = (int)HttpStatusCode.InternalServerError
				};
			}
			finally
			{
				stopwatch.Stop();
				await LogToDatabaseAsync(
					apiUrl,
					method,
					DateTime.UtcNow,
					stopwatch.ElapsedMilliseconds,
					response?.StatusCode,
					isSuccess,
					errorMessage
				);
			}
		}


		private async Task LogToDatabaseAsync(
		string apiUrl,
		string method,
		DateTime requestTime,
		long responseTimeMs,
		HttpStatusCode? statusCode,
		bool isSuccess,
		string errorMessage)
		{

			//var connectionString = _configuration.GetConnectionString("DefaultConnection");
			//if (string.IsNullOrEmpty(connectionString))
			//{
			//	throw new Exception("Database connection string is missing");
			//}

			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();
			var query = @"
                INSERT INTO ApiPerformanceLogs 
                (ApiUrl, Method, RequestTime, ResponseTimeMs, StatusCode, IsSuccess, ErrorMessage)
                VALUES
                (@apiUrl, @method, @requestTime, @responseTimeMs, @statusCode, @isSuccess, @errorMessage)";

			using (var command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@apiUrl", apiUrl);
				command.Parameters.AddWithValue("@method", method);
				command.Parameters.AddWithValue("@requestTime", requestTime);
				command.Parameters.AddWithValue("@responseTimeMs", responseTimeMs);
				command.Parameters.AddWithValue("@statusCode", (object)(int?)statusCode ?? DBNull.Value);
				command.Parameters.AddWithValue("@isSuccess", isSuccess);
				command.Parameters.AddWithValue("@errorMessage", (object)errorMessage ?? DBNull.Value);
				await command.ExecuteNonQueryAsync();
			}

			//using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
			//{
			//	await connection.OpenAsync();
			//	var query = @"
			//             INSERT INTO ApiPerformanceLogs 
			//             (ApiUrl, Method, RequestTime, ResponseTimeMs, StatusCode, IsSuccess, ErrorMessage)
			//             VALUES
			//             (@apiUrl, @method, @requestTime, @responseTimeMs, @statusCode, @isSuccess, @errorMessage)";

			//	using (var command = new SqlCommand(query, connection))
			//	{
			//		command.Parameters.AddWithValue("@apiUrl", apiUrl);
			//		command.Parameters.AddWithValue("@method", method);
			//		command.Parameters.AddWithValue("@requestTime", requestTime);
			//		command.Parameters.AddWithValue("@responseTimeMs", responseTimeMs);
			//		command.Parameters.AddWithValue("@statusCode", (object)(int?)statusCode ?? DBNull.Value);
			//		command.Parameters.AddWithValue("@isSuccess", isSuccess);
			//		command.Parameters.AddWithValue("@errorMessage", (object)errorMessage ?? DBNull.Value);

			//		await command.ExecuteNonQueryAsync();
			//	}

			//}

		}
	}
}
