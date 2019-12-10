GitWatch
===

[![NuGet](http://img.shields.io/nuget/v/Octokit.svg)](https://www.nuget.org/packages/Octokit)

GitWatch is a web server implementation in C#. Service has been implamented on the ASP.NET Core Web Application platform. Using AspNetCore.App(2.2.0), .NETCore.App(2.2.0), Octokit(0.36.0).

GitWatch doesn't has a user interface, it is only an API for the back part of the servos (possible visualization on Razor in the future)

Client needs to enter his\her username in github. Next enter date of start scanning repositories, if do not this than date will be a default.

Example
---

The following is an example that will echo to an endpoint.

```c#

public async Task<IActionResult> GetGitHubRepositories (string login, DateTime startDate = new DateTime()) =>
            new OkObjectResult(await _projectService.ProgectPages(login, startDate));     
        
```
