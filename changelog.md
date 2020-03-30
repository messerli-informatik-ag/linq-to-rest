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
