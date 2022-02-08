﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Oibi.TestHelper
{
	public class ServerFixture<TStartup> : /*IAsyncLifetime,*/ IDisposable where TStartup : class
	{
		/// <summary>
		/// Current <see cref="TStartup"/> server
		/// </summary>
		public TestServer Server { get; }

		/// <summary>
		/// Your <see cref="HttpClient"/>
		/// </summary>
		public HttpClient Client { get; }

		/// <summary>
		/// Get service by type
		/// </summary>
		public TService GetService<TService>() => (TService)Server.Host.Services.GetService(typeof(TService));

		//public ICollection GetRoutesOfController() => throw new NotImplementedException();
		//public ICollection GetRoutesOfControllerMethod() => throw new NotImplementedException();

		/// <summary>
		/// Instantiate a test server with asppsettings and environment valiables
		/// </summary>
		public ServerFixture()
		{
			var builder = new WebHostBuilder() //.UseEnvironment("Development")
					.ConfigureAppConfiguration((_, config) =>
					{
						config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
						config.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
						config.AddJsonFile("appsettings.test.json", optional: true, reloadOnChange: true);
						config.AddEnvironmentVariables();
					})
					.ConfigureLogging((_, logging) =>
					{
						logging.AddDebug();
					})
					.ConfigureServices(services =>
					{
						services.AddSingleton<RouteAnalyzer>();
					})
					.UseStartup<TStartup>();

			Server = new TestServer(builder);
			Client = Server.CreateClient();
		}

		/// <summary>
		/// <inheritdoc cref="HttpClient.GetAsync"/>
		/// <inheritdoc cref="SerializerExtensions.DeserializeBodyAsync{T}(HttpResponseMessage, CancellationToken)"/>
		/// </summary>
		public async Task<T> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default)
		{
			var request = await Client.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
			return await request.DeserializeBodyAsync<T>(cancellationToken: cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// <inheritdoc cref="HttpClient.PostAsync"/>
		/// <inheritdoc cref="SerializerExtensions.DeserializeBodyAsync{T}(HttpResponseMessage, CancellationToken)"/>
		/// </summary>
		public async Task<T> PostAsync<T>(string requestUri, object data, CancellationToken cancellationToken = default)
		{
			var request = await Client.PostAsync(requestUri, data.ToStringContent(), cancellationToken).ConfigureAwait(false);
			return await request.DeserializeBodyAsync<T>(cancellationToken: cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// <inheritdoc cref="HttpClient.PutAsync(string?, HttpContent)"/>
		/// <inheritdoc cref="SerializerExtensions.DeserializeBodyAsync{T}(HttpResponseMessage, CancellationToken)"/>
		/// </summary>
		public async Task<T> PutAsync<T>(string requestUri, object data, CancellationToken cancellationToken = default)
		{
			var request = await Client.PutAsync(requestUri, data.ToStringContent(), cancellationToken).ConfigureAwait(false);
			return await request.DeserializeBodyAsync<T>(cancellationToken: cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// <inheritdoc cref="HttpClient.DeleteAsync(string?, CancellationToken)"/>
		/// <inheritdoc cref="SerializerExtensions.DeserializeBodyAsync{T}(HttpResponseMessage, CancellationToken)"/>
		/// </summary>
		public async Task<T> DeleteAsync<T>(string requestUri, CancellationToken cancellationToken = default)
		{
			var request = await Client.DeleteAsync(requestUri, cancellationToken).ConfigureAwait(false);
			return await request.DeserializeBodyAsync<T>(cancellationToken: cancellationToken).ConfigureAwait(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Client?.Dispose();
				Server?.Dispose();
			}
		}

		//public virtual Task InitializeAsync()
		//{
		//	return Task.CompletedTask;
		//	//throw new NotImplementedException();
		//}

		//public virtual Task DisposeAsync()
		//{
		//	return Task.CompletedTask;
		//	//throw new NotImplementedException();
		//}

		/// <summary>
		/// <inheritdoc cref="HttpClient.GetAsync"/>
		/// </summary>
		public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default)
		{
			return Client.GetAsync(requestUri, cancellationToken);
		}
		
		/// <summary>
		/// <inheritdoc cref="HttpClient.PostAsync"/>
		/// </summary>
		public Task<HttpResponseMessage> PostAsync(string requestUri, object data, CancellationToken cancellationToken = default)
		{
			return Client.PostAsync(requestUri, data.ToStringContent(), cancellationToken);
		}

		/// <summary>
		/// <inheritdoc cref="HttpClient.PutAsync"/>
		/// </summary>
		public Task<HttpResponseMessage> PutAsync(string requestUri, object data, CancellationToken cancellationToken = default)
		{
			return Client.PutAsync(requestUri, data.ToStringContent(), cancellationToken);
		}

		/// <summary>
		/// <inheritdoc cref="HttpClient.DeleteAsync(string?, CancellationToken)"/>
		/// </summary>
		public Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken = default)
		{
			return Client.DeleteAsync(requestUri, cancellationToken);
		}

	}
}