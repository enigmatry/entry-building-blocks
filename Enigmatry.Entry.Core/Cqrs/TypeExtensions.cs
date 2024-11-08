using JetBrains.Annotations;
using MediatR;

namespace Enigmatry.Entry.Core.Cqrs;

[PublicAPI]
public static class TypeExtensions
{
    /// <summary>
    /// Checks if the request is a query.
    /// </summary>
    /// <param name="request">MediatR Request</param>
    /// <typeparam name="T">Type of request</typeparam>
    /// <returns>True if request derives from <see cref="IBaseQuery"/> interface</returns>
    public static bool IsQuery<T>(this T request) where T : IBaseRequest => request is IBaseQuery;

    /// <summary>
    /// Checks if the request is a command.
    /// </summary>
    /// <param name="request">MediatR Request</param>
    /// <typeparam name="T">Type of request</typeparam>
    /// <returns>True if request derives from <see cref="IBaseCommand"/> interface</returns>
    public static bool IsCommand<T>(this T request) where T : IBaseRequest => request is IBaseCommand;
}
