# NewtonsoftJsonApp 

Web application created using [Ivy](https://github.com/Ivy-Interactive/Ivy). 

This sample shows how to use the Newtonsoft.Json library in Ivy.

The sample demostrates simple Serialization and Deserialization of an object that contains different primitive types.

Ivy is a web framework for building interactive web applications using C# and .NET.

## Run

```
dotnet watch
```

## Usage
A sample json file is loaded in the editor. The values can be changed. When the button is clicked, the json is deserialized and the values appear on the second card.
We can also change the values on the UI card and serialize it, in which case, the json value is written to the editor.

In both cases, if an error occurs, a toast shows the error.

## Deploy

```
ivy deploy
```