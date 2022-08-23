using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MikyM.Common.Utilities.Extensions;

#nullable disable
// ReSharper disable UseAwaitUsing

namespace MikyM.Common.Utilities;

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
    private readonly ILifetimeScope _lifetimeScope;
    private readonly ILogger<BackgroundExecutor> _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="lifetimeScope">Scope.</param>
    /// <param name="logger">Logger.</param>
    public BackgroundExecutor(ILifetimeScope lifetimeScope, ILogger<BackgroundExecutor> logger)
    {
        _lifetimeScope = lifetimeScope;
        _logger = logger;

    }

    /// <inheritdoc />
    public Task ExecuteAsync<TService>(Func<TService, Task> func)
    {
        return Task.Run(async () =>
        {
            using var scope = _lifetimeScope.BeginLifetimeScope();
            {
                var service = scope.Resolve<TService>();

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
            using var scope = _lifetimeScope.BeginLifetimeScope();
            {
                var service = scope.Resolve<TService>();

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
}
