using System.Diagnostics.CodeAnalysis;
using MediatR;

namespace Enigmatry.Entry.Core.Cqrs;

/// <summary>
/// Base interface for all the queries.
/// </summary>
[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "This design fits with the MediatR library.")]
public interface IBaseQuery;

/// <summary>
/// Baser query for all the queries that return a response.
/// </summary>
/// <typeparam name="TResponse">Type of response</typeparam>
[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "This design fits with the MediatR library.")]
public interface IQuery<out TResponse> : IBaseQuery, IRequest<TResponse>;
