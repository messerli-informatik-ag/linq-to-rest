# Changelog

## 0.1.0
- Initial release

## 0.2.0
- Fix `Select`ed subqueries using the root url instead of a sub-resource

## 0.2.1
- Add enum support

## 0.3.0
- Add composition root via `QueryProviderBuilder`

## 0.4.0
- Rename `QueryProviderBuilder` to `QueryableBuilder`
- Let `QueryableBuilder` return an `IQueryable<T>` directly
- Provide an interface for `QueryableBuilder`

## 0.4.1
- Add support for deserialization of types that cannot be constructed directly via a cast from `string`

## 0.5.0
- Update to netstandard2.1.
- Remove unused dependencies
- Update nuget metadata

## 0.5.1
- Add support for .NET Standard 2.0.

## 0.5.2
- Improve pluralize for nouns ending with `y`.

## 0.5.3
- Replace pluralization with `Pluralize.NET.Core` nuget version 1.0.0.

## 0.5.4
- Update to `Funcky` 2.0.0.

## 0.5.5
- Update `Messerli.Utility` to 0.3.0.

## 0.5.7
- Provide async eager methods for `IQueryable`: `ToListAsync`, `FirstAsync`, `ForEachAsync`, etc.
  These should be preferred over their synchronous counterparts to prevent deadlocks.
- Prevent deadlocks via `ConfigureAwait(false)`

## 0.6.0
- Implement `IAsyncEnumerable<T>` on Query

## 0.6.1
- Fix relative urls mistakenly being converted to absolute urls
- Publish symbols package

## 0.6.2
- Fix validation for injected properties

## 0.6.3
- Fix deserialization of date(-time) strings to string or string new types.
- Throw `Messerli.LinqToRest.UnavailableResourceException` instead of `Messerli.ServerCommunication.UnavailableResourceException`.

## 0.6.4
- Allow configuration of the naming policy for resource uris:
  ```csharp
  var queryable = new QueryableBuilder()
      .ResourceNamingPolicy(KebabCasePlural.Plural)
      .Build();
  ```
