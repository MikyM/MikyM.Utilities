using Autofac;
using Microsoft.Extensions.Logging;
using MikyM.Common.Utilities.Extensions;

#nullable disable
// ReSharper disable UseAwaitUsing

namespace MikyM.Common.Utilities;

/// <summary>
/// Asynchronous executor
/// </summary>
public interface IAsyncExecutor
{
    public Task ExecuteAsync<T>(Func<T, Task> func);
    public Task ExecuteAsync<T>(Func<Task<T>> func);
    public Task ExecuteAsync(Func<Task> func);
}

/// <inheritdoc cref="IAsyncExecutor"/>
public class AsyncExecutor : IAsyncExecutor
{
    private readonly ILifetimeScope _lifetimeScope;

    public AsyncExecutor(ILifetimeScope lifetimeScope)
    {
        _lifetimeScope = lifetimeScope;
    }

    /// <summary>
    /// Executes a method on a new thread using a child <see cref="ILifetimeScope"/>.
    /// </summary>
    /// <typeparam name="T">Service</typeparam>
    /// <param name="func">Method to be executed</param>
    /// <returns>Created task</returns>
    public Task ExecuteAsync<T>(Func<T, Task> func)
    {
        return Task.Run(async () =>
            {
                using var scope = _lifetimeScope.BeginLifetimeScope();
                {
                    var service = scope.Resolve<T>();
                    await func(service);
                }
            })
            .ContinueWith(
                x => _lifetimeScope.Resolve<ILogger<T>>().LogError(x.Exception, x.Exception?.ToFormattedString()),
                TaskContinuationOptions.OnlyOnFaulted);
    }

    /// <summary>
    /// Executes a method on a new thread using root <see cref="ILifetimeScope"/>.
    /// </summary>
    /// <typeparam name="T">Service</typeparam>
    /// <param name="func">Method to be executed</param>
    /// <returns>Created task</returns>
    public Task ExecuteAsync<T>(Func<Task<T>> func)
    {
        return Task.Run(async () => { await func.Invoke(); })
            .ContinueWith(
                x => _lifetimeScope.Resolve<ILogger<T>>().LogError(x.Exception, x.Exception?.ToFormattedString()),
                TaskContinuationOptions.OnlyOnFaulted);
    }

    /// <summary>
    /// Executes a method on a new thread using root <see cref="ILifetimeScope"/>.
    /// </summary>
    /// <param name="func">Method to be executed</param>
    /// <returns>Created task</returns>
    public Task ExecuteAsync(Func<Task> func)
    {
        return Task.Run(async () => { await func.Invoke(); })
            .ContinueWith(
                x => _lifetimeScope.Resolve<ILoggerFactory>().CreateLogger(func.Target?.GetType().Name ?? "Logger").LogError(x.Exception, x.Exception?.ToFormattedString()),
                TaskContinuationOptions.OnlyOnFaulted);
    }
}

/// <summary>
/// DI extensions
/// </summary>
public static class AsyncExecutorExtensions
{
    /// <summary>
    /// Registers <see cref="IAsyncExecutor"/> with <see cref="ContainerBuilder"/>.
    /// </summary>
    /// <param name="services"><see cref="ContainerBuilder"/> instance</param>
    public static void AddAsyncExecutor(this ContainerBuilder services)
        => services.RegisterType<AsyncExecutor>().As<IAsyncExecutor>().SingleInstance();
}
