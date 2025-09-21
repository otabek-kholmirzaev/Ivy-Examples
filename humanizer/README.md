# HumanizerProject 

Humanizer App is an application that uses the [Humanizer NuGet Package](https://github.com/Humanizr/Humanizer). This simple demo showcases the basic functionalities of Humanizer, including Humanize, Truncate, Pascalize, Camelize, Underscore, and Kebaberize.

Web application created using [Ivy](https://github.com/Ivy-Interactive/Ivy). 
Ivy is a web framework for building interactive web applications using C# and .NET.

For example:

```
Input:
PascalCaseInputStringIsTurnedIntoSentence

Option:
Humanize => Sentence

Output
Pascal case input string is turned into sentence
```

```
Input:
SomeText

Option:
Kebaberize

Output
some-text
```

## Set Up Humanizer 

You can install Humanizer as a [NuGet package](https://www.nuget.org/packages/Humanizer).

Please, refer to the [Humanizer Documentation](https://github.com/Humanizr/Humanizer).

```
dotnet add package Humanizer --version 2.14.1
```

## Run

```
dotnet watch
```

## Deploy

```
ivy deploy
```

## Resources

- https://github.com/Humanizr/Humanizer
- https://github.com/Ivy-Interactive/Ivy-Framework
- https://ivy.app
- https://docs.ivy.app
- https://samples.ivy.app