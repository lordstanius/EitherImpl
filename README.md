# Variation of generic "Either" type implementation in C#
Modified (more specific) ["Either"](https://gist.github.com/siliconbrain/3923828) type implementation which might have a better way to tell about it's intentions:

```CSharp
Result<Resource, Error> result = FetchContent(address);

string content = result.Try(res => res.Data).Catch(err => err.Message);
Console.WriteLine(content);

// or
result
  .Try(res => Console.WriteLine(res.Data))
  .Catch(err => Console.WriteLine(err.Message));
```
