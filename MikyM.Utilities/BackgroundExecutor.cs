#nullable disable
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable UseAwaitUsing

namespace MikyM.Utilities;

/// <summary>
/// Asynchronous executor.
/// </summary>
[PublicAPI]
public interface IBackgroundExecutor
{
    /// <summary>
    /// Executes given <see cref="Func{TResult, Task}"/> asynchronously on a background thread within a created child <see cref="ILifetimeScope"/>.
    /// </summary>
    /// <param name="func">Method to invoke.</param>
    /// <typeparam name="TService">Type of the service.</typeparam>
    /// <returns>A stub task representing the background work.</returns>
    public Task ExecuteAsync<TService>(Func<TService, Task> func);

    /// <summary>
    /// Executes given <see cref="Func{TResult, Task}"/> asynchronously on a background thread within a created child <see cref="ILifetimeScope"/>.
    /// </summary>
    /// <param name="func">Method to invoke.</param>
    /// <typeparam name="TService">Type of the service.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <returns>A stub task representing the background work.</returns>
    public Task ExecuteAsync<TService, TResult>(Func<TService, TResult> func);
    
    /// <summary>
    /// Executes given <see cref="Func{Task}"/> asynchronously on a background thread.
    /// </summary>
    /// <param name="func">Method to invoke.</param>
    /// <returns>A stub task representing the background work.</returns>
    public Task ExecuteAsync(Func<Task> func);
    
    /// <summary>
    /// Executes given <see cref="Func{TResult}"/> asynchronously on a background thread.
    /// </summary>
    /// <param name="func">Method to invoke.</param>
    /// <returns>A stub task representing the background work.</returns>
    public Task ExecuteAsync<TResult>(Func<TResult> func);
}

/// <inheritdoc cref="IBackgroundExecutor"/>
[PublicAPI]
public class BackgroundExecutor : IBackgroundExecutor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundExecutor> _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="serviceProvider">Scope.</param>
    /// <param name="logger">Logger.</param>
    public BackgroundExecutor(IServiceProvider serviceProvider, ILogger<BackgroundExecutor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

    }

    /// <inheritdoc />
    public Task ExecuteAsync<TService>(Func<TService, Task> func)
    {
        return Task.Run(async () =>
        {
            using var scope = _serviceProvider.CreateScope();
            {
                var service = scope.ServiceProvider.GetRequiredService<TService>();

                try
                {
                    await func(service);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing background work");
                }
            }
        });
    }
    
    /// <inheritdoc />
    public Task ExecuteAsync<TService, TResult>(Func<TService, TResult> func)
    {
        return Task.Run(() =>
        {
            using var scope = _serviceProvider.CreateScope();
            {
                var service = scope.ServiceProvider.GetRequiredService<TService>();

                try
                {
                    func(service);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing background work");
                }
            }
        });
    }

    /// <inheritdoc />
    public Task ExecuteAsync(Func<Task> func)
    {
        return Task.Run(async () =>
        {
            try
            {
                await func.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing background work");
            }
        });
    }
    
    /// <inheritdoc />
    public Task ExecuteAsync<TResult>(Func<TResult> func)
    {
        return Task.Run(() =>
        {
            try
            {
                func.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing background work");
            }
        });
    }
}

/// <summary>
/// DI extensions.
/// </summary>
[PublicAPI]
public static class AsyncExecutorExtensions
{
    /// <summary>
    /// Registers <see cref="IBackgroundExecutor"/> with <see cref="ContainerBuilder"/>.
    /// </summary>
    /// <param name="builder"><see cref="ContainerBuilder"/> instance</param>
    /// <returns><see cref="ContainerBuilder"/> instance.</returns>
    public static ContainerBuilder AddAsyncExecutor(this ContainerBuilder builder)
    {
        builder.RegisterType<BackgroundExecutor>().As<IBackgroundExecutor>().SingleInstance();

        return builder;
    }

    /// <summary>
    /// Registers <see cref="IBackgroundExecutor"/> with <see cref="ContainerBuilder"/>.
    /// </summary>
    /// <param name="builder"><see cref="ContainerBuilder"/> instance</param>
    /// <returns><see cref="ContainerBuilder"/> instance.</returns>
    public static ContainerBuilder AddBackgroundExecutor(this ContainerBuilder builder)
        => builder.AddAsyncExecutor();

    /// <summary>
    /// Registers <see cref="IBackgroundExecutor"/> with <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection"><see cref="IServiceCollection"/> instance</param>
    /// <returns><see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddAsyncExecutor(this IServiceCollection serviceCollection)
        => serviceCollection.AddBackgroundExecutor();

    /// <summary>
    /// Registers <see cref="IBackgroundExecutor"/> with <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection"><see cref="IServiceCollection"/> instance</param>
    /// <returns><see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddBackgroundExecutor(this IServiceCollection serviceCollection)
        => serviceCollection.AddSingleton<IBackgroundExecutor, BackgroundExecutor>();
}
